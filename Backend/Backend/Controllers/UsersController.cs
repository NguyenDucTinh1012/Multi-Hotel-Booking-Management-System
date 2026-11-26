using Backend.BLL;
using Backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserBLL _userBLL;

        public UsersController(UserBLL userBLL)
        {
            _userBLL = userBLL;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _userBLL.GetAllAsync());

        // GET: api/users/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userBLL.GetByIdAsync(id);
            return user is null
                ? NotFound(new { message = "Không tìm thấy hồ sơ người dùng." })
                : Ok(user);
        }

        // GET: api/users/by-account/1
        [HttpGet("by-account/{accountId:int}")]
        public async Task<IActionResult> GetByAccountId(int accountId)
        {
            var user = await _userBLL.GetByAccountIdAsync(accountId);
            return user is null
                ? NotFound(new { message = "Tài khoản chưa có hồ sơ người dùng." })
                : Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                var user = await _userBLL.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
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

        // PUT: api/users/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                var updated = await _userBLL.UpdateAsync(id, dto);
                return updated
                    ? NoContent()
                    : NotFound(new { message = "Không tìm thấy hồ sơ người dùng." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/users/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userBLL.DeleteAsync(id);
            return deleted
                ? NoContent()
                : NotFound(new { message = "Không tìm thấy hồ sơ người dùng." });
        }
    }
}
