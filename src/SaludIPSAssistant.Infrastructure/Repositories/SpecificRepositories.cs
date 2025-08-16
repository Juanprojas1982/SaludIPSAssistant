using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<Patient?> GetByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber, cancellationToken);
    }
}

public class ConversationRepository : Repository<Conversation>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Conversation?> GetActiveConversationByPatientAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.PatientId == patientId && c.Status == Domain.Enums.ConversationStatus.Active, cancellationToken);
    }

    public async Task<Conversation?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetConversationHistoryAsync(Guid patientId, int take = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.StartedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}

public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsForRemindersAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.AppointmentDate >= fromDate && a.AppointmentDate <= toDate)
            .Where(a => a.Status == Domain.Enums.AppointmentStatus.Asignado || a.Status == Domain.Enums.AppointmentStatus.ConfirmaAsistencia)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.ExternalId == externalId, cancellationToken);
    }
}

public class AppointmentReminderRepository : Repository<AppointmentReminder>, IAppointmentReminderRepository
{
    public AppointmentReminderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync(DateTime upToDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Doctor)
            .Where(r => !r.IsSent && r.ScheduledFor <= upToDate)
            .ToListAsync(cancellationToken);
    }
}