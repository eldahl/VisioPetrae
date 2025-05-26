import os
import torch
from PIL import Image
from transformers import GenerationConfig
from gptqmodel import GPTQModel

from fastapi import FastAPI, UploadFile, Form, File
from fastapi.responses import HTMLResponse, PlainTextResponse
from fastapi.middleware.cors import CORSMiddleware

from contextlib import asynccontextmanager

import requests

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Or specify ["http://localhost:5500"] or your frontend URL
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

UPLOAD_DIR = "uploads"
os.makedirs(UPLOAD_DIR, exist_ok=True)

REGISTRY_URL = os.environ['REGISTRY_URL']

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Send registration request to registry
    response = requests.post(f"{REGISTRY_URL}/Registry", json={{
      "Hostname": "localhost",
      "Port": 8000,
      "MaxTasks": 5,
      "IsAvailable": True,
      "Status": "Online"
    }})
    print("Registration response: " + str(response.status_code))
    print(response.json())
    uuid = response.json.__dict__.get("uuid")
    yield
    # Deregister from registry
    response = requests.delete(f"{REGISTRY_URL}/Registry/{uuid}")
    print("Deregistration response: " + str(response.status_code))

@app.get("/", response_class=HTMLResponse)
async def inferPage():
    return """
    <!DOCTYPE html>
    <html lang="en">
    <head>
      <meta charset="UTF-8">
      <title>Image + Prompt Upload</title>
      <style>
        body {
          font-family: sans-serif;
          max-width: 500px;
          margin: 2rem auto;
        }
        label, input, textarea, button {
          display: block;
          width: 100%;
          margin-bottom: 1rem;
        }
      </style>
    </head>
    <body>
      <h2>Upload Image with Prompt</h2>
      <form id="uploadForm">
        <label for="image">Choose an image:</label>
        <input type="file" id="image" name="image" accept="image/*" required>

        <label for="prompt">Enter prompt:</label>
        <textarea id="prompt" name="prompt" rows="4" required></textarea>

        <button type="submit">Send to Server</button>
      </form>

      <div id="response"></div>

      <script>
        document.getElementById('uploadForm').addEventListener('submit', async (e) => {
          e.preventDefault();

          const form = e.target;
          const formData = new FormData();
          const imageFile = form.image.files[0];
          const promptText = form.prompt.value;

          formData.append('image', imageFile);
          formData.append('prompt', promptText);

          try {
            const response = await fetch('http://192.168.1.229:8000/infer', {
              method: 'POST',
              body: formData
            });

            const result = await response.text();
            document.getElementById('response').innerText = result;
          } catch (error) {
            document.getElementById('response').innerText = 'Error: ' + error.message;
          }
        });
      </script>
    </body>
    </html>
    """

@app.get("/heartbeat", response_class=PlainTextResponse)
async def heartbeat():
    return "OK"

@app.post("/infer", response_class=PlainTextResponse)
async def infer(image: UploadFile = File(...), prompt: str = Form(...)):
    # Save file to disk
    file_location = os.path.join(UPLOAD_DIR, image.filename)
    with open(file_location, "wb") as f:
        content = await image.read()
        f.write(content)

    # load model
    # customize load device
    load_device = "cuda:0"
    torch.cuda.set_device(load_device)
    # We take AIDC-AI/Ovis2-34B-GPTQ-Int4 as an example. Note that the code snippet is 
    # applicable to any GPTQ-quantized Ovis2 model.
    modelEnvVar = os.environ['OVIS_MODEL']
    model = GPTQModel.load(modelEnvVar, device=load_device, trust_remote_code=True)
    model.model.generation_config = GenerationConfig.from_pretrained(modelEnvVar)
    text_tokenizer = model.get_text_tokenizer()
    visual_tokenizer = model.get_visual_tokenizer()

    # For inference, quantization affects only the model loading part. The rest is the same 
    # as unquantized Ovis2 models. Here we show how to inference with single image input 
    # and without batching. For other input types and batch inference, please refer to
    # https://huggingface.co/AIDC-AI/Ovis2-34B.
    #image_path = input("Enter image path: ")
    #image_path = os.environ['OVIS_IMAGE']
    image_path = file_location #os.environ['OVIS_IMAGE']
    images = [Image.open(image_path)]
    max_partition = 9
    #text = input("Enter prompt: ")
    #text = os.environ['OVIS_PROMPT']
    #text = prompt #os.environ['OVIS_PROMPT']
    query = f'<image>\n{prompt}'

    # format conversation
    prompt, input_ids, pixel_values = model.preprocess_inputs(query, images, max_partition=max_partition)
    attention_mask = torch.ne(input_ids, text_tokenizer.pad_token_id)
    input_ids = input_ids.unsqueeze(0).to(device=model.device)
    attention_mask = attention_mask.unsqueeze(0).to(device=model.device)
    if pixel_values is not None:
        pixel_values = pixel_values.to(dtype=visual_tokenizer.dtype, device=visual_tokenizer.device)
    pixel_values = [pixel_values]
    
    with torch.inference_mode():
        gen_kwargs = dict(
            max_new_tokens=1024,
            do_sample=False,
            top_p=None,
            top_k=None,
            temperature=None,
            repetition_penalty=None,
            eos_token_id=model.generation_config.eos_token_id,
            pad_token_id=text_tokenizer.pad_token_id,
            use_cache=True
        )
        output_ids = model.generate(input_ids, pixel_values=pixel_values, attention_mask=attention_mask, **gen_kwargs)[0]
        output = text_tokenizer.decode(output_ids, skip_special_tokens=True)
        return output
