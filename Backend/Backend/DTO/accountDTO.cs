namespace Backend.DTO
{
    public class RegisterAccountDTO
    {
        public int RoleId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateAccountStatusDTO
    {
        public bool IsActive { get; set; }
    }

    public class AccountResponseDTO
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
