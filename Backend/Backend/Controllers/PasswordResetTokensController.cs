using Backend.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.DTO;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetTokensController : ControllerBase
    {
        private readonly PasswordResetTokenBLL _tokenBLL;

        public PasswordResetTokensController(PasswordResetTokenBLL tokenBLL)
        {
            _tokenBLL = tokenBLL;
        }

        // GET: api/passwordresettokens/by-account/1
        // Administrative/history endpoint. Secure with Admin authorization before deployment.
        [HttpGet("by-account/{accountId:int}")]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            var tokens = await _tokenBLL.GetByAccountIdAsync(accountId);
            return Ok(tokens);
        }

        // POST: api/passwordresettokens/validate
        // Body: { "token": "raw-token-from-email-link" }
        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateResetTokenDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest(new { valid = false, message = "Token không được để trống." });

            var valid = await _tokenBLL.IsTokenValidAsync(dto.Token);
            return Ok(new
            {
                valid,
                message = valid ? "Token hợp lệ." : "Token không hợp lệ, đã hết hạn hoặc đã được sử dụng."
            });
        }

        // POST: api/passwordresettokens/consume
        // This marks a token as used; it does NOT change the account password.
        // A complete reset-password workflow must validate token, update PasswordHash,
        // and mark token used atomically in one database transaction.
        [HttpPost("consume")]
        public async Task<IActionResult> Consume([FromBody] ValidateResetTokenDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest(new { message = "Token không được để trống." });

            var consumed = await _tokenBLL.MarkTokenAsUsedAsync(dto.Token);
            return consumed
                ? Ok(new { message = "Token đã được đánh dấu sử dụng." })
                : BadRequest(new { message = "Token không hợp lệ, đã hết hạn hoặc đã được sử dụng." });
        }
    }
}
