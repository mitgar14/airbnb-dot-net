# 📮 Guía de Postman - Airbnb .NET

> Documentación completa para usar la colección de Postman con los microservicios de Airbnb .NET.

## 📑 Tabla de Contenidos

- [Introducción](#-introducción)
- [Requisitos Previos](#-requisitos-previos)
- [Importar la Colección](#-importar-la-colección)
- [Configurar Environment](#-configurar-environment)
- [Estructura de la Colección](#-estructura-de-la-colección)
- [Gestión Automática de Tokens](#-gestión-automática-de-tokens)
- [Scripts Pre-request y Tests](#-scripts-pre-request-y-tests)
- [Troubleshooting](#-troubleshooting)
- [Tips y Mejores Prácticas](#-tips-y-mejores-prácticas)

## 📖 Introducción

Esta colección de Postman contiene **19 endpoints** organizados en 3 carpetas, correspondientes a los 3 microservicios del proyecto:

| Microservicio | Puerto | Endpoints | Descripción |
|---------------|--------|-----------|-------------|
| **Usuarios API** | 5001 | 6 | Gestión de usuarios y autenticación |
| **Airbnbs API** | 5002 | 7 | Gestión de alojamientos |
| **Reservas API** | 5003 | 6 | Gestión de reservas |

### ✨ Características Principales

- ✅ **Gestión automática de JWT tokens**: El token se captura y guarda automáticamente al hacer login
- ✅ **Tests automáticos**: Validación de status codes, tiempos de respuesta y formato de datos
- ✅ **Variables de entorno**: URLs y puertos configurables fácilmente
- ✅ **Bearer Token automático**: Se aplica automáticamente a endpoints protegidos
- ✅ **Scripts de debugging**: Información útil en la consola de Postman

## ✅ Requisitos Previos

### Software Necesario

1. **Postman Desktop** (Recomendado)
   - [Descargar Postman](https://www.postman.com/downloads/)
   - Versión mínima: 10.0+

2. **Microservicios corriendo**
   ```powershell
   # Iniciar todos los servicios
   .\scripts\start-all.ps1
   
   # Verificar que estén corriendo
   .\scripts\check-status.ps1
   ```

3. **Base de datos MySQL configurada**
   ```powershell
   # Verificar conexión
   mysql -u root -p -e "USE airbnb_app; SHOW TABLES;"
   ```

## 📥 Importar la Colección

### Método 1: Importar Archivos (Recomendado)

1. **Abrir Postman**
2. Click en **"Import"** (botón superior izquierdo)
3. **Arrastrar y soltar** o **seleccionar** los dos archivos:
   - `Airbnb.NET-Microservices.postman_collection.json`
   - `Airbnb.NET-Local.postman_environment.json`
4. Click en **"Import"**

### Método 2: Importar desde URL

Si tienes los archivos en un repositorio:

1. Click en **"Import"** → **"Link"**
2. Pegar la URL del archivo raw de GitHub
3. Click en **"Continue"** → **"Import"**

### Verificar Importación

Después de importar deberías ver:

**Colecciones (Panel izquierdo):**
- ✅ Airbnb .NET - Microservices

**Environments (Dropdown superior derecho):**
- ✅ Airbnb .NET - Local

## ⚙️ Configurar Environment

### 1. Seleccionar el Environment

En la esquina superior derecha, selecciona **"Airbnb .NET - Local"** del dropdown.

### 2. Revisar Variables de Entorno

Click en el ícono del ojo 👁️ para ver las variables:

| Variable | Valor Inicial | Descripción |
|----------|---------------|-------------|
| `base_url` | `http://localhost` | URL base de los servicios |
| `usuarios_port` | `5001` | Puerto de Usuarios.API |
| `airbnbs_port` | `5002` | Puerto de Airbnbs.API |
| `reservas_port` | `5003` | Puerto de Reservas.API |
| `token` | `""` (vacío) | Token JWT general (se llena automáticamente) |
| `token_cliente` | `""` (vacío) | Token JWT del rol Cliente |
| `token_host` | `""` (vacío) | Token JWT del rol Host |
| `token_admin` | `""` (vacío) | Token JWT del rol Admin |

### 3. Personalizar (Opcional)

Si tus microservicios corren en puertos diferentes, edita las variables:

1. Click en **Environments** → **Airbnb .NET - Local**
2. Edita los valores según tu configuración
3. Click en **Save** (Ctrl + S)

**Ejemplo: Cambiar puerto de Usuarios.API a 6001:**

```json
{
  "key": "usuarios_port",
  "value": "6001",
  "type": "default",
  "enabled": true
}
```

## 📂 Estructura de la Colección

```
📦 Airbnb .NET - Microservices
│
├── 📁 Usuarios API (Puerto 5001) - 6 endpoints
│   ├── 🌍 Get All Users
│   ├── 🔍 Get User by ID
│   ├── ✅ Validate User
│   ├── ➕ Create User
│   ├── 🔐 Login User ⭐ (Guarda token automáticamente)
│   └── 🗑️ Delete User
│
├── 📁 Airbnbs API (Puerto 5002) - 7 endpoints
│   ├── 🌍 Get All Airbnbs
│   ├── 🔍 Get Airbnb by ID
│   ├── 👤 Get Airbnbs by Host ID
│   ├── 🏠 Get Airbnbs by Room Type
│   ├── ➕ Create Airbnb
│   ├── ✏️ Update Airbnb
│   └── 🗑️ Delete Airbnb
│
└── 📁 Reservas API (Puerto 5003) - 6 endpoints
    ├── 🌍 Get All Reservas
    ├── 👤 Get Reservas by User ID 🔒
    ├── 🔍 Get Reserva by Reservation ID 🔒
    ├── 🏠 Get Reservas by Host ID 🔒
    ├── ➕ Create Reserva 🔒
    └── 🗑️ Delete Reserva 🔒
```

**Leyenda:**
- 🌍 = Público (no requiere autenticación)
- 🔒 = Protegido (requiere Bearer Token)
- ⭐ = Importante (captura token automáticamente)

## 🔐 Gestión Automática de Tokens

### Cómo Funciona

La colección está configurada para capturar y guardar automáticamente el token JWT cuando haces login.

### Flujo Automático

```
1. Usuario ejecuta: POST /api/usuarios/login
   ↓
2. Script "Tests" captura el token de la respuesta
   ↓
3. Token se guarda en variable de entorno: {{token}}
   ↓
4. Todos los endpoints protegidos usan automáticamente: 
   Authorization: Bearer {{token}}
```

### Script de Captura de Token (Pre-configurado)

En el endpoint **"Login User"**, pestaña **"Tests"**:

```javascript
// Test automático que se ejecuta después del login
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has token", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('token');
});

// Capturar y guardar token automáticamente
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.token) {
        pm.environment.set("token", jsonData.token);
        console.log("✅ Token saved to environment variable");
        console.log("Token:", jsonData.token.substring(0, 20) + "...");
    }
}

// Información de debugging
console.log("=== Login Response ===");
console.log("Status:", pm.response.code);
console.log("User ID:", pm.response.json().user?.userId);
console.log("User Role:", pm.response.json().user?.role);
```

### Usar el Token en Otros Endpoints

**Automático (Ya configurado):**

Los endpoints protegidos ya tienen configurado en la pestaña **"Authorization"**:

- Type: `Bearer Token`
- Token: `{{token}}`

**Manual (Si necesitas configurar un nuevo endpoint):**

1. En la pestaña **"Authorization"** del request
2. Type: **Bearer Token**
3. Token: `{{token}}`

### Ver el Token Actual

```javascript
// En la consola de Postman (Ver → Show Postman Console)
console.log(pm.environment.get("token"));
```

## 🧪 Flujos de Testing

### Flujo 1: Testing Básico (Sin Autenticación)

Endpoints públicos que puedes probar sin login:

```
1. GET All Users
   → http://localhost:5001/api/usuarios

2. GET All Airbnbs
   → http://localhost:5002/api/airbnbs

3. GET Airbnbs by Room Type
   → http://localhost:5002/api/airbnbs/roomType/Entire%20home/apt
```

### Flujo 2: Crear Usuario y Login ⭐

**Paso 1: Crear un nuevo usuario**

```http
POST http://localhost:5001/api/usuarios
Content-Type: application/json

{
  "userId": 100,
  "name": "Test User",
  "email": "test@example.com",
  "password": "Password123!",
  "role": "Cliente"
}
```

**Respuesta esperada:**
```json
{
  "userId": 100,
  "name": "Test User",
  "email": "test@example.com",
  "role": "Cliente"
}
```

**Paso 2: Hacer Login**

```http
POST http://localhost:5001/api/usuarios/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Password123!"
}
```

**Respuesta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "userId": 100,
    "name": "Test User",
    "email": "test@example.com",
    "role": "Cliente"
  }
}
```

✅ El token se guarda automáticamente en `{{token}}`

### Flujo 3: Crear Reserva Completa (Requiere Autenticación)

**Pre-requisito:** Haber hecho login (Flujo 2)

**Paso 1: Buscar un Airbnb**

```http
GET http://localhost:5002/api/airbnbs
```

Copia el `id` de un airbnb de la respuesta (ej: "2595")

**Paso 2: Crear la Reserva**

```http
POST http://localhost:5003/api/reservas
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "userId": "100",
  "airbnbId": "2595",
  "checkIn": "2025-12-20",
  "checkOut": "2025-12-25"
}
```

**Paso 3: Ver tus Reservas**

```http
GET http://localhost:5003/api/reservas/userID/100
Authorization: Bearer {{token}}
```

### Flujo 4: CRUD Completo de Airbnb

**1. Crear Airbnb**

```http
POST http://localhost:5002/api/airbnbs
Content-Type: application/json

{
  "id": "99999",
  "name": "Mi Nueva Propiedad",
  "hostId": "2",
  "hostName": "Host Test",
  "neighbourhoodGroup": "Manhattan",
  "neighbourhood": "Upper West Side",
  "latitude": "40.7812",
  "longitude": "-73.9665",
  "roomType": "Entire home/apt",
  "price": "200",
  "minimumNights": "2",
  "numberOfReviews": "0",
  "rating": "0",
  "rooms": "3",
  "beds": "2",
  "bathrooms": "2"
}
```

**2. Leer Airbnb**

```http
GET http://localhost:5002/api/airbnbs/id/99999
```

**3. Actualizar Airbnb**

```http
PUT http://localhost:5002/api/airbnbs/99999
Content-Type: application/json

{
  "name": "Mi Propiedad Actualizada",
  "price": "250"
}
```

**4. Eliminar Airbnb**

```http
DELETE http://localhost:5002/api/airbnbs/99999
```

## 🔬 Scripts Pre-request y Tests

### Pre-request Scripts

Se ejecutan **antes** de enviar el request. Útiles para:
- Preparar datos
- Calcular valores dinámicos
- Logs de debugging

**Ejemplo en "Create User":**

```javascript
// Pre-request Script
console.log("=== Creating New User ===");
console.log("Endpoint:", pm.request.url.toString());
console.log("Method:", pm.request.method);

// Generar email único
var timestamp = new Date().getTime();
pm.environment.set("unique_email", "user" + timestamp + "@test.com");
```

### Test Scripts

Se ejecutan **después** de recibir la respuesta. Útiles para:
- Validar respuestas
- Extraer datos
- Tests automatizados

**Ejemplo de Tests Comunes:**

```javascript
// Test 1: Verificar status code
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Test 2: Verificar tiempo de respuesta
pm.test("Response time is less than 500ms", function () {
    pm.expect(pm.response.responseTime).to.be.below(500);
});

// Test 3: Verificar estructura de respuesta
pm.test("Response has required fields", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('userId');
    pm.expect(jsonData).to.have.property('name');
    pm.expect(jsonData).to.have.property('email');
});

// Test 4: Verificar tipo de contenido
pm.test("Content-Type is JSON", function () {
    pm.response.to.have.header("Content-Type");
    pm.expect(pm.response.headers.get("Content-Type")).to.include("application/json");
});

// Test 5: Validar datos específicos
pm.test("User role is valid", function () {
    var jsonData = pm.response.json();
    var validRoles = ["Admin", "Host", "Cliente"];
    pm.expect(validRoles).to.include(jsonData.role);
});
```

### Ver Resultados de Tests

Después de ejecutar un request:

1. Pestaña **"Test Results"** (abajo)
2. Verás: ✅ Tests passed / ❌ Tests failed
3. Click en cada test para ver detalles

### Consola de Postman

Para ver los `console.log()`:

1. **View** → **Show Postman Console** (o `Ctrl + Alt + C`)
2. Ejecuta un request
3. Verás todos los logs en tiempo real

## 🧰 Troubleshooting

### Problema 1: "Could not send request"

**Causa:** Los microservicios no están corriendo.

**Solución:**
```powershell
# Verificar servicios
.\scripts\check-status.ps1

# Iniciar servicios
.\scripts\start-all.ps1

# Esperar 10 segundos y probar nuevamente
```

### Problema 2: "401 Unauthorized"

**Causa:** Token JWT inválido o expirado.

**Solución:**
1. Ejecutar **"Login User"** nuevamente
2. Verificar que el token se guardó:
   - Click en ícono del ojo 👁️
   - Buscar variable `token`
   - Debe tener un valor largo

```javascript
// En Console de Postman:
console.log("Current token:", pm.environment.get("token"));
```

### Problema 3: "404 Not Found"

**Causa:** URL incorrecta o puerto equivocado.

**Solución:**

1. Verificar que el environment está seleccionado
2. Revisar las variables de puerto:
   ```
   usuarios_port = 5001
   airbnbs_port = 5002
   reservas_port = 5003
   ```
3. Verificar en navegador:
   - http://localhost:5001/swagger
   - http://localhost:5002/swagger
   - http://localhost:5003/swagger

### Problema 4: "500 Internal Server Error"

**Causa:** Error en el servidor (base de datos, validación, etc.)

**Solución:**

1. **Revisar logs del microservicio** en la consola donde se ejecuta
2. **Verificar base de datos:**
   ```powershell
   mysql -u root -p -e "USE airbnb_app; SHOW TABLES;"
   ```
3. **Verificar el body del request:**
   - Formato JSON correcto
   - Campos requeridos presentes
   - Tipos de datos correctos

### Problema 5: Token no se guarda automáticamente

**Causa:** Script de "Tests" no está configurado o falló.

**Solución:**

1. Ir al endpoint **"Login User"**
2. Pestaña **"Tests"**
3. Verificar que existe el script de captura de token
4. Abrir **Postman Console** antes de ejecutar
5. Ejecutar login y buscar mensaje: `"✅ Token saved to environment variable"`

Si no aparece el mensaje:

```javascript
// Copiar y pegar este script en la pestaña Tests:
if (pm.response.code === 200) {
    var jsonData = pm.response.json();
    if (jsonData.token) {
        pm.environment.set("token", jsonData.token);
        console.log("✅ Token saved to environment variable");
    }
}
```

### Problema 6: "Error: write EPROTO"

**Causa:** Problema de SSL/TLS (si usas HTTPS).

**Solución:**

1. **File** → **Settings** → **General**
2. Desactivar **"SSL certificate verification"**
3. ⚠️ Solo para desarrollo local

## 💡 Tips y Mejores Prácticas

### 1. Usar Variables para Datos Dinámicos

En lugar de hardcodear IDs, usa variables:

```javascript
// Pre-request Script
pm.environment.set("userId", "100");

// En el request URL:
http://localhost:5001/api/usuarios/{{userId}}
```

### 2. Organizar Requests con Folders

Crea subcarpetas para diferentes escenarios:

```
Airbnb .NET - Microservices
├── 🧪 Testing Flows
│   ├── Happy Path
│   └── Error Cases
└── 📁 Usuarios API
    └── ...
```

### 3. Usar Runner para Pruebas Automatizadas

**Collection Runner** permite ejecutar múltiples requests en secuencia:

1. Click derecho en la colección → **Run collection**
2. Selecciona los requests a ejecutar
3. Click **Run Airbnb .NET - Microservices**
4. Verás un dashboard con todos los tests


### 4. Documentar Requests

Agrega descripciones a cada request:

1. Click en el request
2. Pestaña **"Description"**
3. Usa Markdown para formatear:

```markdown
## Get User by ID

Obtiene los detalles de un usuario específico.

### Parámetros
- `id` (path): ID del usuario

### Respuesta
200 OK: Usuario encontrado
404 Not Found: Usuario no existe

### Ejemplo
\```
GET http://localhost:5001/api/usuarios/1
\```
```

### 5. Crear Multiple Environments

Para diferentes entornos (dev, staging, prod):

```json
// Airbnb .NET - Development
{
  "base_url": "http://localhost",
  "usuarios_port": "5001"
}

// Airbnb .NET - Production
{
  "base_url": "https://api.airbnb.com",
  "usuarios_port": "443"
}
```

## 📊 Resumen de Endpoints

### Usuarios API (5001)

| Método | Endpoint | Auth | Descripción |
|--------|----------|------|-------------|
| GET | `/api/usuarios` | No | Listar usuarios |
| GET | `/api/usuarios/{id}` | No | Obtener usuario |
| GET | `/api/usuarios/validation` | No | Validar credenciales |
| POST | `/api/usuarios` | No | Crear usuario |
| POST | `/api/usuarios/login` | No | Login (obtener token) |
| DELETE | `/api/usuarios/{id}` | No | Eliminar usuario |

### Airbnbs API (5002)

| Método | Endpoint | Auth | Descripción |
|--------|----------|------|-------------|
| GET | `/api/airbnbs` | No | Listar airbnbs |
| GET | `/api/airbnbs/id/{id}` | No | Obtener airbnb |
| GET | `/api/airbnbs/hostId/{id}` | No | Airbnbs por host |
| GET | `/api/airbnbs/roomType/{type}` | No | Airbnbs por tipo |
| POST | `/api/airbnbs` | No | Crear airbnb |
| PUT | `/api/airbnbs/{id}` | No | Actualizar airbnb |
| DELETE | `/api/airbnbs/{id}` | No | Eliminar airbnb |

### Reservas API (5003)

| Método | Endpoint | Auth | Descripción |
|--------|----------|------|-------------|
| GET | `/api/reservas` | No | Listar reservas |
| GET | `/api/reservas/userID/{id}` | Sí 🔒 | Reservas de usuario |
| GET | `/api/reservas/reservationID/{id}` | Sí 🔒 | Obtener reserva |
| GET | `/api/reservas/hostID/{id}` | Sí 🔒 | Reservas de host |
| POST | `/api/reservas` | Sí 🔒 | Crear reserva |
| DELETE | `/api/reservas/reservationID/{id}` | Sí 🔒 | Eliminar reserva |