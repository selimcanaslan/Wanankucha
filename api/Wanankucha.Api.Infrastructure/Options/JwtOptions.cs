namespace Wanankucha.Api.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Token";

    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string SecurityKey { get; set; } = string.Empty;
    public int Expiration { get; set; }
}
