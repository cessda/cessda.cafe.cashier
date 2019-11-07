# Dockerfile for Jenkins, use the Dockerfile in the root directory to run locally
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
WORKDIR /app
COPY /publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]