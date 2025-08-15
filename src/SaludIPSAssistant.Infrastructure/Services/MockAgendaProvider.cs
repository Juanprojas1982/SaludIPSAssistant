using Microsoft.Extensions.Configuration;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Domain.ValueObjects;

namespace SaludIPSAssistant.Infrastructure.Services;

public class MockAgendaProvider : IAgendaProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public MockAgendaProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsAsync(string specialtyId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        // TODO: Replace with actual API integration
        // This is a mock implementation for development/testing
        
        await Task.Delay(500, cancellationToken); // Simulate API call

        var slots = new List<AppointmentSlot>();
        var random = new Random();
        
        // Generate some mock available slots
        for (int i = 1; i <= 5; i++)
        {
            var date = fromDate.AddDays(random.Next(1, 14));
            var hour = random.Next(8, 17); // 8 AM to 5 PM
            var time = new TimeSpan(hour, 0, 0);
            
            slots.Add(new AppointmentSlot(
                id: $"slot_{specialtyId}_{i}",
                date: date,
                time: time,
                doctorId: $"doctor_{i}",
                doctorName: $"Dr. {GetRandomDoctorName()}",
                specialtyId: specialtyId
            ));
        }

        return slots;
    }

    public async Task<bool> UpdateAppointmentStatusAsync(string appointmentId, string status, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call to external system
        
        await Task.Delay(200, cancellationToken);
        Console.WriteLine($"[External API] Updated appointment {appointmentId} to status {status}");
        
        return true;
    }

    public async Task<bool> MoveAppointmentAsync(string fromSlotId, string toSlotId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call
        
        await Task.Delay(300, cancellationToken);
        Console.WriteLine($"[External API] Moved appointment from {fromSlotId} to {toSlotId}");
        
        return true;
    }

    public async Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialtyId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call
        
        await Task.Delay(200, cancellationToken);
        
        var doctors = new List<Doctor>
        {
            new Doctor
            {
                Id = Guid.NewGuid(),
                Name = "Dr. María González",
                SpecialtyId = specialtyId,
                SpecialtyName = GetSpecialtyName(specialtyId),
                LicenseNumber = "12345",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Doctor
            {
                Id = Guid.NewGuid(),
                Name = "Dr. Carlos Rodríguez",
                SpecialtyId = specialtyId,
                SpecialtyName = GetSpecialtyName(specialtyId),
                LicenseNumber = "67890",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        return doctors;
    }

    public async Task<Patient?> GetPatientByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call
        
        await Task.Delay(200, cancellationToken);
        
        // Return null for now - in real implementation, this would fetch from external system
        return null;
    }

    private static string GetRandomDoctorName()
    {
        var names = new[] { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez" };
        var firstNames = new[] { "María", "José", "Antonio", "Francisco", "Manuel", "David", "Carmen", "Ana" };
        
        var random = new Random();
        return $"{firstNames[random.Next(firstNames.Length)]} {names[random.Next(names.Length)]}";
    }

    private static string GetSpecialtyName(string specialtyId)
    {
        return specialtyId.ToLower() switch
        {
            "general" => "Medicina General",
            "cardiology" => "Cardiología",
            "dermatology" => "Dermatología",
            "gynecology" => "Ginecología",
            "pediatrics" => "Pediatría",
            _ => "Especialidad Médica"
        };
    }
}