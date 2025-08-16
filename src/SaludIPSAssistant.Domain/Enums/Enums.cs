namespace SaludIPSAssistant.Domain.Enums;

public enum AppointmentStatus
{
    Available = 0,
    Asignado = 1,
    ConfirmaAsistencia = 2,
    Cancelado = 3,
    Completed = 4,
    NoShow = 5
}

public enum ConversationStatus
{
    Active = 0,
    Completed = 1,
    Paused = 2,
    Canceled = 3
}

public enum ConversationState
{
    Initial = 0,
    CollectingSpecialty = 1,
    ShowingAvailableSlots = 2,
    ConfirmingAppointment = 3,
    AppointmentBooked = 4,
    RequestingChange = 5,
    Completed = 6
}

public enum MessageDirection
{
    Inbound = 0,
    Outbound = 1
}

public enum ReminderType
{
    OneDayBefore = 0,
    OneHourBefore = 1
}