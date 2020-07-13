#!/usr/bin/env bash
cd ~ && wget https://github.com/fabasoad/ghacu/releases/download/v2.0.1/ghacu-2.0.1-linux-x64.tgz
tar -xvf ghacu-2.0.1-linux-x64.tgz
chmod +x ~/ghacu-2.0.1-linux-x64/ghacu
ln -s ~/ghacu-2.0.1-linux-x64/ghacu /usr/local/bin/ghacu
