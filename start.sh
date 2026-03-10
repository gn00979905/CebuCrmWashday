#!/bin/bash

# 1. 啟動舊專案 (WASHDAY) 於背景執行
# 執行檔的名稱會跟 .csproj 一樣，所以包含空白，必須加雙引號
ASPNETCORE_URLS="http://localhost:5001" "./washday/WASHDAY 202508" &

# 2. 啟動新專案 (cebu-crm) 於前景執行
ASPNETCORE_URLS="http://0.0.0.0:$PORT" ./cebucrm/CebuCrmApi