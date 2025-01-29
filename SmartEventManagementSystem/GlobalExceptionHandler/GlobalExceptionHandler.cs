using System.Net;
using FluentValidation;
using Smart_Event_Management_System.CustomException;
using Smart_Event_Management_System.Dto;

namespace Smart_Event_Management_System.GlobalExceptionHandler;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var message = "An unexpected error occurred.";
        var statusCode = (int)HttpStatusCode.InternalServerError;
                
        switch (ex)
        {
            case UnauthorizedAccessException uae:
                message = uae.Message;
                statusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            case ArgumentNullException argNullEx:
                message = argNullEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case ArgumentException ae:
                message = ae.Message ;
                statusCode = (int)HttpStatusCode.BadRequest ;
                break;
            
            case NotFoundException notFoundEx:
                message = notFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidIDException invalidIDEx:
                message = invalidIDEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UserAlreadyExistException userExistEx:
                message = userExistEx.Message;
                statusCode = (int)HttpStatusCode.Conflict;
                break;

            case DataNotValidException dataNotValidEx:
                message = dataNotValidEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case EventCompleteException eventCompleteEx:
                message = eventCompleteEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case NoEventFoundException noEventFoundEx:
                message = noEventFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case NoTicketFoundException noTicketFoundEx:
                message = noTicketFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidOperationException invalidOpEx:
                message = invalidOpEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ValidationException validationEx:
                message = validationEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            default:
                _logger.LogError(ex, "An error occurred while processing the request.");
                break;
        }
        
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Message = message
        };
        
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}