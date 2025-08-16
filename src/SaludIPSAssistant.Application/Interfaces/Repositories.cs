using SaludIPSAssistant.Domain.Entities;

namespace SaludIPSAssistant.Application.Interfaces;

/// <summary>
/// Repository interfaces for data persistence
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<Patient?> GetByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
}

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetActiveConversationByPatientAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Conversation>> GetConversationHistoryAsync(Guid patientId, int take = 10, CancellationToken cancellationToken = default);
}

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
}

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAppointmentsForRemindersAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Appointment?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
}

public interface IAppointmentReminderRepository : IRepository<AppointmentReminder>
{
    Task<IEnumerable<AppointmentReminder>> GetPendingRemindersAsync(DateTime upToDate, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of Work pattern for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IConversationRepository Conversations { get; }
    IAppointmentRepository Appointments { get; }
    IAppointmentReminderRepository Reminders { get; }
    IMessageRepository Messages { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}