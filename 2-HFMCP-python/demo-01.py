"""
Hugging Face MCP Server Demo - Image Generation
This demo connects to the Hugging Face MCP Server and generates an image using Flux 1 Schnell.
"""

import asyncio
import os
import base64
from pathlib import Path
from dotenv import load_dotenv
import httpx

# Load environment variables from .env file
load_dotenv()


async def generate_image_with_hf_mcp(prompt: str):
    """
    Connect to Hugging Face MCP Server and generate an image using Flux 1 Schnell.
    
    Args:
        prompt: The text description of the image to generate
    """
    # Get the Hugging Face token from environment variables
    hf_token = os.getenv("HUGGINGFACE_BEARER_TOKEN")
    
    if not hf_token:
        print("❌ Error: HUGGINGFACE_BEARER_TOKEN not found in .env file")
        print("Please create a .env file with your Hugging Face token:")
        print("HUGGINGFACE_BEARER_TOKEN=your_token_here")
        return
    
    print("🚀 Connecting to Hugging Face MCP Server...")
    
    # Hugging Face MCP Server endpoint
    mcp_url = "https://api.huggingface.co/mcp/v1"
    
    headers = {
        "Authorization": f"Bearer {hf_token}",
        "Content-Type": "application/json"
    }
    
    try:
        async with httpx.AsyncClient(timeout=120.0) as client:
            # List available tools
            print("✅ Connected to Hugging Face MCP Server")
            print("\n📋 Listing available tools...")
            
            list_tools_response = await client.post(
                f"{mcp_url}/tools/list",
                headers=headers,
                json={}
            )
            
            if list_tools_response.status_code == 200:
                tools_data = list_tools_response.json()
                tools = tools_data.get("tools", [])
                print(f"   Found {len(tools)} tools")
                for tool in tools[:5]:  # Show first 5 tools
                    print(f"   - {tool.get('name', 'unknown')}")
            
            # Use the provided prompt
            print(f"\n🎨 Generating image with prompt:")
            print(f"   '{prompt}'")
            
            # Call the Flux 1 Schnell image generation tool
            tool_call_response = await client.post(
                f"{mcp_url}/tools/call",
                headers=headers,
                json={
                    "name": "gr1_flux1_schnell_infer",
                    "arguments": {
                        "prompt": prompt,
                        "width": 1024,
                        "height": 1024,
                        "num_inference_steps": 4,
                        "randomize_seed": True
                    }
                }
            )
            
            if tool_call_response.status_code == 200:
                print("✅ Image generated successfully!")
                
                result = tool_call_response.json()
                content_list = result.get("content", [])
                
                # Process the result
                for content in content_list:
                    if content.get("type") == "image":
                        # Save the image
                        output_dir = Path("output")
                        output_dir.mkdir(exist_ok=True)
                        
                        # The image data is in base64 format
                        image_data = base64.b64decode(content.get("data", ""))
                        
                        output_path = output_dir / "generated_image.png"
                        with open(output_path, "wb") as f:
                            f.write(image_data)
                        
                        print(f"💾 Image saved to: {output_path.absolute()}")
                    elif content.get("type") == "text":
                        print(f"📝 Response: {content.get('text', '')}")
            else:
                print(f"❌ Error calling tool: {tool_call_response.status_code}")
                print(f"   Response: {tool_call_response.text}")
                
    except httpx.HTTPStatusError as e:
        print(f"❌ HTTP Error: {e.response.status_code}")
        print(f"   Response: {e.response.text}")
    except Exception as e:
        print(f"❌ Error: {str(e)}")
        print("\nTroubleshooting tips:")
        print("1. Check your Hugging Face token is valid")
        print("2. Verify your internet connection")
        print("3. Ensure the MCP server endpoint is accessible")
        raise


async def main():
    """Main entry point for the demo."""
    print("=" * 60)
    print("🤗 Hugging Face MCP Server - Image Generation Demo")
    print("=" * 60)
    print()
    
    # Define the image generation prompt
    prompt = "A pixelated image of a racoon in Canada"
    
    await generate_image_with_hf_mcp(prompt)
    
    print()
    print("=" * 60)
    print("✨ Demo completed!")
    print("=" * 60)


if __name__ == "__main__":
    asyncio.run(main())
