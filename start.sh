#!/bin/bash

# 1. 啟動洗衣店：
# 【注意】不再使用 dotnet 指令！直接執行 Linux 原生檔案，並強制綁定 127.0.0.1
ASPNETCORE_URLS="http://127.0.0.1:5001" ./washday/WASHDAY &

# 2. 稍微等待 3 秒，讓洗衣店 (分機) 完全開機並跑完 Migrate
sleep 3

# 3. 啟動 CRM 總機：
# 【注意】同樣直接執行原生檔案！並乖乖吃 Render 分配的 PORT
export ASPNETCORE_URLS="http://+:$PORT"
./cebucrm/CebuCrmApi
