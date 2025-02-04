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
        var errorMessage = "An unexpected error occurred.";
        var statusCode = (int)HttpStatusCode.InternalServerError;
                
        switch (ex)
        {
            case UnauthorizedAccessException uae:
                errorMessage = uae.Message;
                statusCode = (int)HttpStatusCode.Unauthorized;
                break;
            
            case ArgumentNullException argNullEx:
                errorMessage = argNullEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            
            case ArgumentException ae:
                errorMessage = ae.Message ;
                statusCode = (int)HttpStatusCode.BadRequest ;
                break;
            
            case NotFoundException notFoundEx:
                errorMessage = notFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidIDException invalidIDEx:
                errorMessage = invalidIDEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UserAlreadyExistException userExistEx:
                errorMessage = userExistEx.Message;
                statusCode = (int)HttpStatusCode.Conflict;
                break;

            case DataNotValidException dataNotValidEx:
                errorMessage = dataNotValidEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case EventCompleteException eventCompleteEx:
                errorMessage = eventCompleteEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case NoEventFoundException noEventFoundEx:
                errorMessage = noEventFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case NoTicketFoundException noTicketFoundEx:
                errorMessage = noTicketFoundEx.Message;
                statusCode = (int)HttpStatusCode.NotFound;
                break;

            case InvalidOperationException invalidOpEx:
                errorMessage = invalidOpEx.Message;
                statusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ValidationException validationEx:
                errorMessage = validationEx.Message;
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
            ErrorMessage = errorMessage
        };
        
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}