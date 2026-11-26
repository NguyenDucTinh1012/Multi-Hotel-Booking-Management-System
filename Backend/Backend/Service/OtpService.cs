using System.Security.Cryptography;
using System.Text;

namespace Backend.Service
{
    
    public class OtpService
    {
        private readonly string _secretKey;

        public OtpService(IConfiguration configuration)
        {
            _secretKey = configuration["OtpSettings:SecretKey"]
                ?? throw new InvalidOperationException(
                    "Chưa cấu hình OtpSettings:SecretKey."
                );

            if (Convert.FromBase64String(_secretKey).Length < 32)
            {
                throw new InvalidOperationException(
                    "OTP Secret Key phải có ít nhất 32 byte."
                );
            }
        }

        // Sinh OTP ngẫu nhiên gồm 6 chữ số
        public string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(0, 1_000_000)
                .ToString("D6");
        }

        // Hash OTP bằng HMAC-SHA256
        public string HashOtp(int accountId, string otp)
        {
            var key = Convert.FromBase64String(_secretKey);

            var data = $"{accountId}:{otp}";

            using var hmac = new HMACSHA256(key);

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(data)
            );

            return Convert.ToHexString(hash);
        }

        // So sánh hash an toàn
        public bool VerifyOtp(
            int accountId,
            string otp,
            string storedHash)
        {
            var inputHash = Convert.FromHexString(
                HashOtp(accountId, otp)
            );

            var savedHash = Convert.FromHexString(storedHash);

            return CryptographicOperations.FixedTimeEquals(
                inputHash,
                savedHash
            );
        }
    }
}
