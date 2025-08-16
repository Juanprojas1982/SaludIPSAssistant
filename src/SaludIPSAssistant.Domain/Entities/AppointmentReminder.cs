using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.Domain.Entities;

public class AppointmentReminder
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public ReminderType Type { get; set; }
    public DateTime ScheduledFor { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Response { get; set; } // Patient response to reminder
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Appointment Appointment { get; set; } = null!;
}