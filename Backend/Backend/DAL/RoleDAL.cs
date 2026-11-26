using Backend.MODEL;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class RoleDAL
    { private readonly connect _context;

        public RoleDAL(connect context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetAllAsync()
            => await _context.Roles.AsNoTracking().OrderBy(x => x.RoleId).ToListAsync();

        public async Task<Role?> GetByIdAsync(int id)
            => await _context.Roles.FindAsync(id);

        public async Task<bool> ExistsByNameAsync(string roleName)
            => await _context.Roles.AnyAsync(x => x.RoleName == roleName);

        public async Task AddAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role is null) return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
