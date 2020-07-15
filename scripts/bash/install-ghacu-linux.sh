#!/usr/bin/env bash
os='linux-x64'
version='2.0.1'
cd ~ && wget "https://github.com/fabasoad/ghacu/releases/download/v$version/ghacu-$version-$os.tgz"
tar -xvf "ghacu-$version-$os.tgz"
chmod +x "~/ghacu-$version-$os/ghacu"
unlink "/usr/local/bin/ghacu"
ln -s "~/ghacu-$version-$os/ghacu" "/usr/local/bin/ghacu"
