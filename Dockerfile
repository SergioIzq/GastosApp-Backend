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
WORKDIR "/src/AppG"
RUN dotnet publish "AppG.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ============================================
# ETAPA 3: RUNTIME - Imagen final optimizada con Alpine
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Establecer el directorio de trabajo
WORKDIR /app

# Instalar curl para health checks e ICU para globalización (Alpine usa apk)
RUN apk add --no-cache curl icu-libs icu-data-full

# Exponer puertos
EXPOSE 80
EXPOSE 443

# Copiar los archivos publicados desde la etapa de publish
COPY --from=publish /app/publish .

# Variables de entorno opcionales
ENV ASPNETCORE_URLS=http://+:80 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Configurar zona horaria (opcional)
RUN apk add --no-cache tzdata
ENV TZ=Europe/Madrid

# Crear usuario no-root para mayor seguridad
RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

# Cambiar al usuario no-root
USER appuser

# Health check (ajusta el endpoint según tu aplicación)
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost/health || exit 1

# Punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "AppG.dll"]
