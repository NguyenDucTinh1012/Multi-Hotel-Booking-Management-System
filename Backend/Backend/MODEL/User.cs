namespace Backend.MODEL
{

    public class User
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

        public Account? Account { get; set; }
    }

}
