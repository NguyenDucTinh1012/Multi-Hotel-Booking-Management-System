using Backend.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailTestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailTestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(
            [FromBody] SendTestEmailDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ToEmail))
            {
                return BadRequest(new
                {
                    message = "Email người nhận không được để trống."
                });
            }

            try
            {
                await _emailService.SendEmailAsync(
                    dto.ToEmail,
                    "Test Email - Multi Hotel Booking System",
                    """
                    <h2>Xin chào!</h2>
                    <p>Đây là email kiểm tra từ hệ thống Multi Hotel Booking.</p>
                    <p>Chức năng gửi email SMTP đang hoạt động.</p>
                    """
                );

                return Ok(new
                {
                    message = "Gửi email thành công."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Không thể gửi email. Vui lòng kiểm tra cấu hình SMTP."
                });
            }
        }
    }

    public class SendTestEmailDTO
    {
        public string ToEmail { get; set; } = string.Empty;
    }
}
