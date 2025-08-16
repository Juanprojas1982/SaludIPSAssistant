using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

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