# Dockerfile for Jenkins, use the Dockerfile in the root directory to run locally
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY /publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]