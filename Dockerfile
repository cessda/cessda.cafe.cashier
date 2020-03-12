# Dockerfile for Jenkins, use the Dockerfile in the root directory to run locally
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY /publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]