# === 第一階段：build ===
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ./WASHDAY ./WASHDAY
COPY ./CebuCrmApi ./CebuCrmApi

RUN dotnet publish ./WASHDAY/WASHDAY/WASHDAY.csproj -c Release -o /app/publish/washday
RUN dotnet publish ./CebuCrmApi/CebuCrmApi.csproj -c Release -o /app/publish/cebucrm


# === 第二階段：runtime ===
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish/washday ./washday
COPY --from=build /app/publish/cebucrm ./cebucrm
COPY start.sh ./

RUN chmod +x ./start.sh

CMD ["./start.sh"]
