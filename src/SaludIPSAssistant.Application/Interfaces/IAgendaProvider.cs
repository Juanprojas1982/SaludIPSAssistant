using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Domain.ValueObjects;

namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Interface for external agenda/appointment API integration
/// </summary>
public interface IAgendaProvider
{
    Task<IEnumerable<AppointmentSlot>> GetAvailableSlotsAsync(string specialtyId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<bool> UpdateAppointmentStatusAsync(string appointmentId, string status, CancellationToken cancellationToken = default);
    Task<bool> MoveAppointmentAsync(string fromSlotId, string toSlotId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyAsync(string specialtyId, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
}