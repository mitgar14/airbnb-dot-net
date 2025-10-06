# Airbnb Microservices - .NET Migration

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)

> 🚀 Migración completa de una aplicación de gestión de Airbnb de microservicios Node.js/Express a una arquitectura .NET moderna utilizando ASP.NET Core 8.0.

---

## 📋 Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Características Principales](#características-principales)
- [Stack Tecnológico](#stack-tecnológico)
- [Arquitectura](#arquitectura)
- [Inicio Rápido](#inicio-rápido)
  - [Requisitos Previos](#requisitos-previos)
  - [Instalación](#instalación)
  - [Configuración](#configuración)
  - [Ejecución](#ejecución)
- [Endpoints de las APIs](#endpoints-de-las-apis)
- [Testing](#testing)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Troubleshooting](#troubleshooting)
- [Documentación Adicional](#documentación-adicional)

## 📖 Descripción General

Este proyecto es una **migración completa** de una aplicación de microservicios de Airbnb de Node.js a **.NET 8.0**. La aplicación consiste en tres microservicios independientes que se comunican entre sí para gestionar usuarios, alojamientos (airbnbs) y reservas.

### ¿Por qué esta migración?

- ✅ **Tipado fuerte**: Reducción de errores en tiempo de ejecución
- ✅ **Performance**: Mayor rendimiento con código compilado
- ✅ **Escalabilidad**: Mejor manejo de recursos y concurrencia
- ✅ **Ecosistema robusto**: Entity Framework Core, AutoMapper, FluentValidation
- ✅ **Mantenibilidad**: Código más estructurado y fácil de mantener

## ✨ Características Principales

- 🏗️ **Arquitectura de Microservicios**: 3 servicios independientes y escalables
- 🔐 **Autenticación JWT**: Sistema de autenticación seguro con tokens
- 🛡️ **Autorización basada en roles**: Admin, Host y Cliente
- 🔄 **Resiliencia con Polly**: Retry policies y Circuit Breaker
- 💾 **Entity Framework Core**: ORM moderno con MySQL
- 📝 **Validación con FluentValidation**: Validaciones robustas y declarativas
- 🗺️ **AutoMapper**: Mapeo automático entre entidades y DTOs
- 📚 **Swagger/OpenAPI**: Documentación interactiva de APIs
- 🏥 **Health Checks**: Monitoreo de salud de servicios
- 🧪 **Testing con Postman**: Colección completa con 19 endpoints

## 🛠️ Stack Tecnológico

### Backend
- **.NET 8.0** (LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0**
- **MySQL** (Pomelo.EntityFrameworkCore.MySql)

### Librerías y Frameworks
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validación de datos
- **BCrypt.Net-Next** - Hash de contraseñas
- **Swagger/OpenAPI** - Documentación de APIs

### Base de Datos
- **MySQL 8.0**

## 🏗️ Arquitectura

### Microservicios

1. **Usuarios.API** (Puerto 5001)
   - Gestión de usuarios
   - Autenticación y validación
   - CRUD de usuarios

2. **Airbnbs.API** (Puerto 5002)
   - Gestión de alojamientos
   - Búsqueda por host y tipo de habitación
   - CRUD de airbnbs

3. **Reservas.API** (Puerto 5003)
   - Gestión de reservas
   - Comunicación con Usuarios y Airbnbs APIs
   - CRUD de reservas

### Patrones Implementados
- **Repository Pattern** - Abstracción de acceso a datos
- **Service Layer** - Lógica de negocio
- **Dependency Injection** - Inversión de control
- **DTO Pattern** - Transferencia de datos

---

## 🚀 Inicio Rápido

### ✅ Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/mysql/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- [PowerShell](https://docs.microsoft.com/powershell/) (para scripts de ejecución)

## 📦 Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/mitgar14/airbnb-dot-net.git
cd airbnb-dot-net
```

### 2. Restaurar Paquetes NuGet

```bash
dotnet restore
```

### 3. Configurar MySQL

#### Opción A: Configuración Predeterminada

Asegúrate de que MySQL esté corriendo en `localhost:3306` con las siguientes credenciales:

```yaml
Host: localhost
Puerto: 3306
Base de datos: airbnb_app
Usuario: root
Contraseña: password
```

#### Opción B: Configuración Personalizada

Si usas credenciales diferentes, edita los archivos `appsettings.json` de cada microservicio:

**Ubicaciones:**
- `src/Usuarios.API/appsettings.json`
- `src/Airbnbs.API/appsettings.json`
- `src/Reservas.API/appsettings.json`

**Modifica la cadena de conexión:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_HOST;Port=TU_PUERTO;Database=airbnb_app;User=TU_USUARIO;Password=TU_CONTRASEÑA;"
  }
}
```

### 4. Crear la Base de Datos

#### Método 1: Usando el script SQL (Recomendado)

1. Abre **MySQL Workbench** o la consola de MySQL
2. Ejecuta el script ubicado en `db/init.sql`:

```bash
# Desde la consola MySQL
mysql -u root -p < db/init.sql

# O desde PowerShell
Get-Content db/init.sql | mysql -u root -p
```

Este script creará automáticamente:
- ✅ Base de datos `airbnb_app`
- ✅ Tablas: `micro_users`, `micro_airbnbs`, `micro_reservas`
- ✅ Índices optimizados

#### Método 2: Usando Entity Framework Core (Alternativo)

Si prefieres usar migraciones de EF Core:

```bash
cd src/Usuarios.API
dotnet ef database update

cd ../Airbnbs.API
dotnet ef database update

cd ../Reservas.API
dotnet ef database update
```

## ⚙️ Configuración

### Cadenas de Conexión

Puedes modificar las cadenas de conexión en los archivos `appsettings.json` de cada microservicio:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=airbnb_app;User=root;Password=password;"
  }
}
```

### Puertos de los Microservicios

- **Usuarios.API**: `http://localhost:5001`
- **Airbnbs.API**: `http://localhost:5002`
- **Reservas.API**: `http://localhost:5003`

Los puertos se configuran en `Properties/launchSettings.json` de cada proyecto.

## 🚀 Ejecución

### Opción 1: Script PowerShell (⭐ Recomendado)

La forma más rápida de iniciar todos los microservicios:

```powershell
# Iniciar todos los servicios
.\scripts\start-all.ps1

# Verificar que todos estén corriendo
.\scripts\check-status.ps1

# Detener todos los servicios
.\scripts\stop-all.ps1
```

**Lo que hace el script:**
- ✅ Inicia los 3 microservicios en terminales separadas
- ✅ Espera 3 segundos entre cada inicio para evitar conflictos
- ✅ Muestra las URLs de cada servicio

### Opción 2: Ejecución Manual

Abre tres terminales y ejecuta en cada una:

**Terminal 1 - Usuarios.API:**
```bash
cd src/Usuarios.API
dotnet run
```

**Terminal 2 - Airbnbs.API:**
```bash
cd src/Airbnbs.API
dotnet run
```

**Terminal 3 - Reservas.API:**
```bash
cd src/Reservas.API
dotnet run
```

### Opción 3: Visual Studio

1. Click derecho en la solución `AirbnbDotNet.sln`
2. Seleccionar "Configure Startup Projects"
3. Seleccionar "Multiple startup projects"
4. Configurar Action = "Start" para cada API
5. Presionar F5

## 📡 Endpoints de las APIs

### Usuarios.API (`http://localhost:5001`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/usuarios` | Obtener todos los usuarios |
| GET | `/api/usuarios/{id}` | Obtener usuario por ID |
| GET | `/api/usuarios/validation?email=&password=` | Validar credenciales |
| POST | `/api/usuarios` | Crear nuevo usuario |
| DELETE | `/api/usuarios/{id}` | Eliminar usuario |

**Swagger UI**: http://localhost:5001/swagger

### Airbnbs.API (`http://localhost:5002`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/airbnbs` | Obtener todos los airbnbs |
| GET | `/api/airbnbs/id/{id}` | Obtener airbnb por ID |
| GET | `/api/airbnbs/hostId/{hostId}` | Obtener airbnbs por host ID |
| GET | `/api/airbnbs/roomType/{roomType}` | Obtener airbnbs por tipo de habitación |
| POST | `/api/airbnbs` | Crear nuevo airbnb |
| PUT | `/api/airbnbs/{id}` | Actualizar airbnb |
| DELETE | `/api/airbnbs/{id}` | Eliminar airbnb |

**Swagger UI**: http://localhost:5002/swagger

### Reservas.API (`http://localhost:5003`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/reservas` | Obtener todas las reservas |
| GET | `/api/reservas/userID/{userId}` | Obtener reservas por user ID |
| GET | `/api/reservas/reservationID/{reservationId}` | Obtener reserva por ID |
| GET | `/api/reservas/hostID/{hostId}` | Obtener reservas por host ID |
| POST | `/api/reservas` | Crear nueva reserva |
| DELETE | `/api/reservas/reservationID/{reservationId}` | Eliminar reserva |

**Swagger UI**: http://localhost:5003/swagger

---

## 🔧 Troubleshooting

### Problema: "Connection refused" o no se puede conectar a MySQL

**Solución:**
```powershell
# 1. Verificar que MySQL esté corriendo
Get-Service MySQL*

# 2. Si no está corriendo, iniciarlo
Start-Service MySQL80  # O el nombre de tu servicio MySQL

# 3. Verificar la conexión
mysql -u root -p -e "SELECT 1;"
```

### Problema: "Port 5001/5002/5003 already in use"

**Solución:**
```powershell
# Ver qué proceso está usando el puerto
netstat -ano | findstr :5001

# Matar el proceso (reemplaza PID con el número obtenido)
taskkill /PID <PID> /F
```

O cambia el puerto en `Properties/launchSettings.json` de cada microservicio.

### Problema: "Table 'airbnb_app.micro_users' doesn't exist"

**Solución:**
```bash
# Ejecutar el script de inicialización de la base de datos
mysql -u root -p < db/init.sql
```

### Problema: Error 401 Unauthorized en endpoints protegidos

**Solución:**
1. Asegúrate de haber hecho login en `POST /api/usuarios/login`
2. Copia el token recibido
3. En Swagger, haz clic en el botón **"Authorize"** (🔒)
4. Ingresa: `Bearer TU_TOKEN_AQUI`
5. En Postman, usa la colección provista que maneja el token automáticamente

### Problema: "FluentValidation" o errores de paquetes NuGet

**Solución:**
```bash
# Limpiar y restaurar paquetes
dotnet clean
dotnet restore
dotnet build
```

### Problema: Microservicios no se comunican (error en Reservas.API)

**Solución:**
1. Verifica que **todos** los microservicios estén corriendo
2. Revisa los logs en las consolas de cada servicio
3. Verifica las URLs en `appsettings.json` de Reservas.API:
```json
{
  "MicroservicesUrls": {
    "UsuariosApi": "http://localhost:5001",
    "AirbnbsApi": "http://localhost:5002"
  }
}
```

### Problema: Errores de Entity Framework Core

**Solución:**
```bash
# Reinstalar herramientas de EF Core
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

## 📚 Documentación Adicional

| Documento | Descripción |
|-----------|-------------|
| [POSTMAN.md](postman/POSTMAN.md) | Guía de uso de la colección de Postman |

---

## 📂 Estructura del Proyecto

```
AirbnbDotNet/
├── src/
│   ├── Usuarios.API/
│   │   ├── Controllers/         # Controladores API
│   │   ├── Data/                # DbContext y entidades
│   │   │   └── Entities/
│   │   ├── DTOs/                # Data Transfer Objects
│   │   ├── Services/            # Lógica de negocio
│   │   ├── Repositories/        # Acceso a datos
│   │   ├── Validators/          # Validaciones FluentValidation
│   │   ├── Profiles/            # Perfiles AutoMapper
│   │   ├── Program.cs           # Configuración de la aplicación
│   │   └── appsettings.json     # Configuración
│   │
│   ├── Airbnbs.API/
│   │   └── [estructura similar]
│   │
│   └── Reservas.API/
│       └── [estructura similar]
│
├── scripts/
│   ├── start-all.ps1           # Iniciar todos los servicios
│   ├── stop-all.ps1            # Detener todos los servicios
│   └── check-status.ps1        # Verificar estado
│
├── db/
│   └── init.sql                # Script de inicialización de BD
│
├── postman/
│   ├── Airbnb.NET-Microservices.postman_collection.json  # Colección Postman (19 endpoints)
│   ├── Airbnb.NET-Local.postman_environment.json         # Variables de entorno
│   └── POSTMAN.md                                        # Guía de uso de Postman
│
├── AirbnbDotNet.sln            # Solución de Visual Studio
└── README.md                   # Este archivo
```

## 🧪 Testing

### Opción 1: Probar con Swagger UI

Cada microservicio tiene una interfaz Swagger interactiva para probar los endpoints:

| Servicio | URL Swagger | Health Check |
|----------|-------------|--------------|
| 🔐 Usuarios | http://localhost:5001/swagger | http://localhost:5001/health |
| 🏠 Airbnbs | http://localhost:5002/swagger | http://localhost:5002/health |
| 📅 Reservas | http://localhost:5003/swagger | http://localhost:5003/health |

**Cómo usar Swagger:**
1. Navega a la URL de Swagger del servicio
2. Expande el endpoint que quieres probar
3. Click en **"Try it out"**
4. Ingresa los parámetros requeridos
5. Click en **"Execute"**
6. Revisa la respuesta en la sección "Responses"

**Autenticación en Swagger:**
1. Ejecuta `POST /api/usuarios/login` para obtener un token
2. Click en el botón **"Authorize"** (🔒) en la parte superior
3. Ingresa: `Bearer TU_TOKEN_AQUI`
4. Ahora puedes ejecutar endpoints protegidos

### Opción 2: Probar con Postman (⭐ Recomendado)

Hemos creado una colección completa de Postman con **19 endpoints** y gestión automática de tokens JWT.

#### Importar la Colección

1. Abre Postman
2. Click en **Import**
3. Selecciona los archivos de la carpeta `postman/`:
   - `Airbnb.NET-Microservices.postman_collection.json`
   - `Airbnb.NET-Local.postman_environment.json`
4. Activa el environment "Airbnb .NET - Local"

#### Características de la Colección

- ✅ **19 endpoints** organizados en 3 carpetas
- ✅ **Gestión automática de token JWT**: El token se captura automáticamente al hacer login
- ✅ **Tests automáticos**: Validación de status codes, tiempos de respuesta y formatos
- ✅ **Variables de entorno**: URLs y puertos configurables
- ✅ **Bearer Token automático**: Se aplica automáticamente a endpoints protegidos
- ✅ **Scripts de debugging**: Información de configuración en consola

#### Flujo de Prueba Recomendado

Sigue este orden para probar todas las funcionalidades:

**1. Crear un Usuario**
```http
POST http://localhost:5001/api/usuarios
Content-Type: application/json

{
  "userId": 1,
  "name": "Juan Pérez",
  "email": "juan@example.com",
  "password": "Password123!",
  "role": "Cliente"
}
```

**2. Hacer Login** (Token se guarda automáticamente en Postman) ⭐
```http
POST http://localhost:5001/api/usuarios/login
Content-Type: application/json

{
  "email": "juan@example.com",
  "password": "Password123!"
}
```

**3. Buscar Propiedades** (No requiere autenticación)
```http
GET http://localhost:5002/api/airbnbs
```

**4. Crear una Reserva** (Usa el token automáticamente) 🔐
```http
POST http://localhost:5003/api/reservas
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": "1",
  "airbnbId": "2595",
  "checkIn": "2025-12-20",
  "checkOut": "2025-12-25"
}
```

**5. Ver tus Reservas** (Usa el token automáticamente) 🔐
```http
GET http://localhost:5003/api/reservas/userID/1
Authorization: Bearer {{token}}
```

> 📖 Para más detalles sobre la colección de Postman, consulta [POSTMAN.md](postman/POSTMAN.md).

## 🔐 Sistema de Autorización

### Roles y Permisos

#### Admin
- Acceso total al sistema
- Puede realizar todas las operaciones sin restricciones

#### Host
- Crear, editar y eliminar **sus propias propiedades**
- Ver reservas de **sus propiedades**
- No puede modificar propiedades de otros hosts

#### Cliente
- Crear reservas **para sí mismo**
- Ver y cancelar **sus propias reservas**
- Buscar propiedades disponibles (sin autenticación)

### Endpoints Públicos (Sin autenticación)
- `POST /api/usuarios` - Registro de usuarios
- `POST /api/usuarios/login` - Login
- `GET /api/airbnbs/**` - Búsqueda de propiedades

## 🔄 Diferencias con la Versión Node.js

| Aspecto | Node.js | .NET 8.0 |
|---------|---------|----------|
| **Tipado** | JavaScript (dinámico) | C# (estático fuerte) |
| **ORM** | Modelos personalizados | Entity Framework Core |
| **Validación** | Manual | FluentValidation |
| **Inyección de Dependencias** | Manual | Nativo |
| **Async/Await** | Nativo | Task-based (async/await) |
| **Puerto** | 3001, 3002, 3003 | 5001, 5002, 5003 |
| **Documentación** | Manual | Swagger automático |
| **Performance** | Alto (V8) | Muy Alto (compilado) |

## 📝 Notas de Desarrollo

### Entity Framework Core

Para crear migraciones:
```bash
cd src/Usuarios.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Configuración de Desarrollo

Los servicios están configurados para modo desarrollo con:
- Hot reload habilitado
- Logs detallados
- Swagger UI habilitado
- CORS permisivo


## 📄 Licencia

Este proyecto está bajo la licencia **MIT**.

## 👤 Autor

**Martín García**
- GitHub: [@mitgar14](https://github.com/mitgar14)