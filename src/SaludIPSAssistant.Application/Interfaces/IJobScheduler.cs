namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Interface for background job scheduling
/// </summary>
public interface IJobScheduler
{
    Task<string> ScheduleReminderAsync(Guid appointmentId, DateTime scheduledTime, string jobType, CancellationToken cancellationToken = default);
    Task<bool> CancelJobAsync(string jobId, CancellationToken cancellationToken = default);
}