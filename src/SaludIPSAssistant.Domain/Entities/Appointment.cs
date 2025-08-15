using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public string ExternalId { get; set; } = string.Empty; // ID from external MVC API
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string SpecialtyId { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Reminder tracking
    public bool OneDayReminderSent { get; set; }
    public bool OneHourReminderSent { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public ICollection<AppointmentReminder> Reminders { get; set; } = new List<AppointmentReminder>();
}