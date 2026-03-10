# === 第一階段：建置程式碼 ===
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 複製兩個專案進容器
COPY ./WASHDAY ./WASHDAY
COPY ./CebuCrmApi ./CebuCrmApi

# 編譯並發布 WASHDAY (注意雙層資料夾與引號)
RUN dotnet publish "./WASHDAY/WASHDAY/WASHDAY 202508.csproj" -c Release -r linux-x64 --self-contained true -o /app/publish/washday

# 編譯並發布 CebuCrmApi
RUN dotnet publish ./CebuCrmApi/CebuCrmApi.csproj -c Release -r linux-x64 --self-contained true -o /app/publish/cebucrm

# === 第二階段：最終執行環境 ===
# 【再次修正】：改用官方最穩定的 bookworm-slim (Debian 12)
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-bookworm-slim AS final
WORKDIR /app

# 複製發布好的檔案與腳本
COPY --from=build /app/publish/washday ./washday
COPY --from=build /app/publish/cebucrm ./cebucrm
COPY start.sh ./

# 給予執行權限
RUN chmod +x ./start.sh "./washday/WASHDAY 202508" ./cebucrm/CebuCrmApi

# 設定啟動腳本
CMD ["./start.sh"]