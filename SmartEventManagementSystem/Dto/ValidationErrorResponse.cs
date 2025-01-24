using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Smart_Event_Management_System.Dto;

public class ValidationErrorResponse
{
    public string Message { get; set; } = null!;
    
    public List<ValidationFailure> Errors { get; set; } = new List<ValidationFailure>();
}
