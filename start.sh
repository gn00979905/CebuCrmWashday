#!/bin/bash

# 1. 走進洗衣店的資料夾，啟動洗衣店
cd /app/washday
ASPNETCORE_URLS="http://127.0.0.1:5001" ./WASHDAY &

# 2. 回到上一層，稍微等個 3 秒
cd /app
sleep 3

# 3. 走進總機的資料夾，啟動總機
cd /app/cebucrm
export ASPNETCORE_URLS="http://+:$PORT"
./CebuCrmApi