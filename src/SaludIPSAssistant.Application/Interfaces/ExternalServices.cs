using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Domain.ValueObjects;

namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Interface for WhatsApp notification services
/// </summary>
public interface INotificationService
{
    Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for external agenda/appointment API integration
/// </summary>
public interface IAgendaProvider
{
    Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsAsync(string specialtyId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<bool> UpdateAppointmentStatusAsync(string appointmentId, string status, CancellationToken cancellationToken = default);
    Task<bool> MoveAppointmentAsync(string fromSlotId, string toSlotId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialtyId, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for NLP service to interpret user messages
/// </summary>
public interface INlpService
{
    Task<string> InterpretIntentAsync(string message, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> ExtractEntitiesAsync(string message, CancellationToken cancellationToken = default);
    Task<string> GenerateResponseAsync(string intent, Dictionary<string, string> entities, string context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for background job scheduling
/// </summary>
public interface IJobScheduler
{
    Task<string> ScheduleReminderAsync(Guid appointmentId, DateTime scheduledTime, string jobType, CancellationToken cancellationToken = default);
    Task<bool> CancelJobAsync(string jobId, CancellationToken cancellationToken = default);
}