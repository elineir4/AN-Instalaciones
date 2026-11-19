# AN Instalaciones — Sistema de Gestión (Sanitarias y Gas)

Aplicación web para la gestión integral de una empresa de instalaciones sanitarias y de gas, desarrollada con **.NET 10**, **Blazor Web App** (Interactive Server + WebAssembly) y **Entity Framework Core**.

---

## Funcionalidades principales

- **Inicio**: Panel principal de la aplicación.
- **Clientes**: Alta y administración de clientes.
- **Saldos Pendientes**: Control de saldos, resúmenes y registro de pagos.
- **Trabajos y Planos**: Gestión de obras/trabajos, carga y visualización de planos.
- **Materiales**: Registro y administración de materiales.
- **Empleados**: Gestión del personal de obra y datos de contacto.
- **Pago Empleados**: Registro y seguimiento de pagos al personal.
- **Reservas**: Carga de reservas y conversión directa a trabajos.
- **Acceso protegido**: Inicio de sesión mediante autenticación por cookies (`/login` y `/logout`).

---

## Estructura de la solución

El repositorio está organizado en los siguientes proyectos (`Proyecto2026.slnx`):

- **`Proyecto2026.Server`**: Servidor ASP.NET Core, configuración principal, autenticación por cookies y controladores API REST (`ClienteController`, `EmpleadoController`, `MaterialController`, `PagoController`, `PagoEmpleadoController`, `ReservaController`, `TrabajoController`).
- **`Proyecto2026.Server.Client`**: Interfaz de usuario en Blazor (páginas, componentes y layout).
- **`Proyecto2026.BD`**: Contexto de Entity Framework Core (`AppDbContext`) y entidades de base de datos.
- **`Proyecto2026.Repositorio`**: Capa de repositorios para acceso a datos.
- **`Proyecto2026.Servicios`**: Servicios HTTP para comunicación entre cliente y servidor.
- **`Proyecto2026.Shared`**: Modelos y DTOs compartidos.

---

## Tecnologías utilizadas

- [.NET 10](https://dotnet.microsoft.com/)
- Blazor Web App (Interactive Server & WebAssembly)
- ASP.NET Core Web API & Razor Pages
- Entity Framework Core 10 (SQL Server)
- Swagger / Swashbuckle (habilitado en entorno de desarrollo)

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server o **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`)

---

## Instalación y ejecución local

1. **Clonar el repositorio**

   ```bash
   git clone <url-del-repositorio>
   cd Proyecto2026
   ```

2. **Configurar la base de datos**

   Por defecto, `Proyecto2026.Server/Proyecto2026.Server/appsettings.json` apunta a LocalDB:

   ```json
   "ConnectionStrings": {
     "ConnSqlServer": "Server=(localdb)\\MSSQLLocalDB;Database=Proyecto2026;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

   Aplicar las migraciones de Entity Framework Core:

   ```bash
   dotnet ef database update --project Proyecto2026.BD --startup-project Proyecto2026.Server/Proyecto2026.Server
   ```

3. **Configurar las credenciales de inicio de sesión**

   El login lee `Auth:Usuario` y `Auth:Password` desde la configuración (`user-secrets` en desarrollo):

   ```bash
   dotnet user-secrets set "Auth:Usuario" "tu_usuario" --project Proyecto2026.Server/Proyecto2026.Server
   dotnet user-secrets set "Auth:Password" "tu_contraseña" --project Proyecto2026.Server/Proyecto2026.Server
   ```

4. **Ejecutar la aplicación**

   ```bash
   dotnet run --project Proyecto2026.Server/Proyecto2026.Server
   ```
