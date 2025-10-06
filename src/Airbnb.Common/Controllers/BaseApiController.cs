using Microsoft.AspNetCore.Mvc;
using Airbnb.Common.Models;

namespace Airbnb.Common.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ObjectResult InternalServerError<T>(string message)
    {
        var response = ApiResponse<T>.ErrorResponse(message, 500);
        return new ObjectResult(response)
        {
            StatusCode = 500
        };
    }

    protected ObjectResult Forbidden<T>(string message)
    {
        var response = ApiResponse<T>.ErrorResponse(message, 403);
        return new ObjectResult(response)
        {
            StatusCode = 403
        };
    }

    protected ObjectResult BadRequestResponse<T>(string message)
    {
        var response = ApiResponse<T>.ErrorResponse(message, 400);
        return new ObjectResult(response)
        {
            StatusCode = 400
        };
    }

    protected new ObjectResult NotFound<T>(string message)
    {
        var response = ApiResponse<T>.ErrorResponse(message, 404);
        return new ObjectResult(response)
        {
            StatusCode = 404
        };
    }

    protected ObjectResult Unauthorized<T>(string message)
    {
        var response = ApiResponse<T>.ErrorResponse(message, 401);
        return new ObjectResult(response)
        {
            StatusCode = 401
        };
    }

    protected ObjectResult SuccessResponse<T>(T data, string message)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, 200);
        return new ObjectResult(response)
        {
            StatusCode = 200
        };
    }

    protected ObjectResult CreatedResponse<T>(T data, string message, string actionName, object routeValues)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, 201);
        return new CreatedAtActionResult(
            actionName,
            null,
            routeValues,
            response
        );
    }
}
