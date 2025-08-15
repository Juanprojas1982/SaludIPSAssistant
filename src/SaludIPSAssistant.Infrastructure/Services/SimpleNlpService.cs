using SaludIPSAssistant.Application.Interfaces;

namespace SaludIPSAssistant.Infrastructure.Services;

/// <summary>
/// Simple rule-based NLP service for basic intent recognition
/// In production, this would be replaced with a proper NLP service like Rasa, LUIS, or Dialogflow
/// </summary>
public class SimpleNlpService : INlpService
{
    private readonly Dictionary<string, string[]> _specialtyKeywords;
    private readonly Dictionary<string, string[]> _intentKeywords;

    public SimpleNlpService()
    {
        // Initialize specialty recognition keywords
        _specialtyKeywords = new Dictionary<string, string[]>
        {
            ["general"] = new[] { "medicina general", "general", "médico general", "doctor general" },
            ["cardiology"] = new[] { "cardiología", "corazón", "cardiólogo", "cardiologia" },
            ["dermatology"] = new[] { "dermatología", "piel", "dermatólogo", "dermatologia" },
            ["gynecology"] = new[] { "ginecología", "ginecólogo", "mujer", "ginecologia" },
            ["pediatrics"] = new[] { "pediatría", "niños", "pediatra", "pediatria" },
            ["other"] = new[] { "otro", "otra", "diferente", "otros" }
        };

        // Initialize intent recognition keywords
        _intentKeywords = new Dictionary<string, string[]>
        {
            ["greeting"] = new[] { "hola", "buenos días", "buenas tardes", "buenas noches", "saludos" },
            ["book_appointment"] = new[] { "cita", "agendar", "reservar", "turno", "hora" },
            ["cancel"] = new[] { "cancelar", "anular", "no", "suspender" },
            ["confirm"] = new[] { "sí", "si", "confirmar", "ok", "bien", "perfecto", "yes" },
            ["change"] = new[] { "cambiar", "mover", "diferente", "otro" },
            ["help"] = new[] { "ayuda", "ayudar", "no entiendo", "qué", "cómo" }
        };
    }

    public async Task<string> InterpretIntentAsync(string message, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken); // Simulate processing time
        
        var normalizedMessage = message.ToLowerInvariant();
        
        // Check for intents
        foreach (var intent in _intentKeywords)
        {
            if (intent.Value.Any(keyword => normalizedMessage.Contains(keyword)))
            {
                return intent.Key;
            }
        }
        
        // Default intent
        return "unknown";
    }

    public async Task<Dictionary<string, string>> ExtractEntitiesAsync(string message, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        
        var entities = new Dictionary<string, string>();
        var normalizedMessage = message.ToLowerInvariant();
        
        // Extract specialty
        foreach (var specialty in _specialtyKeywords)
        {
            if (specialty.Value.Any(keyword => normalizedMessage.Contains(keyword)))
            {
                entities["specialty"] = specialty.Key;
                break;
            }
        }
        
        // Extract numbers (for slot selection)
        var numbers = System.Text.RegularExpressions.Regex.Matches(message, @"\d+");
        if (numbers.Count > 0)
        {
            entities["number"] = numbers[0].Value;
        }
        
        // Extract time patterns
        var timePattern = System.Text.RegularExpressions.Regex.Match(message, @"(\d{1,2}):(\d{2})");
        if (timePattern.Success)
        {
            entities["time"] = timePattern.Value;
        }
        
        // Extract date patterns
        var datePattern = System.Text.RegularExpressions.Regex.Match(message, @"(\d{1,2})/(\d{1,2})/(\d{4})");
        if (datePattern.Success)
        {
            entities["date"] = datePattern.Value;
        }
        
        return entities;
    }

    public async Task<string> GenerateResponseAsync(string intent, Dictionary<string, string> entities, string context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        
        return intent switch
        {
            "greeting" => "¡Hola! ¿En qué puedo ayudarte hoy?",
            "help" => "Puedo ayudarte a agendar citas médicas. Solo dime qué especialidad necesitas.",
            "cancel" => "Entiendo. ¿Hay algo más en lo que pueda ayudarte?",
            "confirm" => "Perfecto, continuemos.",
            _ => "Te ayudo a encontrar la cita médica que necesitas. ¿Qué especialidad te interesa?"
        };
    }
}