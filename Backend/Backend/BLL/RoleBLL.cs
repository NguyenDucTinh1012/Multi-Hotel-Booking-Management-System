using Backend.DAL;
using Backend.DTO;
using Backend.MODEL;

namespace Backend.BLL
{
    public class RoleBLL
    {
        private readonly RoleDAL _roleDAL;

        public RoleBLL(RoleDAL roleDAL)
        {
            _roleDAL = roleDAL;
        }

        public async Task<List<RoleResponseDTO>> GetAllAsync()
        {
            var roles = await _roleDAL.GetAllAsync();
            return roles.Select(ToDTO).ToList();
        }

        public async Task<RoleResponseDTO?> GetByIdAsync(int id)
        {
            var role = await _roleDAL.GetByIdAsync(id);
            return role is null ? null : ToDTO(role);
        }

        public async Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RoleName))
                throw new ArgumentException(
                    "Tên vai trò không được để trống.");

            var name = dto.RoleName.Trim();

            if (name.Length > 50)
                throw new ArgumentException(
                    "Tên vai trò không được vượt quá 50 ký tự.");

            if (await _roleDAL.ExistsByNameAsync(name))
                throw new InvalidOperationException(
                    "Tên vai trò đã tồn tại.");

            var role = new Role
            {
                RoleName = name,
                Description = dto.Description?.Trim()
            };

            await _roleDAL.AddAsync(role);

            return ToDTO(role);
        }

        public async Task<bool> UpdateAsync(int id, UpdateRoleDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RoleName))
                throw new ArgumentException(
                    "Tên vai trò không được để trống.");

            var role = await _roleDAL.GetByIdAsync(id);

            if (role is null)
                return false;

            var name = dto.RoleName.Trim();

            if (name.Length > 50)
                throw new ArgumentException(
                    "Tên vai trò không được vượt quá 50 ký tự.");

            if (!string.Equals(
                role.RoleName,
                name,
                StringComparison.OrdinalIgnoreCase)
                && await _roleDAL.ExistsByNameAsync(name))
            {
                throw new InvalidOperationException(
                    "Tên vai trò đã tồn tại.");
            }

            role.RoleName = name;
            role.Description = dto.Description?.Trim();

            await _roleDAL.UpdateAsync(role);

            return true;
        }
        public Task<bool> DeleteAsync(int id) => _roleDAL.DeleteAsync(id);

        private static RoleResponseDTO ToDTO(Role role) => new()
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }
}
