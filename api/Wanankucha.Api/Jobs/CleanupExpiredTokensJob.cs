using Wanankucha.Api.Persistence.Contexts;

namespace Wanankucha.Api.Jobs;

/// <summary>
/// Background job to clean up expired password reset tokens and refresh tokens
/// </summary>
public class CleanupExpiredTokensJob(
    AppDbContext context,
    ILogger<CleanupExpiredTokensJob> logger)
{
    public async Task ExecuteAsync()
    {
        logger.LogInformation("Starting expired tokens cleanup job");

        var now = DateTime.UtcNow;
        var expiredPasswordResetCount = 0;
        var expiredRefreshTokenCount = 0;

        // Clean up expired password reset tokens
        var usersWithExpiredResetTokens = context.Users
            .Where(u => u.PasswordResetTokenExpiry != null && u.PasswordResetTokenExpiry < now)
            .ToList();

        foreach (var user in usersWithExpiredResetTokens)
        {
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            expiredPasswordResetCount++;
        }

        // Clean up expired refresh tokens
        var usersWithExpiredRefreshTokens = context.Users
            .Where(u => u.RefreshTokenEndDate != null && u.RefreshTokenEndDate < now)
            .ToList();

        foreach (var user in usersWithExpiredRefreshTokens)
        {
            user.RefreshToken = null;
            user.RefreshTokenEndDate = null;
            expiredRefreshTokenCount++;
        }

        if (expiredPasswordResetCount > 0 || expiredRefreshTokenCount > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation(
                "Cleanup completed: {PasswordResetCount} expired password reset tokens, {RefreshTokenCount} expired refresh tokens removed",
                expiredPasswordResetCount, expiredRefreshTokenCount);
        }
        else
        {
            logger.LogInformation("Cleanup completed: No expired tokens found");
        }
    }
}
