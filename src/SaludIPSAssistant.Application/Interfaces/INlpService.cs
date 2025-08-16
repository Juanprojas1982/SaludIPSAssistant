namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Interface for NLP service to interpret user messages
/// </summary>
public interface INlpService
{
    Task<string> InterpretIntentAsync(string message, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> ExtractEntitiesAsync(string message, CancellationToken cancellationToken = default);
    Task<string> GenerateResponseAsync(string intent, Dictionary<string, string> entities, string context, CancellationToken cancellationToken = default);
}