FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Cashier/Cashier.csproj", "Cashier/"]
RUN dotnet restore "Cashier/Cashier.csproj"
COPY . .
WORKDIR "/src/Cashier"
RUN dotnet build "Cashier.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Cashier.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cashier.dll"]