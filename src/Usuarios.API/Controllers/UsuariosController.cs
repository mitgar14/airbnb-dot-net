using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Usuarios.API.DTOs;
using Usuarios.API.Services;
using Airbnb.Common.Controllers;
using Airbnb.Common.Constants;
using Airbnb.Common.Models;

namespace Usuarios.API.Controllers;

[Route("api/[controller]")]
public class UsuariosController : BaseApiController
{
    private readonly IUsuarioService _usuarioService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IUsuarioService usuarioService, IJwtService jwtService, ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _jwtService = jwtService;
        _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioDto>>>> GetUsuarios()
    {
        try
        {
            var usuarios = await _usuarioService.GetAllUsuariosAsync();
            var response = ApiResponse<IEnumerable<UsuarioDto>>.SuccessResponse(
                usuarios,
                "Usuarios obtenidos exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            return InternalServerError<IEnumerable<UsuarioDto>>(
                "Error al obtener usuarios. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize] // Cambiado de "Admin" a cualquier usuario autenticado para permitir llamadas de otros microservicios
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetUsuario(long id)
    {
        try
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                var notFoundResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                    $"Usuario con ID {id} no encontrado",
                    404
                );
                return NotFound(notFoundResponse);
            }
            
            var response = ApiResponse<UsuarioDto>.SuccessResponse(
                usuario,
                "Usuario obtenido exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UserId}", id);
            return InternalServerError<UsuarioDto>(
                "Error al obtener usuario. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("validation")]
    public async Task<ActionResult<ApiResponse<ValidationUsuarioDto>>> ValidateUsuario(
        [FromQuery] string email, 
        [FromQuery] string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var badRequestResponse = ApiResponse<ValidationUsuarioDto>.ErrorResponse(
                    "Email y contraseña son requeridos",
                    400
                );
                return BadRequest(badRequestResponse);
            }

            var usuario = await _usuarioService.ValidateUsuarioAsync(email, password);
            if (usuario == null)
            {
                var unauthorizedResponse = ApiResponse<ValidationUsuarioDto>.ErrorResponse(
                    "Credenciales inválidas",
                    401
                );
                return Unauthorized(unauthorizedResponse);
            }
            
            var response = ApiResponse<ValidationUsuarioDto>.SuccessResponse(
                usuario,
                "Usuario validado exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar usuario {Email}", email);
            var response = ApiResponse<ValidationUsuarioDto>.ErrorResponse(
                "Error al validar usuario. Por favor, intente nuevamente",
                500
            );
            return StatusCode(500, response);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> CreateUsuario(
        [FromBody] CreateUsuarioDto createUsuarioDto)
    {
        try
        {
            var usuario = await _usuarioService.CreateUsuarioAsync(createUsuarioDto);
            var response = ApiResponse<UsuarioDto>.SuccessResponse(
                usuario,
                "Usuario creado exitosamente",
                201
            );
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UserId }, response);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al crear usuario");
            
            // Detectar si es un error de clave duplicada
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
            {
                // Determinar si es ID o email duplicado
                if (errorMessage.Contains("Email", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("IX_Usuarios_Email", StringComparison.OrdinalIgnoreCase))
                {
                    var conflictResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                        $"Ya existe un usuario con el email '{createUsuarioDto.Email}'",
                        409
                    );
                    return Conflict(conflictResponse);
                }
                else
                {
                    var conflictResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                        $"Ya existe un usuario con el ID '{createUsuarioDto.UserId}'. Los IDs deben ser únicos",
                        409
                    );
                    return Conflict(conflictResponse);
                }
            }
            
            return InternalServerError<UsuarioDto>(
                "Error al crear usuario en la base de datos. Por favor, verifique los datos e intente nuevamente"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return InternalServerError<UsuarioDto>(
                "Error al crear usuario. Por favor, verifique los datos e intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> UpdateUsuario(
        long id, 
        [FromBody] UpdateUsuarioDto updateUsuarioDto)
    {
        try
        {
            var updatedUsuario = await _usuarioService.UpdateUsuarioAsync(id, updateUsuarioDto);
            var response = ApiResponse<UsuarioDto>.SuccessResponse(
                updatedUsuario,
                "Usuario actualizado exitosamente",
                200
            );
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            var notFoundResponse = ApiResponse<UsuarioDto>.ErrorResponse(ex.Message, 404);
            return NotFound(notFoundResponse);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error de base de datos al actualizar usuario {UserId}", id);
            
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase))
            {
                // Determinar si es email duplicado
                if (errorMessage.Contains("Email", StringComparison.OrdinalIgnoreCase) ||
                    errorMessage.Contains("IX_Usuarios_Email", StringComparison.OrdinalIgnoreCase))
                {
                    var conflictResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                        $"Ya existe otro usuario con el email '{updateUsuarioDto.Email}'",
                        409
                    );
                    return Conflict(conflictResponse);
                }
                
                var generalConflictResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                    "Error al actualizar: el nuevo valor genera un conflicto con datos existentes",
                    409
                );
                return Conflict(generalConflictResponse);
            }
            
            return InternalServerError<UsuarioDto>(
                "Error al actualizar usuario en la base de datos. Por favor, intente nuevamente"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UserId}", id);
            return InternalServerError<UsuarioDto>(
                "Error al actualizar usuario. Por favor, intente nuevamente"
            );
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<UsuarioDto>>> DeleteUsuario(long id)
    {
        try
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                var notFoundResponse = ApiResponse<UsuarioDto>.ErrorResponse(
                    $"Usuario con ID {id} no encontrado",
                    404
                );
                return NotFound(notFoundResponse);
            }

            await _usuarioService.DeleteUsuarioAsync(id);
            
            var response = ApiResponse<UsuarioDto>.SuccessResponse(
                usuario,
                "Usuario eliminado exitosamente",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {UserId}", id);
            return InternalServerError<UsuarioDto>(
                "Error al eliminar usuario. Por favor, intente nuevamente"
            );
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto loginRequest)
    {
        try
        {
            var validationResult = await _usuarioService.ValidateUsuarioAsync(
                loginRequest.Email, 
                loginRequest.Password
            );
            
            if (validationResult == null)
            {
                var unauthorizedResponse = ApiResponse<LoginResponseDto>.ErrorResponse(
                    "Credenciales inválidas",
                    401
                );
                return Unauthorized(unauthorizedResponse);
            }

            var usuarioDto = new UsuarioDto
            {
                UserId = validationResult.UserId,
                Name = validationResult.Name,
                Role = validationResult.Role,
                Email = validationResult.Email
            };

            var token = _jwtService.GenerateToken(
                validationResult.UserId, 
                validationResult.Email, 
                validationResult.Role
            );

            var loginResponse = new LoginResponseDto
            {
                Token = token,
                Usuario = usuarioDto
            };

            var response = ApiResponse<LoginResponseDto>.SuccessResponse(
                loginResponse,
                "Login exitoso",
                200
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar login");
            return InternalServerError<LoginResponseDto>(
                "Error al realizar login. Por favor, intente nuevamente"
            );
        }
    }
}
