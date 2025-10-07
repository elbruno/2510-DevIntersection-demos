# pip install "mcp" openai
# Environment:
#   MCP_URL=https://your-mcp.example.com/mcp
#   MCP_BEARER_TOKEN=eyJhbGciOi...
#   AZURE_OPENAI_ENDPOINT=https://YOUR-RESOURCE-NAME.openai.azure.com
#   AZURE_OPENAI_API_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxx
#   AZURE_OPENAI_DEPLOYMENT=<your-chat-deployment-name>   # e.g., gpt-4o or gpt-4.1 deployment name

import os, json, asyncio
from openai import OpenAI
from mcp import ClientSession
from mcp.client.streamable_http import streamablehttp_client  # Streamable HTTP transport
from dotenv import load_dotenv

load_dotenv()

# --- Azure OpenAI client (Chat Completions + tool calling) ---
client = OpenAI(
    base_url=f"{os.environ['AZURE_OPENAI_ENDPOINT'].rstrip('/')}/openai/v1/",
    api_key=os.environ["AZURE_OPENAI_API_KEY"],
)
DEPLOYMENT = os.environ["AZURE_OPENAI_DEPLOYMENT"]

MCP_URL = os.environ["MCP_URL"]
MCP_TOKEN = os.environ["MCP_BEARER_TOKEN"]

HEADERS = {
    "Authorization": f"Bearer {MCP_TOKEN}",
    # Add any other headers your server requires
}

async def main():
    # 1) CONNECT to the MCP server (with Bearer token)
    async with streamablehttp_client(MCP_URL, headers=HEADERS) as (read, write, _):
        async with ClientSession(read, write) as session:
            await session.initialize()

            # 2) LIST TOOLS exposed by the server
            tools_resp = await session.list_tools()
            tool_names = [t.name for t in tools_resp.tools]
            print("MCP tools:", tool_names)

            # Convert MCP tools → Azure OpenAI "tools" (function calling) format
            # MCP 'Tool' includes name, description, and inputSchema (JSON Schema)
            aoai_tools = []
            for t in tools_resp.tools:
                desc = (t.description or f"MCP tool {t.name}")[:1000]  # Azure description limit ~1024 chars
                params = t.inputSchema or {"type": "object", "properties": {}}
                aoai_tools.append({
                    "type": "function",
                    "function": {
                        "name": t.name,
                        "description": desc,
                        "parameters": params,
                    },
                })

            # 3) ASK Azure OpenAI to use an MCP tool
            #    Feel free to tailor this prompt to your server's tools (e.g., "add", "search", etc.)
            user_prompt = (
                "Generate a pixelated image of a racoon in Canada using the Flux1 tool from the Hugging Face MCP server"
            )

            messages = [{"role": "user", "content": user_prompt}]
            first = client.chat.completions.create(
                model=DEPLOYMENT,
                messages=messages,
                tools=aoai_tools,
                tool_choice="auto",
            )

            msg = first.choices[0].message
            messages.append(msg)

            # If the model decided to call a tool, execute it on the MCP server
            if msg.tool_calls:
                for call in msg.tool_calls:
                    name = call.function.name
                    args = json.loads(call.function.arguments or "{}")

                    # Execute the MCP tool
                    result = await session.call_tool(name, arguments=args)

                    # Minimal way to feed tool result back to the model
                    # Prefer structuredContent if available; otherwise dump entire result
                    content = (
                        json.dumps(result.structuredContent)
                        if getattr(result, "structuredContent", None) is not None
                        else result.model_dump_json()
                    )

                    messages.append({
                        "role": "tool",
                        "tool_call_id": call.id,
                        "name": name,
                        "content": content,
                    })

                # Ask the model for the final user-facing answer
                final = client.chat.completions.create(
                    model=DEPLOYMENT,
                    messages=messages,
                )
                print("Final answer:", final.choices[0].message.content)
            else:
                print("Model did not call a tool. Try nudging the prompt.")
                

if __name__ == "__main__":
    asyncio.run(main())
