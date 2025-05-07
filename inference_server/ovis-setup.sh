#/bin/bash!
# Conda context for the shell script
conda init
source ~/.bashrc
#source /opt/anaconda/etc/profile.d/conda.sh

# Download Ovis repo
#git clone https://github.com/AIDC-AI/Ovis.git
cd Ovis
# Setup python virtual environment
conda create -n ovis-env python=3.10 -y
conda activate ovis-env

pip cache purge

# Install requirements for the project through pip
#pip install -r requirements.txt
#pip install -e .
pip install torch==2.4.0 transformers==4.49.0 pillow==10.3.0
pip install flash-attn==2.7.0.post2 --no-build-isolation
pip install gptqmodel
pip install numpy==1.25.0
#pip install auto-gptq optimum

# Download 8b-GPTQ-Int4 model files
mkdir models
cd models
#git clone https://huggingface.co/AIDC-AI/Ovis2-8B-GPTQ-Int4
cd ..

# Install fastapi
pip install "fastapi[standard]"
