# Hugging Face MCP Server - Python Demo

This demo shows how to connect to the Hugging Face MCP Server via HTTP and generate images using the Flux 1 Schnell model.

## Prerequisites

- Python 3.9 or higher
- A Hugging Face account and bearer token

## Setup

1. **Install Python dependencies:**

   ```bash
   pip install -r requirements.txt
   ```

2. **Create a `.env` file:**
   Copy `.env.example` to `.env` and add your Hugging Face token:

   ```bash
   copy .env.example .env
   ```

   Then edit `.env` and replace `your_huggingface_token_here` with your actual token.

3. **Get your Hugging Face token:**
   - Go to <https://huggingface.co/settings/tokens>
   - Create a new token or use an existing one
   - Copy the token to your `.env` file

## Running the Demo

```bash
python demo-01.py
```

The demo will:

1. Connect to the Hugging Face MCP Server via HTTP
2. List available tools
3. Generate an image using Flux 1 Schnell
4. Save the generated image to the `output/` directory

## How It Works

The demo uses HTTP requests to communicate with Hugging Face's MCP Server:

1. **Connection**: Makes HTTP requests to the Hugging Face MCP API endpoint
2. **Authentication**: Uses your bearer token in the Authorization header
3. **Image Generation**: Calls the `gr1_flux1_schnell_infer` tool with a text prompt
4. **Output**: Saves the base64-encoded image to disk

## Customization

You can modify the image generation parameters in `demo-01.py`:

- `prompt`: The text description of the image to generate
- `width` / `height`: Image dimensions (256-2048 pixels)
- `num_inference_steps`: Number of denoising steps (1-16)
- `randomize_seed`: Whether to use a random seed for generation

## Troubleshooting

- **Invalid token**: Check your token at <https://huggingface.co/settings/tokens>
- **Connection issues**: Verify your internet connection and firewall settings
- **HTTP errors**: Check the error message and status code for details
