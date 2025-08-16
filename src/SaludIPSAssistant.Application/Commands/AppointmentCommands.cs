using MediatR;
using SaludIPSAssistant.Application.DTOs;

namespace SaludIPSAssistant.Application.Commands;

public record ProcessIncomingMessageCommand(IncomingMessageDto Message) : IRequest<bool>;

public record BookAppointmentCommand(
    Guid PatientId,
    string SlotId,
    string SpecialtyId) : IRequest<Guid>;

public record UpdateAppointmentStatusCommand(
    Guid AppointmentId,
    string NewStatus) : IRequest<bool>;

public record SendReminderCommand(
    Guid AppointmentId,
    string ReminderType) : IRequest<bool>;

public record CancelAppointmentCommand(
    Guid AppointmentId,
    string Reason) : IRequest<bool>;

public record MoveAppointmentCommand(
    Guid AppointmentId,
    string NewSlotId) : IRequest<bool>;