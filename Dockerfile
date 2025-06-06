# Direkt yayın klasörünü IIS'e kopyalayan basit Dockerfile
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8
WORKDIR /inetpub/wwwroot
COPY ./publish/ .
