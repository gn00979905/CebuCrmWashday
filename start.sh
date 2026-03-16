#!/bin/bash

# 1. 啟動洗衣店：強制覆蓋 ASPNETCORE_URLS，並明確指定 127.0.0.1 (不要用 localhost)
ASPNETCORE_URLS="http://127.0.0.1:5001" dotnet WASHDAY/WASHDAY.dll &

# 2. 稍微等待 3 秒，確保洗衣店完全啟動
sleep 3

# 3. 啟動總機 CRM：不加前置變數，讓它乖乖吃 Render 分配的 10000 Port
dotnet cebucrm/CebuCrmApi.dll   # (請保留你原本啟動 CRM 的 dll 路徑)