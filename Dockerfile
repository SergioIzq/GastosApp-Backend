# Usa la imagen base de .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Usa la imagen de SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo .csproj y restaura las dependencias
COPY ["Proyecto Integrado/AppG.csproj", "Proyecto Integrado/"]
RUN dotnet restore "Proyecto Integrado/AppG.csproj"

# Copia el resto del código y construye la aplicación
COPY . .
WORKDIR "/src/Proyecto Integrado"
RUN dotnet build "AppG.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "AppG.csproj" -c Release -o /app/publish

# Construye la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppG.dll"]
