using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Reservas.API.DTOs;
using Reservas.API.Services;
using Airbnb.Common.Controllers;
using Airbnb.Common.Models;

namespace Reservas.API.Controllers;

[Route("api/[controller]")]
public class ReservasController : BaseApiController
{
    private readonly IReservaService _reservaService;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(IReservaService reservaService, ILogger<ReservasController> logger)
    {
        _reservaService = reservaService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservaDto>>>> GetReservas()
    {
        try
        {
            var reservas = await _reservaService.GetAllReservasAsync();
            var response = ApiResponse<IEnumerable<ReservaDto>>.SuccessResponse(
                reservas,
                "Reservas obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reservas");
            return InternalServerError<IEnumerable<ReservaDto>>(
                "Error al obtener reservas. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Cliente,Admin")]
    [HttpGet("userID/{userId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservaDto>>>> GetReservasByUserId(string userId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var reservas = await _reservaService.GetReservasByUserIdAsync(userId, currentUserId!, currentUserRole!);
            var response = ApiResponse<IEnumerable<ReservaDto>>.SuccessResponse(
                reservas,
                $"Reservas del usuario {userId} obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de ver reservas del usuario {UserId}", userId);
            return Forbidden<IEnumerable<ReservaDto>>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reservas por user ID {UserId}", userId);
            return InternalServerError<IEnumerable<ReservaDto>>(
                "Error al obtener reservas del usuario. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Cliente,Host,Admin")]
    [HttpGet("reservationID/{reservationId}")]
    public async Task<ActionResult<ApiResponse<ReservaDto>>> GetReserva(int reservationId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var reserva = await _reservaService.GetReservaByIdAsync(reservationId, currentUserId!, currentUserRole!);
            if (reserva == null)
            {
                var notFoundResponse = ApiResponse<ReservaDto>.ErrorResponse(
                    $"Reserva con ID {reservationId} no encontrada",
                    404
                );
                return NotFound(notFoundResponse);
            }
            
            var response = ApiResponse<ReservaDto>.SuccessResponse(
                reserva,
                "Reserva obtenida exitosamente",
                200
            );
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de ver reserva {ReservationId}", reservationId);
            return Forbidden<ReservaDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reserva {ReservationId}", reservationId);
            return InternalServerError<ReservaDto>(
                "Error al obtener reserva. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Host,Admin")]
    [HttpGet("hostID/{hostId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReservaDto>>>> GetReservasByHostId(string hostId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var reservas = await _reservaService.GetReservasByHostIdAsync(hostId, currentUserId!, currentUserRole!);
            var response = ApiResponse<IEnumerable<ReservaDto>>.SuccessResponse(
                reservas,
                $"Reservas del host {hostId} obtenidas exitosamente",
                200
            );
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de ver reservas del host {HostId}", hostId);
            return Forbidden<IEnumerable<ReservaDto>>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reservas por host ID {HostId}", hostId);
            return InternalServerError<IEnumerable<ReservaDto>>(
                "Error al obtener reservas del host. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Cliente,Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReservaDto>>> CreateReserva([FromBody] CreateReservaDto createReservaDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var reserva = await _reservaService.CreateReservaAsync(createReservaDto, userId!, userRole!);
            var response = ApiResponse<ReservaDto>.SuccessResponse(
                reserva,
                "Reserva creada exitosamente",
                201
            );
            return CreatedAtAction(nameof(GetReserva), new { reservationId = reserva.ReservationId }, response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de crear reserva");
            return Forbidden<ReservaDto>(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error de validación al crear reserva");
            var response = ApiResponse<ReservaDto>.ErrorResponse(ex.Message, 404);
            return NotFound(response);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al crear reserva");
            
            // Detectar si es un error de clave duplicada
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
            {
                var conflictResponse = ApiResponse<ReservaDto>.ErrorResponse(
                    "Error al crear la reserva: ya existe un registro con el mismo ID. Esto puede ser un error de base de datos",
                    409
                );
                return Conflict(conflictResponse);
            }
            
            return InternalServerError<ReservaDto>(
                "Error al crear reserva en la base de datos. Por favor, verifique los datos e intente nuevamente"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear reserva");
            return InternalServerError<ReservaDto>(
                "Error al crear reserva. Por favor, verifique los datos e intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Cliente,Admin")]
    [HttpDelete("reservationID/{reservationId}")]
    public async Task<ActionResult<ApiResponse<ReservaDto>>> DeleteReserva(int reservationId)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            var reserva = await _reservaService.GetReservaByIdAsync(reservationId, currentUserId!, currentUserRole!);
            if (reserva == null)
            {
                var notFoundResponse = ApiResponse<ReservaDto>.ErrorResponse(
                    $"Reserva con ID {reservationId} no encontrada",
                    404
                );
                return NotFound(notFoundResponse);
            }

            await _reservaService.DeleteReservaAsync(reservationId, currentUserId!, currentUserRole!);
            
            var response = ApiResponse<ReservaDto>.SuccessResponse(
                reserva,
                "Reserva eliminada exitosamente",
                200
            );
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var response = ApiResponse<ReservaDto>.ErrorResponse(ex.Message, 404);
            return NotFound(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento no autorizado de eliminar reserva {ReservationId}", reservationId);
            return Forbidden<ReservaDto>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar reserva {ReservationId}", reservationId);
            return InternalServerError<ReservaDto>(
                "Error al eliminar reserva. Por favor, intente nuevamente"
            );
        }
    }
}
