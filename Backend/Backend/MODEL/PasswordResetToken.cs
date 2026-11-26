namespace Backend.MODEL
{
    public class PasswordResetToken
    {
        public long TokenId { get; set; }
        public int AccountId { get; set; }

        // Chi luu SHA-256 hash cua token, khong luu token goc.
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public Account? Account { get; set; }
    }
}
