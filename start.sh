#!/bin/bash

# 1. 走進洗衣店的專屬房間
echo "Starting WASHDAY..."
cd /app/washday
# 【關鍵改變】因為不是原生檔了，要加回 dotnet 指令，並指定 .dll
ASPNETCORE_URLS="http://127.0.0.1:5001" dotnet WASHDAY.dll &

# 2. 回到大廳，給洗衣店 15 秒的時間建立資料庫和拉開鐵門
echo "Waiting for WASHDAY to initialize database and start..."
cd /app
sleep 15

# 3. 走進總機的專屬房間
echo "Starting CebuCrmApi..."
cd /app/cebucrm
export ASPNETCORE_URLS="http://+:$PORT"
# 【關鍵改變】同樣加回 dotnet 指令，並指定 .dll
dotnet CebuCrmApi.dll