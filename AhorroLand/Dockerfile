# ETAPA 1: BUILD - Compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos de solución y proyectos
COPY AhorroLand.sln ./
COPY AhorroLand.Api/AhorroLand.Api.csproj AhorroLand.Api/
COPY AhorroLand.Application/AhorroLand.Application.csproj AhorroLand.Application/
COPY AhorroLand.Domain/AhorroLand.Domain.csproj AhorroLand.Domain/
COPY AhorroLand.Infrastructure/AhorroLand.Infrastructure.csproj AhorroLand.Infrastructure/
COPY AhorroLand.Shared.Application/AhorroLand.Shared.Application.csproj AhorroLand.Shared.Application/
COPY AhorroLand.Shared.Domain/AhorroLand.Shared.Domain.csproj AhorroLand.Shared.Domain/

# Restaurar dependencias
RUN dotnet restore

# Copiar el resto del código fuente
COPY . .

# Compilar la aplicación
WORKDIR /src/AhorroLand.Api
RUN dotnet build -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 2: RUNTIME - Crear la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copiar los archivos publicados desde la etapa de build
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "AhorroLand.Api.dll"]
