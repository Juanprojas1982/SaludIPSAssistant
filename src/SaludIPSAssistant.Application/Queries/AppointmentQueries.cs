using MediatR;
using SaludIPSAssistant.Application.DTOs;

namespace SaludIPSAssistant.Application.Queries;

public record GetAvailableSlotsQuery(
    string SpecialtyId,
    DateTime FromDate,
    DateTime ToDate) : IRequest<IEnumerable<AppointmentSlotDto>>;

public record GetPatientAppointmentsQuery(
    Guid PatientId) : IRequest<IEnumerable<AppointmentDto>>;

public record GetConversationHistoryQuery(
    Guid PatientId,
    int Take = 10) : IRequest<IEnumerable<ConversationDto>>;

public record GetPendingRemindersQuery(
    DateTime UpToDate) : IRequest<IEnumerable<ReminderDto>>;

public record ConversationDto
{
    public Guid Id { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public string CurrentState { get; init; } = string.Empty;
}

public record ReminderDto
{
    public Guid Id { get; init; }
    public Guid AppointmentId { get; init; }
    public string Type { get; init; } = string.Empty;
    public DateTime ScheduledFor { get; init; }
    public AppointmentDto Appointment { get; init; } = null!;
}