#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["Cashier/Cashier.csproj", "Cashier/"]
RUN dotnet restore "Cashier/Cashier.csproj"
COPY . .
WORKDIR "/src/Cashier"
RUN dotnet build "Cashier.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cashier.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cashier.dll"]