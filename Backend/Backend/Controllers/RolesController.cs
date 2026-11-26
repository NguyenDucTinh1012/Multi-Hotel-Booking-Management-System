using Backend.BLL;
using Backend.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleBLL _roleBLL;

        public RolesController(RoleBLL roleBLL)
        {
            _roleBLL = roleBLL;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _roleBLL.GetAllAsync());

        // GET: api/roles/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleBLL.GetByIdAsync(id);
            return role is null ? NotFound(new { message = "Không tìm thấy vai trò." }) : Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDTO dto)
        {
            try
            {
                var role = await _roleBLL.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = role.RoleId }, role);
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

        // PUT: api/roles/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDTO dto)
        {
            try
            {
                var updated = await _roleBLL.UpdateAsync(id, dto);
                return updated ? NoContent() : NotFound(new { message = "Không tìm thấy vai trò." });
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

        // DELETE: api/roles/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _roleBLL.DeleteAsync(id);
                return deleted ? NoContent() : NotFound(new { message = "Không tìm thấy vai trò." });
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return Conflict(new { message = "Không thể xóa vai trò đang được tài khoản sử dụng." });
            }
        }

    }
}
