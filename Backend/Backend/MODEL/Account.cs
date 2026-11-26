using System.Data;

namespace Backend.MODEL
{
    public class Account
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;

        public string? VerificationTokenHash { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Role? Role { get; set; }
        public User? User { get; set; }
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
            = new List<PasswordResetToken>();
        public int VerificationOtpAttempts { get; set; } = 0;
    }
}
