using System.Net.Http.Headers;
using System.Text.Json;
using Reservas.API.DTOs;

namespace Reservas.API.Services;

public class MicroserviceClient : IMicroserviceClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MicroserviceClient> _logger;

    public MicroserviceClient(IHttpClientFactory httpClientFactory, ILogger<MicroserviceClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<UsuarioDto?> GetUsuarioAsync(string userId, string? authToken = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("UsuariosAPI");
            
            // Agregar token de autenticación si está disponible
            if (!string.IsNullOrEmpty(authToken))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", authToken);
            }
            
            var response = await client.GetAsync($"/api/usuarios/{userId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Usuario {UserId} no encontrado. Status: {StatusCode}", userId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            // Deserializar el ApiResponse wrapper
            var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<UsuarioDto>>(
                content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            
            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId} del microservicio", userId);
            throw;
        }
    }

    public async Task<AirbnbDto?> GetAirbnbAsync(string airbnbId, string? authToken = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AirbnbsAPI");
            
            // Agregar token de autenticación si está disponible
            if (!string.IsNullOrEmpty(authToken))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", authToken);
            }
            
            var response = await client.GetAsync($"/api/airbnbs/id/{airbnbId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Airbnb {AirbnbId} no encontrado. Status: {StatusCode}", airbnbId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            // Deserializar el ApiResponse wrapper
            var apiResponse = JsonSerializer.Deserialize<ApiResponseWrapper<AirbnbDto>>(
                content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            
            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener airbnb {AirbnbId} del microservicio", airbnbId);
            throw;
        }
    }
    
    // Clase helper para deserializar ApiResponse
    private class ApiResponseWrapper<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}
