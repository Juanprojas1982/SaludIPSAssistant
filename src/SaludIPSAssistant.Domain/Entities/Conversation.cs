using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public ConversationStatus Status { get; set; }
    public ConversationState CurrentState { get; set; }
    public string Context { get; set; } = string.Empty; // JSON context for state management
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public DateTime LastActivity { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageDirection Direction { get; set; }
    public DateTime Timestamp { get; set; }
    public string? MessageId { get; set; } // External WhatsApp message ID
    public bool IsProcessed { get; set; }

    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
}