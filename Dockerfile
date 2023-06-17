FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app 
COPY . .
RUN dotnet publish -c Release -o /app/dist
WORKDIR /app/dist

EXPOSE 5000

HEALTHCHECK --interval=10s \
    --timeout=30s \
    --start-period=5s \
    --retries=3 \
    CMD curl --fail http://localhost:5000/_health || exit 1

ENTRYPOINT [ "dotnet", "Flaky.Api.dll" ]
