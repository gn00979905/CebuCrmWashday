# === 第一階段：建置程式碼 (使用最通用的 .NET 10 SDK 標籤) ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 複製兩個專案進容器
COPY ./WASHDAY ./WASHDAY
COPY ./CebuCrmApi ./CebuCrmApi

# 編編並發布 WASHDAY (向下相容編譯 .NET 8)
RUN dotnet publish "./WASHDAY/WASHDAY/WASHDAY.csproj" -c Release -r linux-x64 --self-contained true -o /app/publish/washday

# 編譯並發布 CebuCrmApi
RUN dotnet publish ./CebuCrmApi/CebuCrmApi.csproj -c Release -r linux-x64 --self-contained true -o /app/publish/cebucrm

# === 第二階段：最終執行環境 ===
# 【修正點】：使用最通用的 runtime-deps 標籤，不加 OS 代號
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app

# 複製發布好的檔案與腳本
COPY --from=build /app/publish/washday ./washday
COPY --from=build /app/publish/cebucrm ./cebucrm
COPY start.sh ./

# 給予執行權限
RUN chmod +x ./start.sh "./washday/WASHDAY" ./cebucrm/CebuCrmApi

# 設定啟動腳本
CMD ["./start.sh"]