# Usa la imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usa la imagen de SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AppG/AppG.csproj", "AppG/"]
RUN dotnet restore "AppG/AppG.csproj"
COPY . .
WORKDIR "/src/AppG"
RUN dotnet build "AppG.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppG.csproj" -c Release -o /app/publish

# Construye la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppG.dll"]
