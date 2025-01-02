using FluentValidation;
using Smart_Event_Management_System.Models;
using Smart_Event_Management_System.Repository;

namespace Smart_Event_Management_System.Service;

public class CheckInLogService : ICheckInLogService
{
    private readonly ICheckInLogRepository _checkInLogRepository;
    private readonly IValidator<CheckInLog> _checkInLogValidator;

    public CheckInLogService(IValidator<CheckInLog> checkInLogValidator,
        ICheckInLogRepository checkInLogRepository)
    {
        _checkInLogValidator = checkInLogValidator;
        _checkInLogRepository = checkInLogRepository;
    }

    public async Task<IEnumerable<CheckInLog>> GetAllCheckInLogsAsync()
    {
        return await _checkInLogRepository.GetAllCheckInLogsAsync();
    }

    public async Task<CheckInLog> GetCheckInLogByIdAsync(int id)
    {
        return await _checkInLogRepository.GetCheckInLogByIdAsync(id);
    }

    public async Task<CheckInLog> CreateCheckInLogAsync(CheckInLog checkInLog)
    {
        var validationResult = await _checkInLogValidator.ValidateAsync(checkInLog);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        checkInLog = await _checkInLogRepository.CreateCheckInLogAsync(checkInLog);

        return checkInLog;
    }

    public async Task<bool> DeleteCheckInLogAsync(int id)
    {
        return await _checkInLogRepository.DeleteCheckInLogAsync(id);
    }
}