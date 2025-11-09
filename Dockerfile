# ============================================
# ETAPA 1: BUILD - Compilar la aplicación
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y proyectos para restaurar dependencias
# Esto aprovecha la caché de Docker cuando solo cambian archivos de código
COPY ["AppG.sln", "./"]
COPY ["AppG/AppG.csproj", "AppG/"]

# Restaurar dependencias
RUN dotnet restore "AppG.sln"

# Copiar todo el código fuente
COPY . .

# Compilar en modo Release
WORKDIR "/src/AppG"
RUN dotnet build "AppG.csproj" -c Release -o /app/build --no-restore

# ============================================
# ETAPA 2: PUBLISH - Publicar la aplicación
# ============================================
FROM build AS publish
RUN dotnet publish "AppG.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    --no-build \
    /p:UseAppHost=false

# ============================================
# ETAPA 3: RUNTIME - Imagen final optimizada
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Establecer el directorio de trabajo
WORKDIR /app

# Exponer puertos
EXPOSE 80
EXPOSE 443

# Copiar los archivos publicados desde la etapa de publish
COPY --from=publish /app/publish .

# Variables de entorno opcionales
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Usuario no-root para seguridad (opcional pero recomendado)
# USER $APP_UID

# Health check (ajusta el endpoint según tu aplicación)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost/health || exit 1

# Punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "AppG.dll"]
