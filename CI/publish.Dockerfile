# Dockerfile for CI, use the Dockerfile in the Cashier directory to run locally
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY /publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]