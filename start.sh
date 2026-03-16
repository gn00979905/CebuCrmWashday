#!/bin/bash
set -e

echo "Starting WASHDAY..."
cd /app/washday
ASPNETCORE_URLS=http://127.0.0.1:5001 ./WASHDAY &

echo "Starting CebuCrmApi..."
cd /app/cebucrm
ASPNETCORE_URLS=http://0.0.0.0:$PORT ./CebuCrmApi