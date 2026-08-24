namespace DengeWeb.Models;

public class AdminUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
}