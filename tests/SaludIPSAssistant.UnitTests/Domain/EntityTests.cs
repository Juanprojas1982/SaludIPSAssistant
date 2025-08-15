using Xunit;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.UnitTests.Domain;

public class PatientTests
{
    [Fact]
    public void Patient_Creation_Should_Set_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var phoneNumber = "+573001234567";
        var name = "Juan Pérez";
        var createdAt = DateTime.UtcNow;

        // Act
        var patient = new Patient
        {
            Id = id,
            PhoneNumber = phoneNumber,
            Name = name,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, patient.Id);
        Assert.Equal(phoneNumber, patient.PhoneNumber);
        Assert.Equal(name, patient.Name);
        Assert.Equal(createdAt, patient.CreatedAt);
        Assert.NotNull(patient.Appointments);
        Assert.NotNull(patient.Conversations);
    }
}

public class AppointmentTests
{
    [Fact]
    public void Appointment_Creation_Should_Set_Default_Status()
    {
        // Arrange & Act
        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDate = DateTime.Today.AddDays(1),
            AppointmentTime = new TimeSpan(10, 0, 0),
            Status = AppointmentStatus.Available
        };

        // Assert
        Assert.Equal(AppointmentStatus.Available, appointment.Status);
        Assert.False(appointment.OneDayReminderSent);
        Assert.False(appointment.OneHourReminderSent);
    }

    [Fact]
    public void Appointment_Should_Allow_Status_Update()
    {
        // Arrange
        var appointment = new Appointment
        {
            Status = AppointmentStatus.Available
        };

        // Act
        appointment.Status = AppointmentStatus.Asignado;

        // Assert
        Assert.Equal(AppointmentStatus.Asignado, appointment.Status);
    }
}

public class ConversationTests
{
    [Fact]
    public void Conversation_Creation_Should_Set_Initial_State()
    {
        // Arrange & Act
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            SessionId = Guid.NewGuid().ToString(),
            Status = ConversationStatus.Active,
            CurrentState = ConversationState.Initial,
            StartedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(ConversationStatus.Active, conversation.Status);
        Assert.Equal(ConversationState.Initial, conversation.CurrentState);
        Assert.NotNull(conversation.Messages);
    }
}