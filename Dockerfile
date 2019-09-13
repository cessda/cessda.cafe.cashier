# Dockerfile for Jenkins, use the Dockerfile in ./Cashier to run locally
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
WORKDIR /app
COPY /Cashier/publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]