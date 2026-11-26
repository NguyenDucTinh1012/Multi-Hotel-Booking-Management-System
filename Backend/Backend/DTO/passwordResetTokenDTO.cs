namespace Backend.DTO
{
    // Khong dua TokenHash ra response API.
    public class PasswordResetTokenResponseDTO
    {
        public long TokenId { get; set; }
        public int AccountId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RequestPasswordResetDTO
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

}
