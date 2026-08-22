## Requisitos

- .NET SDK 10
- SQL Server local o Docker Desktop

Comprueba la versión de .NET:

```powershell
#Debe mostrar una versión `10.x`.
dotnet --version
```


## Levantar el proyecto

### 1. Configurar SQL Server

La API utiliza la cadena de conexión ubicada en:

```text
src/Api/appsettings.Development.json
```

Puedes utilizar una instancia local de SQL Server o Docker. En ambos casos, verifica que `ConnectionStrings:Default` apunte a la instancia, base de datos y credenciales disponibles en tu entorno.

#### Opción A: usar Docker Desktop

Desde la raíz del repositorio:

```powershell
docker compose up -d
```

La cadena de conexión incluida por defecto ya está configurada para ese contenedor.

Verifica que el contenedor esté corriendo:

```powershell
docker ps
```

#### Opción B: usar SQL Server local

Actualiza `ConnectionStrings:Default` en `src/Api/appsettings.Development.json` según tu instancia local.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=metropolitan;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Restaurar dependencias

```powershell
dotnet restore Backend.slnx
```

### 3. Ejecutar la API

```powershell
dotnet run --project src/Api
```

Al iniciar, la API aplica automáticamente las migraciones pendientes y carga los datos iniciales si la base de datos está vacía.

El backend permite por defecto solicitudes desde este origen:

```text
http://localhost:5173
```

Si el frontend se ejecuta en otro puerto, agregar al arreglo `Cors:AllowedOrigins` de `src/Api/appsettings.Development.json`.

## Ejecutar pruebas unitarias

```powershell
dotnet test Backend.slnx
```

## Problemas comunes para arrancar el servidor

| Problema | Solución |
|---|---|
| Error de conexión a SQL Server | Verifica la cadena de conexión y que SQL Server esté activo. |
| Puerto `1433` ocupado | Cambia el puerto en `docker-compose.yml` y actualiza `ConnectionStrings:Default` en `src/Api/appsettings.Development.json`. |
| Error CORS desde el frontend | Verifica que el origen del frontend esté incluido en `Cors:AllowedOrigins` y que `VITE_API_URL` apunte a `http://localhost:5268/api`. |

---
## Stack utilizado

| Categoría | Tecnología |
|---|---|
| Runtime / Framework | .NET 10 / ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Base de datos | SQL Server |
| Validaciones | FluentValidation |
| Tests | xUnit, EF Core InMemory, TimeProvider.Testing |
| Contenedor local | Docker Compose |

