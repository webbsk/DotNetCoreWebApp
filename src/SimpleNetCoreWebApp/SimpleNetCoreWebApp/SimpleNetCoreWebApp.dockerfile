FROM microsoft/dotnet:2.2.103-sdk-alpine3.8

WORKDIR /app

COPY ./bin/Release/netcoreapp2.2 SimpleNetCoreWebApp

EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT dotnet SimpleNetCoreWebApp/SimpleNetCoreWebApp.dll -- -RunContainer