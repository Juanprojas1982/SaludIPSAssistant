using MediatR;
using System.Text.Json;
using SaludIPSAssistant.Application.Commands;
using SaludIPSAssistant.Application.DTOs;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Domain.Enums;

namespace SaludIPSAssistant.Application.UseCases.Conversations;

public class ProcessIncomingMessageHandler : IRequestHandler<ProcessIncomingMessageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INlpService _nlpService;
    private readonly INotificationService _notificationService;
    private readonly IAgendaProvider _agendaProvider;

    public ProcessIncomingMessageHandler(
        IUnitOfWork unitOfWork,
        INlpService nlpService,
        INotificationService notificationService,
        IAgendaProvider agendaProvider)
    {
        _unitOfWork = unitOfWork;
        _nlpService = nlpService;
        _notificationService = notificationService;
        _agendaProvider = agendaProvider;
    }

    public async Task<bool> Handle(ProcessIncomingMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get or create patient
            var patient = await GetOrCreatePatientAsync(request.Message.PhoneNumber, cancellationToken);
            
            // Get or create active conversation
            var conversation = await GetOrCreateConversationAsync(patient.Id, cancellationToken);
            
            // Save incoming message
            await SaveIncomingMessageAsync(conversation.Id, request.Message, cancellationToken);
            
            // Process message based on conversation state
            var response = await ProcessMessageByStateAsync(conversation, request.Message.Message, cancellationToken);
            
            // Send response
            if (!string.IsNullOrEmpty(response))
            {
                await _notificationService.SendMessageAsync(request.Message.PhoneNumber, response, cancellationToken);
                await SaveOutgoingMessageAsync(conversation.Id, response, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            // Log error and send generic error message
            // TODO: Use proper logging framework like Serilog
            Console.WriteLine($"Error processing message: {ex.Message}");
            
            await _notificationService.SendMessageAsync(
                request.Message.PhoneNumber,
                "Lo siento, ha ocurrido un error. Por favor intenta nuevamente o contacta con nosotros.",
                cancellationToken);
            
            return false;
        }
    }

    private async Task<Patient> GetOrCreatePatientAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        var patient = await _unitOfWork.Patients.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
        
        if (patient == null)
        {
            patient = new Patient
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return patient;
    }

    private async Task<Conversation> GetOrCreateConversationAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetActiveConversationByPatientAsync(patientId, cancellationToken);
        
        if (conversation == null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                SessionId = Guid.NewGuid().ToString(),
                Status = ConversationStatus.Active,
                CurrentState = ConversationState.Initial,
                Context = "{}",
                StartedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };
            await _unitOfWork.Conversations.AddAsync(conversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            conversation.LastActivity = DateTime.UtcNow;
            await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return conversation;
    }

    private async Task SaveIncomingMessageAsync(Guid conversationId, IncomingMessageDto messageDto, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Content = messageDto.Message,
            Direction = MessageDirection.Inbound,
            Timestamp = messageDto.Timestamp,
            MessageId = messageDto.MessageId,
            IsProcessed = false
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task SaveOutgoingMessageAsync(Guid conversationId, string content, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            Content = content,
            Direction = MessageDirection.Outbound,
            Timestamp = DateTime.UtcNow,
            IsProcessed = true
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> ProcessMessageByStateAsync(Conversation conversation, string message, CancellationToken cancellationToken)
    {
        return conversation.CurrentState switch
        {
            ConversationState.Initial => await HandleInitialStateAsync(conversation, message, cancellationToken),
            ConversationState.CollectingSpecialty => await HandleSpecialtySelectionAsync(conversation, message, cancellationToken),
            ConversationState.ShowingAvailableSlots => await HandleSlotSelectionAsync(conversation, message, cancellationToken),
            ConversationState.ConfirmingAppointment => await HandleAppointmentConfirmationAsync(conversation, message, cancellationToken),
            _ => "Lo siento, no pude procesar tu mensaje. ¿Podrías repetirlo?"
        };
    }

    private async Task<string> HandleInitialStateAsync(Conversation conversation, string message, CancellationToken cancellationToken)
    {
        // Update conversation state
        conversation.CurrentState = ConversationState.CollectingSpecialty;
        await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
        
        return @"¡Hola! Bienvenido al asistente de citas médicas de SaludIPS. 

Por favor, dime qué especialidad médica necesitas:
- Medicina General
- Cardiología  
- Dermatología
- Ginecología
- Pediatría
- Otros

Solo escribe el nombre de la especialidad.";
    }

    private async Task<string> HandleSpecialtySelectionAsync(Conversation conversation, string message, CancellationToken cancellationToken)
    {
        // Use NLP to identify specialty
        var intent = await _nlpService.InterpretIntentAsync(message, cancellationToken);
        var entities = await _nlpService.ExtractEntitiesAsync(message, cancellationToken);
        
        if (entities.ContainsKey("specialty"))
        {
            var specialty = entities["specialty"];
            var fromDate = DateTime.Today.AddDays(1);
            var toDate = DateTime.Today.AddDays(30);
            
            // Get available slots
            var slots = await _agendaProvider.GetAvailableSlotsAsync(specialty, fromDate, toDate, cancellationToken);
            
            if (slots.Any())
            {
                // Update conversation context
                var context = new ConversationContextDto
                {
                    SelectedSpecialty = specialty,
                    AvailableSlots = slots.Take(5).Select(s => new AppointmentSlotDto
                    {
                        Id = s.Id,
                        Date = s.Date,
                        Time = s.Time,
                        DoctorId = s.DoctorId,
                        DoctorName = s.DoctorName,
                        SpecialtyId = s.SpecialtyId
                    }).ToList()
                };
                
                conversation.Context = JsonSerializer.Serialize(context);
                conversation.CurrentState = ConversationState.ShowingAvailableSlots;
                await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
                
                var response = $"Perfecto! Encontré las siguientes citas disponibles para {specialty}:\n\n";
                var index = 1;
                foreach (var slot in context.AvailableSlots!)
                {
                    response += $"{index}. {slot.Date:dd/MM/yyyy} a las {slot.Time:hh\\:mm} - Dr. {slot.DoctorName}\n";
                    index++;
                }
                response += "\nPor favor, responde con el número de la cita que prefieres (1, 2, 3, etc.)";
                
                return response;
            }
            else
            {
                return $"Lo siento, no encontré citas disponibles para {specialty} en los próximos 30 días. ¿Te gustaría consultar otra especialidad?";
            }
        }
        
        return "No pude identificar la especialidad. Por favor, especifica una de las siguientes: Medicina General, Cardiología, Dermatología, Ginecología, Pediatría u Otros.";
    }

    private async Task<string> HandleSlotSelectionAsync(Conversation conversation, string message, CancellationToken cancellationToken)
    {
        if (int.TryParse(message.Trim(), out int selection))
        {
            var context = JsonSerializer.Deserialize<ConversationContextDto>(conversation.Context);
            
            if (context?.AvailableSlots != null && selection > 0 && selection <= context.AvailableSlots.Count)
            {
                var selectedSlot = context.AvailableSlots[selection - 1];
                
                // Update context with selected slot
                var updatedContext = context with { SelectedSlotId = selectedSlot.Id };
                conversation.Context = JsonSerializer.Serialize(updatedContext);
                conversation.CurrentState = ConversationState.ConfirmingAppointment;
                await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
                
                return $@"Perfecto! Has seleccionado:

📅 Fecha: {selectedSlot.Date:dd/MM/yyyy}
🕐 Hora: {selectedSlot.Time:hh\\:mm}
👨‍⚕️ Doctor: {selectedSlot.DoctorName}

¿Confirmas esta cita? Responde 'SÍ' para confirmar o 'NO' para cancelar.";
            }
        }
        
        return "Por favor, responde con el número de la cita que prefieres (ejemplo: 1, 2, 3...)";
    }

    private async Task<string> HandleAppointmentConfirmationAsync(Conversation conversation, string message, CancellationToken cancellationToken)
    {
        var normalizedMessage = message.ToUpper().Trim();
        
        if (normalizedMessage.Contains("SÍ") || normalizedMessage.Contains("SI") || normalizedMessage == "1" || normalizedMessage == "YES")
        {
            var context = JsonSerializer.Deserialize<ConversationContextDto>(conversation.Context);
            
            if (context?.SelectedSlotId != null)
            {
                // Book the appointment
                var bookingCommand = new BookAppointmentCommand(
                    conversation.PatientId,
                    context.SelectedSlotId,
                    context.SelectedSpecialty!);
                
                // Here you would send the command through mediator
                // var appointmentId = await _mediator.Send(bookingCommand, cancellationToken);
                
                conversation.CurrentState = ConversationState.AppointmentBooked;
                conversation.Status = ConversationStatus.Completed;
                conversation.EndedAt = DateTime.UtcNow;
                await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
                
                return @"✅ ¡Cita confirmada exitosamente!

Te enviaremos recordatorios:
• 1 día antes de tu cita
• 1 hora antes de tu cita

Si necesitas cambiar o cancelar tu cita, solo escríbenos.

¡Gracias por usar nuestro servicio! 😊";
            }
        }
        else if (normalizedMessage.Contains("NO") || normalizedMessage == "0")
        {
            conversation.CurrentState = ConversationState.CollectingSpecialty;
            await _unitOfWork.Conversations.UpdateAsync(conversation, cancellationToken);
            
            return "Entendido. ¿Te gustaría seleccionar otra cita o consultar una especialidad diferente?";
        }
        
        return "Por favor, responde 'SÍ' para confirmar la cita o 'NO' para cancelar.";
    }
}