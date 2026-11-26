namespace Backend.DTO
{
    public class CreateRoleDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleDTO
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class RoleResponseDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
