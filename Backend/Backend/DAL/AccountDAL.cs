using Backend.MODEL;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class AccountDAL
    {
        private readonly connect _context;

        public AccountDAL(connect context)
        {
            _context = context;
        }

        public async Task<List<Account>> GetAllAsync()
            => await _context.Accounts
                .AsNoTracking()
                .Include(x => x.Role)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<Account?> GetByIdAsync(int id)
            => await _context.Accounts
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.AccountId == id);

        public async Task<Account?> GetByEmailAsync(string email)
            => await _context.Accounts
                .FirstOrDefaultAsync(x => x.Email == email);

        public async Task<bool> ExistsByEmailAsync(string email)
            => await _context.Accounts.AnyAsync(x => x.Email == email);

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account is null) return false;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
