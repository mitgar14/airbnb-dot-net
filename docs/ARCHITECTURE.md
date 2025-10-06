# Arquitectura del Sistema Airbnb .NET

# 🏗️ Arquitectura del Sistema Airbnb .NET

> Documentación técnica completa de la arquitectura de microservicios, patrones de diseño, flujos de comunicación y decisiones arquitectónicas.

---

## 📑 Tabla de Contenidos

- [Visión General](#visión-general)
- [Diagrama de Arquitectura](#diagrama-de-arquitectura)
- [Componentes del Sistema](#componentes-del-sistema)
- [Diagramas de Secuencia](#diagramas-de-secuencia)
- [Patrones de Diseño](#patrones-de-diseño)
- [Seguridad](#seguridad)
- [Resiliencia](#resiliencia)
- [Health Checks](#health-checks)
- [Base de Datos](#base-de-datos)
- [Escalabilidad](#escalabilidad)
- [Comunicación entre Microservicios](#comunicación-entre-microservicios)
- [Mejores Prácticas](#mejores-prácticas)
- [Decisiones Arquitectónicas](#decisiones-arquitectónicas)
- [Evolución Futura](#evolución-futura)

---

## 📐 Visión General

Este proyecto implementa una **arquitectura de microservicios** basada en .NET 8.0 para gestionar un sistema de reservas estilo Airbnb. La arquitectura está diseñada con los siguientes principios:

- 🎯 **Separación de responsabilidades**: Cada microservicio tiene un dominio claro
- 📦 **Independencia de despliegue**: Los servicios se pueden desplegar independientemente
- 🔄 **Escalabilidad horizontal**: Cada servicio puede escalar según demanda
- 🛡️ **Resiliencia**: Fallos aislados no afectan todo el sistema
- 🔐 **Seguridad**: Autenticación y autorización en múltiples capas

## 🏗️ Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                    Clientes / Frontend                           │
│              (Swagger UI, Postman, Apps Web/Mobile)              │
└──────────────────────┬──────────────────────────────────────────┘
                       │
                       │ HTTP/REST (JSON)
                       │
     ┌─────────────────┼─────────────────┐
     │                 │                 │
     ▼                 ▼                 ▼
┌─────────┐      ┌──────────┐     ┌──────────┐
│Usuarios │      │ Airbnbs  │     │ Reservas │
│   API   │      │   API    │     │   API    │
│ :5001   │      │  :5002   │     │  :5003   │
└────┬────┘      └────┬─────┘     └────┬─────┘
     │                │                 │
     │                │                 │ Polly
     │                │                 │ (Retry + Circuit Breaker)
     │                │                 │
     └────────────────┴─────────────────┘
                      │
                      │ MySQL Connection (EF Core)
                      ▼
          ┌───────────────────────┐
          │    MySQL Database     │
          │   localhost:3306      │
          │   airbnb_app          │
          └───────────────────────┘
```

## 🔧 Componentes del Sistema

### 1. Usuarios.API (Puerto 5001)
**Responsabilidades:**
- Gestión de usuarios (CRUD)
- Autenticación con JWT
- Validación de credenciales
- Generación de tokens

**Tecnologías Clave:**
- BCrypt.Net para hashing de contraseñas
- JWT Bearer Authentication
- FluentValidation para validación de entrada

**Base de Datos:**
- Tabla: `micro_users`

### 2. Airbnbs.API (Puerto 5002)
**Responsabilidades:**
- Gestión de alojamientos (CRUD)
- Búsqueda por host
- Filtrado por tipo de habitación
- Gestión de detalles de propiedades

**Tecnologías Clave:**
- Repository Pattern
- AutoMapper para DTOs
- FluentValidation

**Base de Datos:**
- Tabla: `micro_airbnbs`

### 3. Reservas.API (Puerto 5003)
**Responsabilidades:**
- Gestión de reservas (CRUD)
- Coordinación entre usuarios y airbnbs
- Comunicación con otros microservicios
- Resiliencia con Polly

**Tecnologías Clave:**
- HttpClient con IHttpClientFactory
- Polly para resiliencia (Retry + Circuit Breaker)
- Comunicación síncrona REST

**Base de Datos:**
- Tabla: `micro_reservas`

**Integraciones:**
- Llama a Usuarios.API para validar usuarios
- Llama a Airbnbs.API para validar alojamientos

---

## 📊 Diagramas de Secuencia

### Flujo 1: Autenticación de Usuario (Login)

```
┌─────────┐          ┌──────────────┐          ┌──────────┐          ┌─────────┐
│ Cliente │          │UsuariosAPI   │          │Repository│          │ MySQL   │
└────┬────┘          └──────┬───────┘          └────┬─────┘          └────┬────┘
     │                      │                       │                     │
     │ POST /api/usuarios/login                     │                     │
     │ { email, password }  │                       │                     │
     ├─────────────────────>│                       │                     │
     │                      │                       │                     │
     │                      │ ValidateLoginDto()    │                     │
     │                      │ (FluentValidation)    │                     │
     │                      │────────┐              │                     │
     │                      │        │              │                     │
     │                      │<───────┘              │                     │
     │                      │                       │                     │
     │                      │ GetUserByEmail(email) │                     │
     │                      ├──────────────────────>│                     │
     │                      │                       │                     │
     │                      │                       │ SELECT * FROM       │
     │                      │                       │ micro_users WHERE   │
     │                      │                       │ email = ?           │
     │                      │                       ├────────────────────>│
     │                      │                       │                     │
     │                      │                       │  User Entity        │
     │                      │                       │<────────────────────┤
     │                      │                       │                     │
     │                      │  User Entity          │                     │
     │                      │<──────────────────────┤                     │
     │                      │                       │                     │
     │                      │ BCrypt.Verify()       │                     │
     │                      │ (password, hash)      │                     │
     │                      │────────┐              │                     │
     │                      │        │              │                     │
     │                      │<───────┘              │                     │
     │                      │                       │                     │
     │                      │ GenerateJWT()         │                     │
     │                      │────────┐              │                     │
     │                      │        │              │                     │
     │                      │<───────┘              │                     │
     │                      │                       │                     │
     │  200 OK              │                       │                     │
     │  {                   │                       │                     │
     │    "token": "...",   │                       │                     │
     │    "user": {...}     │                       │                     │
     │  }                   │                       │                     │
     │<─────────────────────┤                       │                     │
     │                      │                       │                     │
```

**Pasos del flujo:**
1. El cliente envía credenciales (email + password)
2. FluentValidation valida el formato de los datos
3. Se busca el usuario por email en la base de datos
4. Se verifica la contraseña con BCrypt
5. Si es válido, se genera un JWT token
6. Se retorna el token y datos del usuario

### Flujo 2: Crear una Reserva (Comunicación entre Microservicios)

```
┌─────────┐     ┌───────────┐     ┌──────────┐     ┌─────────┐
│ Cliente │     │ReservasAPI│     │UsuariosAPI│     │AirbnbsAPI│
└────┬────┘     └─────┬─────┘     └─────┬────┘     └────┬─────┘
     │                │                 │                │
     │ POST /api/reservas               │                │
     │ + Bearer Token │                 │                │
     ├───────────────>│                 │                │
     │                │                 │                │
     │                │ ValidateJWT()   │                │
     │                │────────┐        │                │
     │                │        │        │                │
     │                │<───────┘        │                │
     │                │                 │                │
     │                │ ValidateDto()   │                │
     │                │────────┐        │                │
     │                │        │        │                │
     │                │<───────┘        │                │
     │                │                 │                │
     │                │ GET /api/usuarios/{userId}       │
     │                ├────────────────>│                │
     │                │                 │                │
     │                │ [Polly: Retry]  │                │
     │                │ Attempt 1       │                │
     │                │                 │                │
     │                │  200 OK         │                │
     │                │  User Data      │                │
     │                │<────────────────┤                │
     │                │                 │                │
     │                │ GET /api/airbnbs/id/{airbnbId}   │
     │                ├─────────────────────────────────>│
     │                │                 │                │
     │                │ [Polly: Retry]  │                │
     │                │ Attempt 1       │                │
     │                │                 │                │
     │                │         200 OK  │                │
     │                │      Airbnb Data│                │
     │                │<─────────────────────────────────┤
     │                │                 │                │
     │                │ CreateReserva() │                │
     │                │────────┐        │                │
     │                │        │        │                │
     │                │<───────┘        │                │
     │                │                 │                │
     │                │ SaveToDatabase()│                │
     │                │────────┐        │                │
     │                │        │        │                │
     │                │<───────┘        │                │
     │                │                 │                │
     │  201 Created   │                 │                │
     │  Reserva Data  │                 │                │
     │<───────────────┤                 │                │
     │                │                 │                │
```

**Pasos del flujo:**
1. Cliente envía request con JWT token en header
2. Se valida el token JWT
3. Se valida el DTO con FluentValidation
4. **Comunicación con Usuarios.API** para verificar que el usuario existe
5. **Comunicación con Airbnbs.API** para verificar que el airbnb existe
6. Si ambas validaciones pasan, se crea la reserva
7. Se guarda en la base de datos
8. Se retorna la reserva creada

**Resiliencia:**
- Las llamadas a otros microservicios usan **Polly** para reintentos
- Si falla una llamada, se reintenta hasta 3 veces con backoff exponencial
- Si después de 3 intentos falla, se retorna error 503

### Flujo 3: Circuit Breaker en Acción

```
Escenario: Usuarios.API está caído

Intento 1: ───X (fallo) → Retry después de 2s
Intento 2: ───X (fallo) → Retry después de 4s
Intento 3: ───X (fallo) → Retry después de 8s
Intento 4: ───X (fallo)
Intento 5: ───X (fallo)

→ Circuit Breaker ABRE

Durante 30 segundos:
  Todos los requests → Fallo inmediato (503)
  No se hacen llamadas a Usuarios.API

Después de 30s:
  Circuit Breaker → HALF-OPEN
  Se permite 1 request de prueba
  
  Si tiene éxito → CLOSED (normal)
  Si falla → OPEN por otros 30s
```

---

## 🎨 Patrones de Diseño

### 1. Repository Pattern

**Propósito**: Abstraer el acceso a datos y desacoplar la lógica de negocio de la capa de persistencia.

**Implementación:**

```
┌─────────────┐
│ Controller  │ ← Recibe HTTP requests
└──────┬──────┘
       │
       │ calls
       ▼
┌─────────────┐
│  Service    │ ← Contiene lógica de negocio
└──────┬──────┘
       │
       │ calls
       ▼
┌─────────────┐
│ Repository  │ ← Abstracción de acceso a datos
└──────┬──────┘
       │
       │ uses
       ▼
┌─────────────┐
│  DbContext  │ ← Entity Framework Core
└──────┬──────┘
       │
       ▼
   Database
```

**Ventajas:**
- ✅ Testeable: Fácil de mockear en tests unitarios
- ✅ Mantenible: Cambios en la BD no afectan la lógica de negocio
- ✅ Reutilizable: Lógica de acceso a datos centralizada
- ✅ Flexible: Fácil cambiar de ORM o BD

**Ejemplo de uso:**

```csharp
// Interface del Repository
public interface IUsuarioRepository
{
    Task<Usuario> GetByIdAsync(long id);
    Task<Usuario> GetByEmailAsync(string email);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task CreateAsync(Usuario usuario);
    Task DeleteAsync(long id);
}

// Implementación
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Usuario> GetByIdAsync(long id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.UserId == id);
    }
}
```

### 2. Service Layer Pattern

**Propósito**: Encapsular la lógica de negocio y coordinar operaciones entre múltiples repositories.

**Responsabilidades:**
- Validación de reglas de negocio
- Coordinación de transacciones
- Transformación de datos (DTOs ↔ Entidades)
- Llamadas a otros microservicios

**Ejemplo:**

```csharp
public class ReservaService : IReservaService
{
    private readonly IReservaRepository _repository;
    private readonly IMicroserviceClient _client;
    
    public async Task<ReservaDto> CreateReservaAsync(CreateReservaDto dto)
    {
        // 1. Validar que el usuario existe (llamada a Usuarios.API)
        var usuarioExists = await _client.ValidateUsuarioAsync(dto.UserId);
        if (!usuarioExists)
            throw new ValidationException("Usuario no encontrado");
        
        // 2. Validar que el airbnb existe (llamada a Airbnbs.API)
        var airbnbExists = await _client.ValidateAirbnbAsync(dto.AirbnbId);
        if (!airbnbExists)
            throw new ValidationException("Airbnb no encontrado");
        
        // 3. Crear la reserva
        var reserva = _mapper.Map<Reserva>(dto);
        await _repository.CreateAsync(reserva);
        
        // 4. Retornar DTO
        return _mapper.Map<ReservaDto>(reserva);
    }
}
```

### 3. Dependency Injection (DI)

**Propósito**: Invertir el control de dependencias para mejorar testabilidad y desacoplamiento.

**Configuración en Program.cs:**

```csharp
// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAirbnbRepository, AirbnbRepository>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();

// Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAirbnbService, AirbnbService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));
```

**Ventajas:**
- ✅ **Testeable**: Fácil inyectar mocks en tests
- ✅ **Mantenible**: Cambiar implementaciones sin modificar código
- ✅ **Configuración centralizada**: Todo se configura en un lugar
- ✅ **Lifetime management**: Control del ciclo de vida de objetos

### 4. DTO (Data Transfer Object) Pattern

**Propósito**: Controlar qué datos se exponen en las APIs y desacoplar modelos internos de externos.

**Flujo:**

```
Request JSON → DTO → Service → Entity → Repository → Database
                ↓                 ↑
            Validation      AutoMapper
```

**Ventajas:**
- ✅ No se exponen entidades de BD directamente
- ✅ Control fino de la serialización
- ✅ Validación centralizada con FluentValidation
- ✅ Versionado de APIs más fácil

**Ejemplo:**

```csharp
// Entity (modelo interno)
public class Usuario
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }  // Hash BCrypt
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// DTO (modelo externo - no expone password ni timestamps)
public class UsuarioDto
{
    public long UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
```

---

## 🔐 Seguridad

### JWT Authentication
- **Algoritmo**: HS256 (HMAC-SHA256)
- **Expiración**: 1440 minutos (24 horas)
- **Claims incluidos**:
  - `sub`: User ID
  - `email`: Email del usuario
  - `role`: Rol del usuario (Admin, Host, Cliente)
  - `jti`: Token ID único

### Flujo de Autenticación
```
1. Usuario → POST /api/usuarios/login { email, password }
2. Usuarios.API valida credenciales con BCrypt
3. Si válido → Genera JWT token
4. Usuario recibe token + datos del usuario
5. Usuario incluye token en header: Authorization: Bearer {token}
6. Todas las APIs validan el token en cada request
```

## 🏥 Health Checks

Cada microservicio expone 3 endpoints de health:

### 1. `/health`
- **Propósito**: Health check completo
- **Verifica**: 
  - DbContext (EF Core)
  - Conexión MySQL

### 2. `/health/ready`
- **Propósito**: Readiness probe (Kubernetes)
- **Verifica**: Que la API pueda procesar requests
- **Tags**: `ready`, `db`, `mysql`

### 3. `/health/live`
- **Propósito**: Liveness probe (Kubernetes)
- **Verifica**: Que el proceso esté vivo
- **Uso**: Detectar deadlocks o procesos colgados

## 🔄 Resiliencia con Polly

### Retry Policy
- **Intentos**: 3 reintentos
- **Estrategia**: Exponential Backoff
- **Delays**: 2s, 4s, 8s

```csharp
Retry 1 → espera 2 segundos
Retry 2 → espera 4 segundos
Retry 3 → espera 8 segundos
```

### Circuit Breaker
- **Umbral**: 5 fallos consecutivos
- **Duración del break**: 30 segundos
- **Comportamiento**: 
  - Después de 5 fallos → circuito abierto
  - Durante 30s → rechaza requests inmediatamente
  - Después de 30s → intenta una request (half-open)
  - Si falla → vuelve a abrir por 30s
  - Si funciona → circuito cerrado

```
Normal → Falla (1) → Falla (2) → ... → Falla (5) → OPEN
  ↑                                                    ↓
CLOSED ← Success ← Half-Open (intenta 1 request) ← 30s
  ↓                    ↓
Normal            OPEN (si falla)
```

## 📊 Patrón Repository

Todos los microservicios implementan Repository Pattern:

```
Controller → Service → Repository → DbContext → Database
```

**Ventajas:**
- Separación de preocupaciones
- Testabilidad mejorada
- Abstracción de la capa de datos
- Fácil cambio de ORM o base de datos

## 🗄️ Base de Datos

### Estrategia
- **ORM**: Entity Framework Core
- **Provider**: Pomelo.EntityFrameworkCore.MySql
- **Migrations**: Código primero (Code First)

### Timestamps Automáticos
Todas las entidades incluyen:
- `created_at`: Timestamp de creación (UTC)
- `updated_at`: Timestamp de última actualización (UTC)

**Implementación**: Interceptados en `SaveChanges()` del DbContext

## 🚀 Escalabilidad

### Horizontal Scaling
Cada microservicio puede escalar independientemente:
- **Usuarios.API**: Escalar para alta carga de autenticación
- **Airbnbs.API**: Escalar para búsquedas intensivas
- **Reservas.API**: Escalar para picos de reservas

### Stateless
- No se guarda estado en memoria
- JWT permite autenticación sin sesiones del servidor
- Cada request es independiente

## 🔌 Comunicación entre Microservicios

### Protocolo
- **Tipo**: HTTP/REST síncrono
- **Formato**: JSON
- **Cliente**: HttpClient con IHttpClientFactory

### Consideraciones Futuras
- **Mensajería asíncrona**: RabbitMQ o Azure Service Bus
- **gRPC**: Para comunicación interna más rápida
- **API Gateway**: Kong, Ocelot o Azure API Management

## 📈 Monitoreo y Observabilidad

### Health Checks
- Endpoints para verificar el estado del sistema
- Integración con Kubernetes probes
- Monitoreo de conexiones a base de datos

### Logging
- Logging estructurado con .NET logging
- Niveles: Information, Warning, Error
- Output: Consola (desarrollo)

### Futuro
- Serilog para logging avanzado
- Application Insights
- Distributed tracing con OpenTelemetry

## 🛡️ Mejores Prácticas Implementadas

1. **Separation of Concerns**: Capas bien definidas (Controller → Service → Repository)
2. **Dependency Injection**: Inyección nativa de .NET
3. **DTOs**: No se exponen entidades directamente
4. **Validation**: FluentValidation en la capa de entrada
5. **Error Handling**: Try-catch con logging estructurado
6. **CORS**: Configurado para permitir cualquier origen (desarrollo)
7. **Async/Await**: Todas las operaciones I/O son asíncronas
8. **Configuration**: appsettings.json para configuración externa

## 🔮 Evolución Futura

### Corto Plazo
- [ ] Implementar rate limiting
- [ ] Agregar caching (Redis)
- [ ] Mejorar logging con Serilog
- [ ] Tests unitarios e integración

### Mediano Plazo
- [ ] API Gateway (Ocelot)
- [ ] Containerización con Docker
- [ ] Orquestación con Kubernetes
- [ ] CI/CD pipeline

### Largo Plazo
- [ ] Event-driven architecture
- [ ] CQRS pattern para lectura/escritura
- [ ] Migrar a gRPC para comunicación interna
- [ ] Service Mesh (Istio, Linkerd)

## 📝 Decisiones Arquitectónicas

### ¿Por qué Microservicios?
- **Escalabilidad independiente**: Cada servicio escala según necesidad
- **Desarrollo desacoplado**: Equipos pueden trabajar independientemente
- **Tecnologías flexibles**: Posibilidad de usar diferentes stacks
- **Resiliencia**: Fallo de un servicio no afecta a otros

### ¿Por qué REST sobre gRPC?
- **Simplicidad**: Más fácil de debuggear y testear
- **Compatibilidad**: Cualquier cliente HTTP puede consumir las APIs
- **Tooling**: Swagger/OpenAPI para documentación interactiva
- **Futuro**: Migración a gRPC es posible sin cambiar arquitectura

### ¿Por qué JWT sobre sesiones?
- **Stateless**: No requiere almacenamiento en servidor
- **Escalable**: Funciona en múltiples instancias
- **Cross-domain**: Funciona entre diferentes dominios
- **Mobile-friendly**: Ideal para apps móviles futuras

---

## 🎯 Mejores Prácticas Implementadas

### 1. Código y Arquitectura

| Práctica | Implementación | Beneficio |
|----------|----------------|-----------|
| **Async/Await** | Todos los métodos I/O son asíncronos | Mayor throughput y escalabilidad |
| **Inyección de Dependencias** | Nativa de .NET | Testabilidad y desacoplamiento |
| **Logging estructurado** | ILogger de .NET | Debugging más fácil |
| **Configuration externa** | appsettings.json | Configuración sin recompilar |
| **Health Checks** | Endpoints /health | Monitoreo y orquestación |

### 2. Seguridad

```csharp
✅ Passwords hasheados con BCrypt (12 rounds)
✅ JWT con expiración de 24 horas
✅ Validación de entrada con FluentValidation
✅ HTTPS en producción (configurado)
✅ CORS configurado correctamente
✅ No se exponen entidades directamente (DTOs)
✅ Autorización basada en roles
```

### 3. Performance

```csharp
✅ Consultas asíncronas (async/await)
✅ Índices en columnas frecuentemente consultadas
✅ Carga selectiva de datos (Select específicos)
✅ Connection pooling de EF Core
✅ HTTP client factory para reutilizar conexiones
```

### 4. Mantenibilidad

```csharp
✅ Separación de responsabilidades (capas)
✅ Nombres descriptivos y convenciones C#
✅ Documentación XML en código
✅ Swagger para documentación de APIs
✅ Validaciones declarativas (FluentValidation)
```

---

## ⚖️ Comparación de Arquitecturas

### Microservicios vs Monolito

| Aspecto | Microservicios (Actual) | Monolito |
|---------|-------------------------|----------|
| **Escalabilidad** | ✅ Escalar servicios individualmente | ❌ Escalar toda la app |
| **Despliegue** | ✅ Independiente por servicio | ❌ Todo junto |
| **Complejidad** | ⚠️ Mayor (comunicación, monitoreo) | ✅ Menor |
| **Desarrollo** | ✅ Equipos independientes | ⚠️ Coordinación necesaria |
| **Resiliencia** | ✅ Fallos aislados | ❌ Fallo afecta todo |
| **Latencia** | ⚠️ Llamadas de red | ✅ En memoria |
| **Testing** | ⚠️ Tests de integración complejos | ✅ Tests más simples |

**Conclusión**: Microservicios son ideales cuando:
- El equipo es grande o distribuido
- Se necesita escalar partes específicas
- Los dominios están bien definidos
- Se espera crecimiento rápido

### REST vs gRPC

| Aspecto | REST (Actual) | gRPC |
|---------|---------------|------|
| **Facilidad de uso** | ✅ Simple, HTTP estándar | ⚠️ Requiere definir .proto |
| **Performance** | ⚠️ JSON, HTTP/1.1 | ✅ Binario, HTTP/2 |
| **Tooling** | ✅ Swagger, Postman | ⚠️ Herramientas específicas |
| **Debugging** | ✅ Fácil con curl, browser | ⚠️ Requiere herramientas |
| **Compatibilidad** | ✅ Universal | ⚠️ No todos los clients |
| **Streaming** | ❌ Limitado | ✅ Bidireccional |

**Decisión**: REST para MVP, considerar gRPC en el futuro para comunicación interna.

### Síncrono vs Asíncrono (Mensajería)

| Aspecto | HTTP Síncrono (Actual) | Message Queue |
|---------|------------------------|---------------|
| **Simplicidad** | ✅ Directo | ⚠️ Infraestructura extra |
| **Latencia** | ✅ Baja (respuesta inmediata) | ⚠️ Procesamiento diferido |
| **Resiliencia** | ⚠️ Requiere Polly | ✅ Retry automático |
| **Acoplamiento** | ⚠️ Servicios deben estar arriba | ✅ Desacoplado |
| **Debugging** | ✅ Fácil | ⚠️ Más complejo |
| **Consistencia** | ✅ Fuerte | ⚠️ Eventual |

**Decisión**: HTTP para operaciones que requieren respuesta inmediata (crear reserva).
Considerar mensajería para:
- Notificaciones
- Procesamiento en batch
- Eventos de dominio

---

## 📐 Principios SOLID Aplicados

### Single Responsibility Principle (SRP)
- ✅ Cada clase tiene una responsabilidad única
- ✅ Controllers solo manejan HTTP
- ✅ Services solo lógica de negocio
- ✅ Repositories solo acceso a datos

### Open/Closed Principle (OCP)
- ✅ Uso de interfaces (abierto para extensión)
- ✅ Implementaciones concretas cerradas
- ✅ Fácil agregar nuevos servicios sin modificar existentes

### Liskov Substitution Principle (LSP)
- ✅ Todas las implementaciones de IRepository son intercambiables
- ✅ Se pueden inyectar mocks en tests

### Interface Segregation Principle (ISP)
- ✅ Interfaces pequeñas y específicas
- ✅ Clientes no dependen de métodos que no usan

### Dependency Inversion Principle (DIP)
- ✅ Dependencia de abstracciones (interfaces)
- ✅ No dependencia de implementaciones concretas
- ✅ DI container maneja las instancias

---

## 🔮 Consideraciones para Producción

### Seguridad
- [ ] Implementar rate limiting
- [ ] Agregar API keys para comunicación entre servicios
- [ ] Configurar HTTPS con certificados válidos
- [ ] Implementar refresh tokens
- [ ] Agregar logging de eventos de seguridad

### Performance
- [ ] Implementar caching con Redis
- [ ] Agregar CDN para recursos estáticos
- [ ] Optimizar queries con índices compuestos
- [ ] Implementar pagination en listados grandes
- [ ] Usar compression (gzip/brotli)

### Observabilidad
- [ ] Implementar Application Insights
- [ ] Agregar distributed tracing (OpenTelemetry)
- [ ] Configurar alertas (CPU, memoria, errores)
- [ ] Dashboards con métricas clave
- [ ] Logging centralizado (ELK stack)

### DevOps
- [ ] Dockerizar servicios
- [ ] CI/CD con GitHub Actions
- [ ] Despliegue en Kubernetes
- [ ] Blue-green deployments
- [ ] Automated rollbacks

---

## 📚 Recursos Adicionales

### Documentación Relacionada
- [README.md](../README.md) - Información general y guía de inicio rápido
- [DEVELOPMENT.md](DEVELOPMENT.md) - Guía para desarrolladores
- [DATABASE.md](DATABASE.md) - Esquema de base de datos
- [POSTMAN.md](../postman/POSTMAN.md) - Testing con Postman

### Referencias Externas
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/aspnet/core/fundamentals/best-practices)
- [Microservices Patterns](https://microservices.io/patterns/)
- [Polly Documentation](https://github.com/App-vNext/Polly)

---

**Última actualización:** Octubre 2025  
**Versión de la arquitectura:** 1.0