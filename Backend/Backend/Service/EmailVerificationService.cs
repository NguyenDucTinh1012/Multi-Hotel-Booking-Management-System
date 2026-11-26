using Backend.DAL;
using Backend.MODEL;

namespace Backend.Service
{
    public class EmailVerificationService
    {
        private readonly AccountDAL _accountDAL;
        private readonly OtpService _otpService;
        private readonly IEmailService _emailService;

        public EmailVerificationService(
            AccountDAL accountDAL,
            OtpService otpService,
            IEmailService emailService)
        {
            _accountDAL = accountDAL;
            _otpService = otpService;
            _emailService = emailService;
        }

        // Gửi OTP xác minh email
        public async Task SendOtpAsync(Account account)
        {
            if (account.IsEmailVerified)
            {
                throw new InvalidOperationException(
                    "Email đã được xác minh."
                );
            }

            string otp = _otpService.GenerateOtp();

            account.VerificationTokenHash =
                _otpService.HashOtp(account.AccountId, otp);

            account.VerificationTokenExpiry =
                DateTime.UtcNow.AddMinutes(5);

            account.VerificationOtpAttempts = 0;

            await _accountDAL.UpdateAsync(account);

            string subject = "Xác minh tài khoản Multi-Hotel Booking";

            string body = $"""
                <div style="font-family:Arial,sans-serif;
                            max-width:600px;margin:auto;padding:24px;">
                    <h2>Xác minh tài khoản</h2>

                    <p>Xin chào,</p>

                    <p>
                        Đây là mã OTP để xác minh email của bạn:
                    </p>

                    <h1 style="letter-spacing:8px;color:#2563eb;">
                        {otp}
                    </h1>

                    <p>
                        Mã OTP có hiệu lực trong 5 phút.
                    </p>

                    <p>
                        Không chia sẻ mã này với bất kỳ ai.
                    </p>

                    <p>Multi-Hotel Booking System</p>
                </div>
                """;

            await _emailService.SendEmailAsync(
                account.Email,
                subject,
                body
            );
        }

        // Xác minh OTP
        public async Task<bool> VerifyOtpAsync(
            string email,
            string otp)
        {
            var account = await _accountDAL.GetByEmailAsync(
                email.Trim().ToLowerInvariant()
            );

            if (account == null)
                return false;

            if (account.IsEmailVerified)
                return true;

            if (string.IsNullOrWhiteSpace(
                    account.VerificationTokenHash) ||
                account.VerificationTokenExpiry == null)
            {
                return false;
            }

            if (account.VerificationTokenExpiry <= DateTime.UtcNow)
            {
                ClearOtp(account);

                await _accountDAL.UpdateAsync(account);

                return false;
            }

            if (account.VerificationOtpAttempts >= 5)
                return false;

            bool isValid = _otpService.VerifyOtp(
                account.AccountId,
                otp,
                account.VerificationTokenHash
            );

            if (!isValid)
            {
                account.VerificationOtpAttempts++;

                if (account.VerificationOtpAttempts >= 5)
                {
                    ClearOtp(account);
                }

                await _accountDAL.UpdateAsync(account);

                return false;
            }

            // Xác minh thành công
            account.IsEmailVerified = true;

            ClearOtp(account);

            await _accountDAL.UpdateAsync(account);

            return true;
        }
        public async Task ResendOtpAsync(string email)
        {
            email = email.Trim().ToLower();

            var account = await _accountDAL.GetByEmailAsync(email);

            // Không tìm thấy tài khoản hoặc email đã xác minh
            // thì không gửi OTP.
            if (account == null || account.IsEmailVerified || !account.IsActive)
            {
                return;
            }

            await SendOtpAsync(account);
        }
        private static void ClearOtp(Account account)
        {
            account.VerificationTokenHash = null;
            account.VerificationTokenExpiry = null;
            account.VerificationOtpAttempts = 0;
        }
    }
}
