#!/bin/bash

# 1. 啟動 WASHDAY
# --contentRoot 指定程式檔案路徑，--urls 指定監聽位址
./washday/"WASHDAY 202508" --urls "http://localhost:5001" --contentRoot /app/washday &

# 2. 啟動 CebuCrmApi (門口守衛)
# 這裡一樣指定路徑，並使用 Render 分配的 $PORT
./cebucrm/CebuCrmApi --urls "http://0.0.0.0:$PORT" --contentRoot /app/cebucrm