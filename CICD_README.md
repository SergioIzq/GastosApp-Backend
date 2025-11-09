# 🚀 CI/CD Pipeline - AhorroLand Backend

Este repositorio incluye un pipeline de CI/CD con GitHub Actions que automáticamente construye y publica imágenes Docker del backend en Docker Hub.

## 📁 Estructura del Proyecto

Este repositorio maneja **dos estructuras diferentes** según la rama:

### Rama `main`:
```
AhorroLand-Backend/
├── AppG.sln                    # Solución principal
├── AppG/                       # Proyecto Web API
│   ├── AppG.csproj
│   ├── Program.cs
│   ├── Startup.cs
│   └── ...
├── NHTools/                    # Herramienta Windows (no se incluye en Docker)
├── Dockerfile                  # Dockerfile optimizado
└── .github/workflows/ci-cd.yml # Pipeline CI/CD
```

### Rama `feature/clean-architecture`:
```
AhorroLand-Backend/
├── AhorroLand/
│   ├── AhorroLand.sln          # Solución Clean Architecture
│   ├── AhorroLand.Api/
│   ├── AhorroLand.Application/
│   ├── AhorroLand.Domain/
│   ├── AhorroLand.Infrastructure/
│   ├── Dockerfile
│   └── ...
└── .github/workflows/ci-cd.yml # Pipeline CI/CD (mismo)
```

## 🎯 Detección Automática

El workflow de GitHub Actions **detecta automáticamente** qué estructura estás usando:

- ✅ Si encuentra `AppG.sln` → Usa estructura de `main`
- ✅ Si encuentra `AhorroLand/AhorroLand.sln` → Usa estructura de `feature/clean-architecture`

**No necesitas hacer nada**, el pipeline se adapta solo.

## 📋 Requisitos Previos

1. **Cuenta de Docker Hub**: Necesitas tener una cuenta en [Docker Hub](https://hub.docker.com/)
2. **Repositorio Docker Hub**: El repositorio `sergioizqdev/ahorroland-backend` debe existir en Docker Hub

## 🔧 Configuración en GitHub

### Paso 1: Agregar Secrets

Ve a tu repositorio en GitHub → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**

Agrega los siguientes secrets:

| Secret Name | Descripción | Ejemplo |
|-------------|-------------|---------|
| `DOCKER_USERNAME` | Tu usuario de Docker Hub | `sergioizqdev` |
| `DOCKER_PASSWORD` | Tu token de acceso o contraseña de Docker Hub | `dckr_pat_...` |

#### ¿Cómo obtener un token de Docker Hub?
1. Ve a [Docker Hub](https://hub.docker.com/)
2. Inicia sesión
3. Ve a **Account Settings** → **Security** → **New Access Token**
4. Dale un nombre descriptivo (ej: "GitHub Actions")
5. Copia el token generado y úsalo como `DOCKER_PASSWORD`

### Paso 2: Verificar el Workflow

El archivo de workflow está ubicado en `.github/workflows/ci-cd.yml`

## 🎯 ¿Cómo Funciona?

### Triggers (Disparadores)

El pipeline se ejecuta automáticamente cuando:

- ✅ Se hace **push** a las ramas `main`, `master`, `develop` o `feature/clean-architecture`
- ✅ Se crean **tags** con formato `v*.*.*` (ej: `v1.0.0`)
- ✅ Se abren **Pull Requests** hacia `main`, `master` o `develop`
- ✅ Se ejecuta **manualmente** desde GitHub Actions

### Stages (Etapas)

#### 1. **Detect Project Structure** 🔍
- Detecta automáticamente la estructura del proyecto
- Determina qué solución compilar
- Configura las rutas para Docker

#### 2. **Build and Test** 🧪
- Descarga el código
- Configura .NET 8.0
- Restaura las dependencias
- Compila el proyecto
- Ejecuta las pruebas unitarias

#### 3. **Docker Build and Push** 🐳
- Construye la imagen Docker usando el Dockerfile optimizado
- Genera tags automáticos basados en:
  - Rama actual (ej: `main`, `develop`, `feature-clean-architecture`)
  - Commit SHA (ej: `main-abc1234`)
  - Versión semántica si es un tag (ej: `1.0.0`, `1.0`, `1`)
  - `latest` para la rama principal
- Sube la imagen a Docker Hub
- Utiliza caché para optimizar builds futuros
- Genera imágenes multi-arquitectura (amd64, arm64)

#### 4. **Notify** 📢
- Notifica el resultado del despliegue con información detallada

## 🏷️ Sistema de Tags

### Tags Automáticos por Rama
```bash
# Push a main
→ sergioizqdev/ahorroland-backend:main
→ sergioizqdev/ahorroland-backend:latest

# Push a develop
→ sergioizqdev/ahorroland-backend:develop

# Push a feature/clean-architecture
→ sergioizqdev/ahorroland-backend:feature-clean-architecture

# Commit específico
→ sergioizqdev/ahorroland-backend:main-abc1234
```

### Tags por Versión (Releases)
```bash
# Tag: v1.2.3
→ sergioizqdev/ahorroland-backend:1.2.3
→ sergioizqdev/ahorroland-backend:1.2
→ sergioizqdev/ahorroland-backend:1
→ sergioizqdev/ahorroland-backend:latest
```

## 📦 Uso de las Imágenes

### Despliegue Manual
```bash
# Última versión estable
docker pull sergioizqdev/ahorroland-backend:latest

# Versión específica
docker pull sergioizqdev/ahorroland-backend:1.2.3

# Rama de desarrollo
docker pull sergioizqdev/ahorroland-backend:develop

# Rama feature
docker pull sergioizqdev/ahorroland-backend:feature-clean-architecture
```

### En docker-compose.prod.yml
```yaml
services:
  api:
    image: sergioizqdev/ahorroland-backend:${API_VERSION:-latest}
    # ...resto de configuración
```

Para usar una versión específica:
```bash
API_VERSION=1.2.3 docker-compose -f docker-compose.prod.yml up -d
```

## 🔄 Workflow de Desarrollo

### Para desarrollo en rama main:

```bash
# 1. Haz tus cambios en main
git checkout main
git pull origin main
# ... hacer cambios ...
git add .
git commit -m "feat: nueva funcionalidad"
git push origin main

# 2. El pipeline construirá automáticamente la imagen con tag 'main' y 'latest'
```

### Para desarrollo en Clean Architecture:

```bash
# 1. Haz tus cambios en feature/clean-architecture
git checkout feature/clean-architecture
git pull origin feature/clean-architecture
# ... hacer cambios ...
git add .
git commit -m "feat: nueva funcionalidad"
git push origin feature/clean-architecture

# 2. El pipeline construirá automáticamente la imagen con tag 'feature-clean-architecture'
```

### Para crear un release desde main:

```bash
# 1. Asegúrate de estar en la rama principal
git checkout main
git pull origin main

# 2. Crea un tag con la versión
git tag -a v1.2.3 -m "Release version 1.2.3"

# 3. Sube el tag a GitHub
git push origin v1.2.3

# 4. El pipeline se ejecutará automáticamente
```

## 🐳 Dockerfile Optimizado

El Dockerfile incluye:

- ✅ **Multi-stage build** para imágenes pequeñas
- ✅ **Caché de dependencias** optimizado
- ✅ **Health check** integrado
- ✅ **Variables de entorno** configurables
- ✅ **Seguridad** mejorada
- ✅ **Imagen final mínima** (solo runtime)

### Características del Dockerfile:

1. **Etapa BUILD**: Compila la aplicación con SDK completo
2. **Etapa PUBLISH**: Publica la aplicación optimizada
3. **Etapa RUNTIME**: Imagen final con solo ASP.NET Core runtime

### Health Check:

El Dockerfile incluye un health check que verifica `/health`. Asegúrate de tener este endpoint en tu API:

```csharp
// En Program.cs o Startup.cs
app.MapHealthChecks("/health");
```

Si no tienes este endpoint, puedes modificar el health check en el Dockerfile:

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost/api/status || exit 1
```

## 🐛 Troubleshooting

### Error: "authentication required"
- Verifica que los secrets `DOCKER_USERNAME` y `DOCKER_PASSWORD` estén correctamente configurados
- Asegúrate de que el token de Docker Hub tenga permisos de escritura

### Error: "repository does not exist"
- Crea el repositorio en Docker Hub antes de ejecutar el pipeline
- O cambia el nombre del repositorio en el workflow (`DOCKER_IMAGE` en el env)

### Error en la compilación
- Revisa los logs del job "Build and Test"
- Asegúrate de que todas las dependencias estén correctamente restauradas
- Verifica que la solución compile localmente:
  ```bash
  # Para main
  dotnet restore AppG.sln
  dotnet build AppG.sln
  
  # Para feature/clean-architecture
  dotnet restore AhorroLand/AhorroLand.sln
  dotnet build AhorroLand/AhorroLand.sln
  ```

### Error "No solution file found"
- El workflow no pudo detectar la estructura del proyecto
- Verifica que exista `AppG.sln` o `AhorroLand/AhorroLand.sln`
- Revisa los logs del job "Detect Project Structure"

### Health check falla
- Asegúrate de tener un endpoint `/health` en tu API
- O modifica el health check en el Dockerfile para usar otro endpoint
- Puedes comentar temporalmente el health check si no lo necesitas

## 📊 Monitoreo

Puedes ver el estado de tus pipelines en:
- **GitHub**: `https://github.com/SergioIzq/AhorroLand-Backend/actions`
- **Docker Hub**: `https://hub.docker.com/r/sergioizqdev/ahorroland-backend/tags`

## 🔒 Mejores Prácticas

1. ✅ Usa siempre tokens de acceso en lugar de contraseñas
2. ✅ Nunca comitees credenciales en el código
3. ✅ Usa versiones específicas en producción (no `latest`)
4. ✅ Prueba localmente antes de hacer push:
   ```bash
   # Compilar
   dotnet build AppG.sln
   
   # Construir imagen Docker
   docker build -t test-backend .
   
   # Probar la imagen
   docker run -p 8080:80 test-backend
   ```
5. ✅ Revisa los logs del pipeline cuando falle
6. ✅ Mantén el `.dockerignore` actualizado

## 🚀 Build Local con Docker

Para probar la imagen Docker localmente:

```bash
# Construir la imagen
docker build -t ahorroland-backend:local .

# Ejecutar el contenedor
docker run -d -p 8080:80 --name backend-test ahorroland-backend:local

# Ver logs
docker logs -f backend-test

# Probar la API
curl http://localhost:8080/health

# Detener y eliminar
docker stop backend-test
docker rm backend-test
```

## 📝 Notas

- Las imágenes están optimizadas con caché para builds más rápidos
- Se generan imágenes multi-arquitectura (AMD64 y ARM64)
- Los Pull Requests solo ejecutan build y tests, no publican imágenes
- El proyecto `NHTools` (Windows Forms) se excluye automáticamente del Docker build
- El workflow funciona en ambas ramas sin modificaciones

## 🔧 Personalización

### Cambiar el nombre de la imagen:

Edita el archivo `.github/workflows/ci-cd.yml`:

```yaml
env:
  DOCKER_IMAGE: tu-usuario/tu-repositorio
```

### Agregar más ramas al pipeline:

```yaml
on:
  push:
    branches:
      - main
      - develop
      - tu-nueva-rama
```

### Modificar el health check:

Edita el `Dockerfile`:

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost/api/tu-endpoint || exit 1
```

## 🎓 Recursos

- [Documentación de .NET en Docker](https://docs.microsoft.com/en-us/dotnet/core/docker/introduction)
- [GitHub Actions para .NET](https://docs.github.com/en/actions/guides/building-and-testing-net)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

---

**¿Necesitas ayuda?** Abre un issue en el repositorio o consulta los logs del pipeline en GitHub Actions.
