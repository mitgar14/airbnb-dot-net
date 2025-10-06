using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Airbnbs.API.DTOs;
using Airbnbs.API.Services;
using Airbnb.Common.Controllers;
using Airbnb.Common.Models;

namespace Airbnbs.API.Controllers;

[Route("api/[controller]")]
public class AirbnbsController : BaseApiController
{
    private readonly IAirbnbService _airbnbService;
    private readonly ILogger<AirbnbsController> _logger;

    public AirbnbsController(IAirbnbService airbnbService, ILogger<AirbnbsController> logger)
    {
        _airbnbService = airbnbService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<AirbnbDto>>>> GetAirbnbs()
    {
        try
        {
            var airbnbs = await _airbnbService.GetAllAirbnbsAsync();
            var response = ApiResponse<IEnumerable<AirbnbDto>>.SuccessResponse(
                airbnbs,
                "Propiedades obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener airbnbs");
            return InternalServerError<IEnumerable<AirbnbDto>>(
                "Error al obtener propiedades. Por favor, intente nuevamente"
            );
        }
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<ApiResponse<AirbnbDto>>> GetAirbnb(string id)
    {
        try
        {
            var airbnb = await _airbnbService.GetAirbnbByIdAsync(id);
            if (airbnb == null)
            {
                var notFoundResponse = ApiResponse<AirbnbDto>.ErrorResponse(
                    $"Propiedad con ID {id} no encontrada",
                    404
                );
                return NotFound(notFoundResponse);
            }
            
            var response = ApiResponse<AirbnbDto>.SuccessResponse(
                airbnb,
                "Propiedad obtenida exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener airbnb {AirbnbId}", id);
            return InternalServerError<AirbnbDto>(
                "Error al obtener propiedad. Por favor, intente nuevamente"
            );
        }
    }

    [HttpGet("hostId/{hostId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AirbnbDto>>>> GetAirbnbsByHostId(string hostId)
    {
        try
        {
            var airbnbs = await _airbnbService.GetAirbnbsByHostIdAsync(hostId);
            var response = ApiResponse<IEnumerable<AirbnbDto>>.SuccessResponse(
                airbnbs,
                $"Propiedades del host {hostId} obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener airbnbs por host {HostId}", hostId);
            return InternalServerError<IEnumerable<AirbnbDto>>(
                "Error al obtener propiedades del host. Por favor, intente nuevamente"
            );
        }
    }

    [HttpGet("roomType/{roomType}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AirbnbDto>>>> GetAirbnbsByRoomType(string roomType)
    {
        try
        {
            var airbnbs = await _airbnbService.GetAirbnbsByRoomTypeAsync(roomType);
            var response = ApiResponse<IEnumerable<AirbnbDto>>.SuccessResponse(
                airbnbs,
                $"Propiedades de tipo '{roomType}' obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener airbnbs por tipo {RoomType}", roomType);
            return InternalServerError<IEnumerable<AirbnbDto>>(
                "Error al obtener propiedades por tipo. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Host,Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<AirbnbDto>>> CreateAirbnb(
        [FromBody] CreateAirbnbDto createAirbnbDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var airbnb = await _airbnbService.CreateAirbnbAsync(createAirbnbDto, userId!, userRole!);
            var response = ApiResponse<AirbnbDto>.SuccessResponse(
                airbnb,
                "Propiedad creada exitosamente",
                201
            );
            return CreatedAtAction(nameof(GetAirbnb), new { id = airbnb.Id }, response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de crear propiedad");
            return Forbidden<AirbnbDto>(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al crear airbnb");
            
            // Detectar si es un error de clave duplicada
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
            {
                var conflictResponse = ApiResponse<AirbnbDto>.ErrorResponse(
                    $"Ya existe una propiedad con el ID '{createAirbnbDto.Id}'. Los IDs deben ser únicos",
                    409
                );
                return Conflict(conflictResponse);
            }
            
            return InternalServerError<AirbnbDto>(
                "Error al crear propiedad en la base de datos. Por favor, verifique los datos e intente nuevamente"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear airbnb");
            return InternalServerError<AirbnbDto>(
                "Error al crear propiedad. Por favor, verifique los datos e intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Host,Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AirbnbDto>>> UpdateAirbnb(
        string id, 
        [FromBody] UpdateAirbnbDto updateAirbnbDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var updatedAirbnb = await _airbnbService.UpdateAirbnbAsync(id, updateAirbnbDto, userId!, userRole!);
            var response = ApiResponse<AirbnbDto>.SuccessResponse(
                updatedAirbnb,
                "Propiedad actualizada exitosamente",
                200
            );
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var response = ApiResponse<AirbnbDto>.ErrorResponse(ex.Message, 404);
            return NotFound(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de actualizar propiedad {AirbnbId}", id);
            return Forbidden<AirbnbDto>(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al actualizar airbnb {AirbnbId}", id);
            
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase))
            {
                var conflictResponse = ApiResponse<AirbnbDto>.ErrorResponse(
                    "Error al actualizar: el nuevo valor genera un conflicto con datos existentes",
                    409
                );
                return Conflict(conflictResponse);
            }
            
            return InternalServerError<AirbnbDto>(
                "Error al actualizar propiedad en la base de datos. Por favor, intente nuevamente"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar airbnb {AirbnbId}", id);
            return InternalServerError<AirbnbDto>(
                "Error al actualizar propiedad. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Host,Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<AirbnbDto>>> DeleteAirbnb(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var airbnb = await _airbnbService.GetAirbnbByIdAsync(id);
            if (airbnb == null)
            {
                var notFoundResponse = ApiResponse<AirbnbDto>.ErrorResponse(
                    $"Propiedad con ID {id} no encontrada",
                    404
                );
                return NotFound(notFoundResponse);
            }

            await _airbnbService.DeleteAirbnbAsync(id, userId!, userRole!);
            
            var response = ApiResponse<AirbnbDto>.SuccessResponse(
                airbnb,
                "Propiedad eliminada exitosamente",
                200
            );
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var response = ApiResponse<AirbnbDto>.ErrorResponse(ex.Message, 404);
            return NotFound(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de eliminar propiedad {AirbnbId}", id);
            return Forbidden<AirbnbDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar airbnb {AirbnbId}", id);
            return InternalServerError<AirbnbDto>(
                "Error al eliminar propiedad. Por favor, intente nuevamente"
            );
        }
    }
}
