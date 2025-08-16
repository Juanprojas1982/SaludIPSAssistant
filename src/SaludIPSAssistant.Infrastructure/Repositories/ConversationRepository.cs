using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

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