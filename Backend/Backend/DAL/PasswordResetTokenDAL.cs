using Backend.MODEL;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL
{
    public class PasswordResetTokenDAL
    {
        private readonly connect _context;

        public PasswordResetTokenDAL(connect context)
        {
            _context = context;
        }

        public async Task<PasswordResetToken?> GetByIdAsync(long id)
            => await _context.PasswordResetTokens.FindAsync(id);

        public async Task<PasswordResetToken?> GetValidByHashAsync(string tokenHash)
            => await _context.PasswordResetTokens
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash &&
                    x.UsedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);

        public async Task<List<PasswordResetToken>> GetByAccountIdAsync(int accountId)
            => await _context.PasswordResetTokens
                .AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task AddAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsUsedAsync(PasswordResetToken token)
        {
            token.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
