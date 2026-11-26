using Backend.MODEL;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class UserDAL
    {
        private readonly connect _context;

        public UserDAL(connect context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
            => await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.UserId)
                .ToListAsync();

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.UserId == id);

        public async Task<User?> GetByAccountIdAsync(int accountId)
            => await _context.Users
                .FirstOrDefaultAsync(x => x.AccountId == accountId);

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
