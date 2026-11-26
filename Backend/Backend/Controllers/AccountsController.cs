using Backend.BLL;
using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountBLL _accountBLL;
        private readonly RoleBLL _roleBLL;
        private readonly EmailVerificationService _emailVerificationService;

        public AccountsController(
      AccountBLL accountBLL,
      RoleBLL roleBLL,
      EmailVerificationService emailVerificationService)
        {
            _accountBLL = accountBLL;
            _roleBLL = roleBLL;
            _emailVerificationService = emailVerificationService;
        }

        // GET: api/accounts
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _accountBLL.GetAllAsync());

        // GET: api/accounts/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountBLL.GetByIdAsync(id);
            return account is null
                ? NotFound(new { message = "Không tìm thấy tài khoản." })
                : Ok(account);
        }

        // POST: api/accounts/register
        // Public registration is always assigned Customer role.
        // Do not accept RoleId from an anonymous client.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountDTO dto)
        {
            try
            {
                var customerRole = (await _roleBLL.GetAllAsync())
                    .FirstOrDefault(x => x.RoleName == "Customer");

                if (customerRole is null)
                    return StatusCode(500, new { message = "Chưa cấu hình vai trò Customer." });

                dto.RoleId = customerRole.RoleId;

                var account = await _accountBLL.RegisterAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = account.AccountId }, account);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PATCH: api/accounts/1/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateAccountStatusDTO dto)
        {
            var updated = await _accountBLL.UpdateStatusAsync(id, dto);
            return updated
                ? NoContent()
                : NotFound(new { message = "Không tìm thấy tài khoản." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(
    [FromBody] VerifyEmailDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Otp))
            {
                return BadRequest(new
                {
                    message = "Email và OTP không được để trống."
                });
            }

            bool result = await _emailVerificationService.VerifyOtpAsync(
                dto.Email,
                dto.Otp
            );

            if (!result)
            {
                return BadRequest(new
                {
                    message = "OTP không hợp lệ, đã hết hạn hoặc vượt quá số lần thử."
                });
            }

            return Ok(new
            {
                message = "Xác minh email thành công."
            });
        }
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(
     [FromBody] ResendOtpDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new
                {
                    message = "Email không được để trống."
                });
            }

            await _emailVerificationService.ResendOtpAsync(dto.Email);

            return Ok(new
            {
                message = "Nếu tài khoản đủ điều kiện, OTP sẽ được gửi."
            });
        }
    }
}
