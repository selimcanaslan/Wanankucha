namespace Wanankucha.Api.Domain.Entities;

public class User : Common.BaseEntity<Guid>
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public string UserName { get; private set; } = string.Empty;
    public string NormalizedUserName { get; private set; } = string.Empty;
    public ValueObjects.Email Email { get; private set; } = null!;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string NameSurname { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenEndDate { get; private set; }

    // Password Reset
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiry { get; private set; }

    // Account Lockout
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool LockoutEnabled { get; private set; } = true;

    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    // ─── Factory ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a new User. Use this instead of calling the constructor directly.
    /// </summary>
    public static User Create(string nameSurname, string email, string userName, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameSurname);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var emailObject = ValueObjects.Email.Create(email);

        return new User
        {
            Id = Guid.NewGuid(),
            NameSurname = nameSurname,
            Email = emailObject,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            PasswordHash = passwordHash
        };
    }

    // Required by EF Core
    private User() { }

    // ─── Domain Behaviours ──────────────────────────────────────────────────────

    public bool IsLockedOut()
        => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (LockoutEnabled && FailedLoginAttempts >= MaxFailedAttempts)
        {
            LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
            AddDomainEvent(new Wanankucha.Api.Domain.Events.UserLockedOutDomainEvent(Id, Email));
        }
    }

    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    public void SetPassword(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    public void SetRefreshToken(string token, DateTime expiry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        RefreshToken = token;
        RefreshTokenEndDate = expiry;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenEndDate = null;
    }

    public void GeneratePasswordResetToken()
    {
        PasswordResetToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
    }

    public bool IsPasswordResetTokenValid(string token)
        => PasswordResetToken == token
           && PasswordResetTokenExpiry.HasValue
           && PasswordResetTokenExpiry > DateTime.UtcNow;

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
    }
}
