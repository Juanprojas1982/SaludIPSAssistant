using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

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