# Usar uma imagem base do .NET 6
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usar uma imagem base do SDK do .NET 6 para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Temperatura_api.csproj", "."]
RUN dotnet restore "Temperatura_api.csproj"
COPY . .
RUN dotnet build "Temperatura_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Temperatura_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Temperatura_api.dll"]
