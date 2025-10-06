using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Airbnb.Common.Models;
using Reservas.API.Data;
using Reservas.API.Repositories;
using Reservas.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Configure Entity Framework Core with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Register repositories and services
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IMicroserviceClient, MicroserviceClient>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
        
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                
                var response = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "No autorizado. Token inválido o ausente.",
                    Data = null,
                    Success = false,
                    Timestamp = DateTime.UtcNow
                };
                
                await context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                
                var response = new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Message = "Acceso prohibido. No tiene permisos suficientes.",
                    Data = null,
                    Success = false,
                    Timestamp = DateTime.UtcNow
                };
                
                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });
builder.Services.AddAuthorization();

// Configure HttpClient for microservices communication with Polly
builder.Services.AddHttpClient("UsuariosAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"Retry {retryAttempt} for UsuariosAPI after {timespan.TotalSeconds}s");
        }))
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, duration) =>
        {
            Console.WriteLine($"Circuit breaker opened for UsuariosAPI for {duration.TotalSeconds}s");
        },
        onReset: () =>
        {
            Console.WriteLine("Circuit breaker reset for UsuariosAPI");
        }));

builder.Services.AddHttpClient("AirbnbsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"Retry {retryAttempt} for AirbnbsAPI after {timespan.TotalSeconds}s");
        }))
.AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, duration) =>
        {
            Console.WriteLine($"Circuit breaker opened for AirbnbsAPI for {duration.TotalSeconds}s");
        },
        onReset: () =>
        {
            Console.WriteLine("Circuit breaker reset for AirbnbsAPI");
        }));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger/OpenAPI with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Reservas API",
        Version = "v1",
        Description = "API para gestión de reservas"
    });
    
    // Configurar autenticación JWT en Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese 'Bearer' [espacio] y luego su token JWT.\n\nEjemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'"
    });
    
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddMySql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "mysql",
        tags: new[] { "db", "mysql", "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
