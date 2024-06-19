#!/bin/bash

# 文件路径
FILE_PATH="/Users/zhuangshaokun/Downloads/test_zip/character.zip.part0"

# 每秒输出文件大小
while true; do
  SIZE=$(du -sh "$FILE_PATH" | awk '{print $1}')
  TIME=$(date "+%Y-%m-%d %H:%M:%S")
  echo "$TIME - $SIZE"
  sleep 1
done
