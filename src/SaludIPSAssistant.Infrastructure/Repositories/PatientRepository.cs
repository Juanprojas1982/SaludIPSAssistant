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