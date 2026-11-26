using Backend.DAL;
using Backend.DTO;
using Backend.MODEL;

namespace Backend.BLL
{
    public class UserBLL
    {
        private readonly UserDAL _userDAL;
        private readonly AccountDAL _accountDAL;

        public UserBLL(UserDAL userDAL, AccountDAL accountDAL)
        {
            _userDAL = userDAL;
            _accountDAL = accountDAL;
        }

        public async Task<List<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userDAL.GetAllAsync();
            return users.Select(ToDTO).ToList();
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            var user = await _userDAL.GetByIdAsync(id);
            return user is null ? null : ToDTO(user);
        }

        public async Task<UserResponseDTO?> GetByAccountIdAsync(int accountId)
        {
            var user = await _userDAL.GetByAccountIdAsync(accountId);
            return user is null ? null : ToDTO(user);
        }

        public async Task<UserResponseDTO> CreateAsync(CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Họ tên không được để trống.");

            if (await _accountDAL.GetByIdAsync(dto.AccountId) is null)
                throw new ArgumentException("Tài khoản không tồn tại.");

            if (await _userDAL.GetByAccountIdAsync(dto.AccountId) is not null)
                throw new InvalidOperationException("Tài khoản đã có hồ sơ người dùng.");

            ValidateGender(dto.Gender);

            var user = new User
            {
                AccountId = dto.AccountId,
                FullName = dto.FullName.Trim(),
                Phone = dto.Phone?.Trim(),
                Avatar = dto.Avatar?.Trim(),
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address?.Trim()
            };

            await _userDAL.AddAsync(user);
            return ToDTO(user);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDTO dto)
        {
            var user = await _userDAL.GetByIdAsync(id);
            if (user is null) return false;

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Họ tên không được để trống.");

            ValidateGender(dto.Gender);

            user.FullName = dto.FullName.Trim();
            user.Phone = dto.Phone?.Trim();
            user.Avatar = dto.Avatar?.Trim();
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.Address = dto.Address?.Trim();

            await _userDAL.UpdateAsync(user);
            return true;
        }

        public Task<bool> DeleteAsync(int id) => _userDAL.DeleteAsync(id);

        private static void ValidateGender(string? gender)
        {
            if (gender is not null &&
                gender is not ("Nam" or "Nữ" or "Khác"))
                throw new ArgumentException("Gender chỉ nhận Nam, Nữ hoặc Khác.");
        }

        private static UserResponseDTO ToDTO(User user) => new()
        {
            UserId = user.UserId,
            AccountId = user.AccountId,
            FullName = user.FullName,
            Phone = user.Phone,
            Avatar = user.Avatar,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address
        };
    }
}
