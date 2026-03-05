using System.Text.RegularExpressions;
using Wanankucha.Api.Domain.Common;
using Wanankucha.Api.Domain.Exceptions;

namespace Wanankucha.Api.Domain.ValueObjects;

public class Email : ValueObject
{
    private const string EmailRegexPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
    private static readonly Regex EmailRegex = new(EmailRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string Value { get; private set; }

    // Required by EF Core
    private Email()
    {
        Value = string.Empty;
    }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email value cannot be empty.");

        value = value.Trim();

        if (value.Length > 256)
            throw new DomainException("Email is too long.");

        if (!EmailRegex.IsMatch(value))
            throw new DomainException("Email format is invalid.");

        Value = value.ToLowerInvariant();
    }

    public static Email Create(string value)
    {
        return new Email(value);
    }

    public static Result<Email> TryCreate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure(Error.Validation("Email.Empty", "Email value cannot be empty."));

        value = value.Trim();

        if (value.Length > 256)
            return Result<Email>.Failure(Error.Validation("Email.TooLong", "Email is too long."));

        if (!EmailRegex.IsMatch(value))
            return Result<Email>.Failure(Error.Validation("Email.InvalidFormat", "Email format is invalid."));

        return Result<Email>.Success(new Email(value.ToLowerInvariant()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
