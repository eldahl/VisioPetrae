import os
import torch
from PIL import Image
from transformers import GenerationConfig
from gptqmodel import GPTQModel

from fastapi import FastAPI

app = FastAPI()

@app.post("/infer")
def infer():
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
    image_path = os.environ['OVIS_IMAGE']
    images = [Image.open(image_path)]
    max_partition = 9
    #text = input("Enter prompt: ")
    text = os.environ['OVIS_PROMPT']
    query = f'<image>\n{text}'

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
