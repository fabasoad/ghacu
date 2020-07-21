#!/usr/bin/env bash
os='linux-x64'
asset=$(curl -s https://api.github.com/repos/fabasoad/ghacu/releases/latest | jq -c '.assets[] | select(.name | contains('"\"$os.tgz\""'))')
cd ~ && echo ${asset} | jq -r '.browser_download_url' | xargs wget
tar_name=$(echo ${asset} | jq -r '.name')
tar -xvf ${tar_name}
folder=$(basename ${tar_name} .tgz)
PATH=$PATH:~/${folder}
chmod +x ~/${folder}/ghacu
ln -sfn ~/${folder}/ghacu /usr/local/bin/ghacu
