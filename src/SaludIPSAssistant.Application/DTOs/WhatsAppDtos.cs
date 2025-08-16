using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.Application.DTOs;

public record IncomingMessageDto
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? MessageId { get; init; }
    public DateTime Timestamp { get; init; }
}

public record OutgoingMessageDto
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? TemplateId { get; init; }
    public Dictionary<string, string>? Parameters { get; init; }
}

public record AppointmentSlotDto
{
    public string Id { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public TimeSpan Time { get; init; }
    public string DoctorId { get; init; } = string.Empty;
    public string DoctorName { get; init; } = string.Empty;
    public string SpecialtyId { get; init; } = string.Empty;
    public string SpecialtyName { get; init; } = string.Empty;
}

public record AppointmentDto
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = string.Empty;
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public TimeSpan AppointmentTime { get; init; }
    public string SpecialtyName { get; init; } = string.Empty;
    public AppointmentStatus Status { get; init; }
    public string DoctorName { get; init; } = string.Empty;
    public string PatientName { get; init; } = string.Empty;
}

public record ConversationContextDto
{
    public string? SelectedSpecialty { get; init; }
    public List<AppointmentSlotDto>? AvailableSlots { get; init; }
    public string? SelectedSlotId { get; init; }
    public Guid? PendingAppointmentId { get; init; }
    public DateTime? LastInteraction { get; init; }
}