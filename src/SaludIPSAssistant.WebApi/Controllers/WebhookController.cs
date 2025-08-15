using Microsoft.AspNetCore.Mvc;
using MediatR;
using SaludIPSAssistant.Application.Commands;
using SaludIPSAssistant.Application.DTOs;

namespace SaludIPSAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IMediator mediator, ILogger<WebhookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// WhatsApp webhook endpoint for receiving incoming messages
    /// </summary>
    [HttpPost("whatsapp")]
    public async Task<IActionResult> ReceiveWhatsAppMessage([FromBody] WhatsAppWebhookRequest request)
    {
        try
        {
            _logger.LogInformation("Received WhatsApp webhook message from {PhoneNumber}", request.From);

            var messageDto = new IncomingMessageDto
            {
                PhoneNumber = request.From,
                Message = request.Text ?? string.Empty,
                MessageId = request.MessageId,
                Timestamp = DateTime.UtcNow
            };

            var command = new ProcessIncomingMessageCommand(messageDto);
            var result = await _mediator.Send(command);

            if (result)
            {
                return Ok(new { status = "success", message = "Message processed successfully" });
            }
            else
            {
                return BadRequest(new { status = "error", message = "Failed to process message" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WhatsApp webhook message");
            return StatusCode(500, new { status = "error", message = "Internal server error" });
        }
    }

    /// <summary>
    /// WhatsApp webhook verification endpoint
    /// </summary>
    [HttpGet("whatsapp")]
    public IActionResult VerifyWebhook([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
    {
        const string expectedToken = "your_verify_token_here"; // Should come from configuration
        
        if (hub_mode == "subscribe" && hub_verify_token == expectedToken)
        {
            _logger.LogInformation("WhatsApp webhook verified successfully");
            return Ok(hub_challenge);
        }
        
        _logger.LogWarning("WhatsApp webhook verification failed");
        return Forbid();
    }
}

/// <summary>
/// DTO for WhatsApp webhook requests
/// </summary>
public class WhatsAppWebhookRequest
{
    public string From { get; set; } = string.Empty;
    public string? Text { get; set; }
    public string? MessageId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}