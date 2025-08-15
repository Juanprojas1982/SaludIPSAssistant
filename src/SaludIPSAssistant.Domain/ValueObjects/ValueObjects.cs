namespace SaludIPSAssistant.Domain.ValueObjects;

public record PhoneNumber
{
    public string Value { get; private set; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty", nameof(value));
        
        // Basic validation - should include country code format
        var cleaned = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        if (!cleaned.StartsWith("+") || cleaned.Length < 10)
            throw new ArgumentException("Phone number must be in international format", nameof(value));

        Value = cleaned;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    public static implicit operator PhoneNumber(string value) => new(value);
}

public record AppointmentSlot
{
    public string Id { get; private set; }
    public DateTime Date { get; private set; }
    public TimeSpan Time { get; private set; }
    public string DoctorId { get; private set; }
    public string DoctorName { get; private set; }
    public string SpecialtyId { get; private set; }

    public AppointmentSlot(string id, DateTime date, TimeSpan time, string doctorId, string doctorName, string specialtyId)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Date = date;
        Time = time;
        DoctorId = doctorId ?? throw new ArgumentNullException(nameof(doctorId));
        DoctorName = doctorName ?? throw new ArgumentNullException(nameof(doctorName));
        SpecialtyId = specialtyId ?? throw new ArgumentNullException(nameof(specialtyId));
    }
}