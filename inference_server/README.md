# What is this?
This is a simple inference server using FastAPI that interfaces with the Ovis2 Vision Language Model.

# Install

Run inference server setup script with:
```
chmod +x ovis-setup.sh
./ovis-setup.sh
```

# Usage
Run inference server with:
```
conda activate ovis-env
REGISTRY_URL=[Host:Port] OVIS_MODEL=Ovis/models/Ovis2-8B-GPTQ-Int4 fastapi run infer.py > inference_server.log
```

# Credits/License
[Ovis2](https://github.com/AIDC-AI/Ovis) is licensed by the [Apache License | Version 2.0](https://www.apache.org/licenses/LICENSE-2.0.txt)     
[FastAPI](https://github.com/fastapi/fastapi/) is licensed by the [MIT License](https://github.com/fastapi/fastapi/blob/master/LICENSE)
