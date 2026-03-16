#!/bin/bash

# 1. 暫時清空 Render 的強制網址綁定，以免影響洗衣店
export ASPNETCORE_URLS=

# 2. 啟動洗衣店 (背景執行)，明確綁定 5001
dotnet WASHDAY/WASHDAY.dll --urls "http://127.0.0.1:5001" &

# 3. 稍微等個 3 秒，讓洗衣店先跑起來，避免總機啟動太快找不到人
sleep 3

# 4. 把環境變數加回來，讓 CRM 吃 Render 分配的 PORT (這很重要)
export ASPNETCORE_URLS="http://+:$PORT"

# 5. 啟動 CRM 總機 (前景執行)
dotnet cebu-crm/CebuCrmApi.dll