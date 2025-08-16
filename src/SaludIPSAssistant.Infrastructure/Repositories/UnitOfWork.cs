using Microsoft.EntityFrameworkCore.Storage;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Infrastructure.Persistence;

namespace SaludIPSAssistant.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        
        Patients = new PatientRepository(_context);
        Conversations = new ConversationRepository(_context);
        Appointments = new AppointmentRepository(_context);
        Reminders = new AppointmentReminderRepository(_context);
        Messages = new MessageRepository(_context);
    }

    public IPatientRepository Patients { get; }
    public IConversationRepository Conversations { get; }
    public IAppointmentRepository Appointments { get; }
    public IAppointmentReminderRepository Reminders { get; }
    public IMessageRepository Messages { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}