namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Interface for WhatsApp notification services
/// </summary>
public interface INotificationService
{
    Task<bool> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    Task<bool> SendTemplateMessageAsync(string phoneNumber, string templateName, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}