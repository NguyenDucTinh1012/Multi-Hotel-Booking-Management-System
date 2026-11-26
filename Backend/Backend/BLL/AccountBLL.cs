using Backend.DAL;
using Backend.DTO;
using Backend.MODEL;
using Microsoft.AspNetCore.Identity;

namespace Backend.BLL
{
    public class AccountBLL
    {
        private readonly AccountDAL _accountDAL;
        private readonly RoleDAL _roleDAL;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public AccountBLL(
            AccountDAL accountDAL,
            RoleDAL roleDAL,
            IPasswordHasher<Account> passwordHasher)
        {
            _accountDAL = accountDAL;
            _roleDAL = roleDAL;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<AccountResponseDTO>> GetAllAsync()
        {
            var accounts = await _accountDAL.GetAllAsync();
            return accounts.Select(ToDTO).ToList();
        }

        public async Task<AccountResponseDTO?> GetByIdAsync(int id)
        {
            var account = await _accountDAL.GetByIdAsync(id);
            return account is null ? null : ToDTO(account);
        }

        public async Task<AccountResponseDTO> RegisterAsync(RegisterAccountDTO dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email không được để trống.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
                throw new ArgumentException("Mật khẩu phải có ít nhất 8 ký tự.");

            if (await _accountDAL.ExistsByEmailAsync(email))
                throw new InvalidOperationException("Email đã được sử dụng.");

            var role = await _roleDAL.GetByIdAsync(dto.RoleId);
            if (role is null)
                throw new ArgumentException("RoleId không tồn tại.");

            var account = new Account
            {
                RoleId = dto.RoleId,
                Email = email,
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            account.PasswordHash = _passwordHasher.HashPassword(account, dto.Password);

            await _accountDAL.AddAsync(account);
            account.Role = role;

            return ToDTO(account);
        }

        public async Task<bool> UpdateStatusAsync(int id, UpdateAccountStatusDTO dto)
        {
            var account = await _accountDAL.GetByIdAsync(id);
            if (account is null) return false;

            account.IsActive = dto.IsActive;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountDAL.UpdateAsync(account);
            return true;
        }

        private static AccountResponseDTO ToDTO(Account account) => new()
        {
            AccountId = account.AccountId,
            RoleId = account.RoleId,
            RoleName = account.Role?.RoleName,
            Email = account.Email,
            IsActive = account.IsActive,
            IsEmailVerified = account.IsEmailVerified,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }
}
