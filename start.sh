#!/bin/bash

# 1. 啟動洗衣店 (WASHDAY)
cd /app/washday
# 既然已經改名沒空格了，"./WASHDAY" 的引號可加可不加，建議保持一致
ASPNETCORE_URLS="http://127.0.0.1:5001" ./WASHDAY &

# 2. 啟動 CRM 大門 (CebuCrmApi)
cd /app/cebucrm
# 使用 exec 可以讓這個程序接收 Render 的關閉訊號，部署會更穩定
ASPNETCORE_URLS="http://0.0.0.0:$PORT" exec ./CebuCrmApi