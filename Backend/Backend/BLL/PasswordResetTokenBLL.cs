using Backend.DAL;
using Backend.DTO;
using Backend.MODEL;
using System.Security.Cryptography;
using System.Text;

namespace Backend.BLL
{
    public class PasswordResetTokenBLL
    {
        private readonly PasswordResetTokenDAL _tokenDAL;
        private readonly AccountDAL _accountDAL;

        public PasswordResetTokenBLL(
            PasswordResetTokenDAL tokenDAL,
            AccountDAL accountDAL)
        {
            _tokenDAL = tokenDAL;
            _accountDAL = accountDAL;
        }

        // Tra ve token goc de tang email. Chi luu hash trong database.
        // Controller/Email service khong nen ghi token goc vao log.
        public async Task<string> CreateTokenAsync(int accountId)
        {
            var account = await _accountDAL.GetByIdAsync(accountId);
            if (account is null)
                throw new ArgumentException("Tài khoản không tồn tại.");

            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var token = new PasswordResetToken
            {
                AccountId = accountId,
                TokenHash = HashToken(rawToken),
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                CreatedAt = DateTime.UtcNow
            };

            await _tokenDAL.AddAsync(token);
            return rawToken;
        }

        public async Task<bool> IsTokenValidAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken)) return false;

            var tokenHash = HashToken(rawToken);
            return await _tokenDAL.GetValidByHashAsync(tokenHash) is not null;
        }

        // Sau khi xac thuc token, BLL danh dau token da su dung.
        // Viec doi PasswordHash can duoc thuc hien trong mot luong reset password
        // co transaction va PasswordHasher, khong chi goi ham nay rieng le.
        public async Task<bool> MarkTokenAsUsedAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken)) return false;

            var token = await _tokenDAL.GetValidByHashAsync(HashToken(rawToken));
            if (token is null) return false;

            await _tokenDAL.MarkAsUsedAsync(token);
            return true;
        }

        public async Task<List<PasswordResetTokenResponseDTO>> GetByAccountIdAsync(int accountId)
        {
            var tokens = await _tokenDAL.GetByAccountIdAsync(accountId);
            return tokens.Select(x => new PasswordResetTokenResponseDTO
            {
                TokenId = x.TokenId,
                AccountId = x.AccountId,
                ExpiresAt = x.ExpiresAt,
                UsedAt = x.UsedAt,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        private static string HashToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes);
        }
    }
}
