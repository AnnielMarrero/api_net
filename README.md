# Task Management API

API REST para gestionar tareas con ASP.NET Core 9, Entity Framework Core y SQL Server.

## Funcionalidad

- Crear, listar, consultar, editar y eliminar tareas.
- Campos: título, descripción, estado (`Pending`, `InProgress`, `Completed`), responsable y fecha límite UTC.
- Filtros por estado y responsable.
- Paginación con `page` y `pageSize` (máximo 100 elementos por página).
- Respuestas de error en formato Problem Details y validación con FluentValidation.

## Arquitectura

El código está separado por responsabilidades dentro del proyecto API:

- `Domain`: entidad `TaskItem` y enumeraciones.
- `Application/DTOs`: contratos HTTP y respuesta paginada.
- `Application/Services`: casos de uso y reglas de orquestación.
- `Application/Repositories`: interfaces de repositorio y Unit of Work.
- `Application/Validators`: validaciones de entrada.
- `Infrastructure/Persistence`: `DbContext`, configuración EF Core, repositorio, Unit of Work y migraciones.
- `Controllers`: capa HTTP delgada que delega en el servicio.

El repositorio encapsula las consultas de EF Core y `UnitOfWork` centraliza `SaveChangesAsync`. El estado se almacena como texto en SQL Server para que los datos sean legibles y más estables frente a cambios en el orden del enum.

## Requisitos

- .NET SDK 9.0 o superior.
- Docker Desktop, o una instancia SQL Server accesible.
- La herramienta local `dotnet-ef` 9.0.10, restaurable con `dotnet tool restore`.

## Ejecución local con SQL Server

1. Iniciar SQL Server:

   ```powershell
   Copy-Item .env.example .env
   docker compose up -d sqlserver
   ```

   Para que `dotnet ef` y la API reciban las mismas variables en PowerShell, cárgalas también en la sesión actual:

   ```powershell
   $env:DB_SERVER = "localhost"
   $env:DB_PORT = "1433"
   $env:DB_NAME = "TaskManagement"
   $env:DB_USER = "sa"
   $env:DB_PASSWORD = "Your_password123"
   $env:DB_TRUST_SERVER_CERTIFICATE = "true"
   $env:DB_ENCRYPT = "false"
   ```

2. Aplicar la migración inicial:

   ```bash
   dotnet tool restore
   dotnet ef database update \
     --project src/TaskManagement.Api \
     --startup-project src/TaskManagement.Api
   ```

   En PowerShell, el mismo comando puede escribirse en una sola línea.

3. Arrancar la API:

   ```bash
   dotnet run --project src/TaskManagement.Api
   or
   $env:DB_SERVER="localhost"; $env:DB_PORT="1433"; $env:DB_NAME="TaskManagement"; $env:DB_USER="anniel"; $env:DB_PASSWORD="anniel"; $env:DB_TRUST_SERVER_CERTIFICATE="true"; $env:DB_ENCRYPT="false"; dotnet run --project .\src\TaskManagement.Api\TaskManagement.Api.csproj --urls http://localhost:5014
   ```

La conexión se configura mediante variables de entorno. La API acepta una cadena completa en `ConnectionStrings__DefaultConnection` o construye la conexión desde `DB_SERVER`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`, `DB_TRUST_SERVER_CERTIFICATE` y `DB_ENCRYPT`.

Para una instancia local por defecto de SQL Server, deja `DB_PORT` sin definir: así el cliente puede resolver Shared Memory o la instancia local. Define `DB_PORT=1433` únicamente cuando TCP/IP esté habilitado y SQL Server escuche explícitamente en ese puerto.

Para ejecutar la API desde PowerShell con el SQL Server del compose:

```powershell
$env:DB_SERVER = "localhost"
$env:DB_PORT = "1433"
$env:DB_NAME = "TaskManagement"
$env:DB_USER = "sa"
$env:DB_PASSWORD = "Your_password123"
$env:DB_TRUST_SERVER_CERTIFICATE = "true"
$env:DB_ENCRYPT = "false"
dotnet run --project src/TaskManagement.Api
```

`.env` está excluido de Git. En un entorno real deben usarse variables del sistema, User Secrets o un gestor de secretos; no se deben versionar credenciales reales.

## Endpoints principales

| Método | Ruta | Resultado |
| --- | --- | --- |
| `POST` | `/api/tasks` | Crea una tarea y devuelve `201 Created`. |
| `GET` | `/api/tasks` | Lista con filtros y paginación. |
| `GET` | `/api/tasks/{id}` | Obtiene una tarea. |
| `PUT` | `/api/tasks/{id}` | Reemplaza los datos editables y devuelve `204 No Content`. |
| `DELETE` | `/api/tasks/{id}` | Elimina la tarea y devuelve `204 No Content`. |

Ejemplo de consulta:

```text
GET /api/tasks?status=Pending&responsible=Ana&page=1&pageSize=20
```

Ejemplo de cuerpo para crear o editar:

```json
{
  "title": "Preparar informe",
  "description": "Revisar los datos del trimestre",
  "status": "InProgress",
  "responsible": "Ana",
  "dueDateUtc": "2026-12-31T00:00:00Z"
}
```

## Tests

Los tests usan SQLite en memoria para comprobar el servicio y el repositorio sin depender de una instancia SQL Server:

```bash
dotnet test
```

Actualmente cubren creación y lectura, filtros/paginación, actualización, eliminación y el caso de recurso inexistente.

## Decisiones y siguientes pasos

- Se eligió SQL Server porque es un requisito explícito y EF Core permite mantener el acceso a datos aislado del dominio.
- Se usa SQLite únicamente en tests para que sean rápidos, deterministas y ejecutables en CI.
- Se dejan fuera autenticación/autorización, control de concurrencia, auditoría, búsqueda de texto y observabilidad avanzada por estar fuera del alcance solicitado.
- Con más tiempo añadiría autenticación (por ejemplo, JWT/OIDC), `rowversion` para evitar actualizaciones perdidas, pruebas de integración HTTP con SQL Server en contenedor, health checks, métricas y un pipeline CI/CD.

## Uso de IA

La IA se utilizó como apoyo de diseño y revisión:

- Se le pidió proponer una API REST en capas con controllers, services, repositories, validators y Unit of Work, además de migraciones y tests.
- Se aceptó la estructura general, el uso de DTOs, EF Core, FluentValidation, Problem Details y SQLite para tests.
- Se corrigieron manualmente detalles durante la implementación: el conflicto entre el enum de dominio `TaskStatus` y el tipo del framework, la construcción correcta de `ValidationProblemDetails`, la configuración local de NuGet y la generación de la migración EF Core.
