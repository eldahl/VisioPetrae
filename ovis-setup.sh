#/bin/bash!

# Download Ovis repo
git clone git@github.com:AIDC-AI/Ovis.git
cd Ovis
# Setup python virtual environment
conda create -n ovis-venv python=3.10 -y
conda activate ovis-venv
# Install requirements for the project through pip
pip install -r requirements.txt
pip install -e .

# Download 8b-GPTQ-Int4 model files
cd models
git clone https://huggingface.co/AIDC-AI/Ovis2-8B-GPTQ-Int4
cd ..

# Install fastapi
pip install "fastapi[standard]"

