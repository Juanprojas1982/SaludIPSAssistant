using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Domain.Entities;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);
    }
}