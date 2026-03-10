#!/bin/bash

# 1. 先進入洗衣店資料夾再啟動，這樣它才找得到 wwwroot 和 Pages
cd /app/washday
ASPNETCORE_URLS="http://localhost:5001" "./WASHDAY 202508" &

# 2. 回到 CRM 資料夾啟動大門口
cd /app/cebucrm
ASPNETCORE_URLS="http://0.0.0.0:$PORT" ./CebuCrmApi