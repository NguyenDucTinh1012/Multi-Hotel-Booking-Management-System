using Backend.MODEL;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    public class connect:DbContext
    {
        public connect(DbContextOptions<connect> options) : base(options) { }
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.RoleId);
                entity.Property(x => x.RoleName).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Description).HasMaxLength(255);
                entity.HasIndex(x => x.RoleName).IsUnique();
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(x => x.AccountId);
                entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
                entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(x => x.VerificationTokenHash).HasMaxLength(255);
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.IsEmailVerified).HasDefaultValue(false);
                entity.HasIndex(x => x.Email).IsUnique();
                entity.HasIndex(x => x.RoleId);

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.Accounts)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(x => x.VerificationOtpAttempts)
    .HasDefaultValue(0);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", table =>
                    table.HasCheckConstraint(
                        "CK_Users_Gender",
                        "[Gender] IS NULL OR [Gender] IN (N'Nam', N'Nữ', N'Khác')"));

                entity.HasKey(x => x.UserId);
                entity.Property(x => x.FullName).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Phone).HasMaxLength(20);
                entity.Property(x => x.Avatar).HasMaxLength(500);
                entity.Property(x => x.Gender).HasMaxLength(20);
                entity.Property(x => x.DateOfBirth).HasColumnType("date");
                entity.Property(x => x.Address).HasMaxLength(255);
                entity.HasIndex(x => x.AccountId).IsUnique();

                entity.HasOne(x => x.Account)
                    .WithOne(x => x.User)
                    .HasForeignKey<User>(x => x.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetTokens");
                entity.HasKey(x => x.TokenId);
                entity.Property(x => x.TokenHash).HasMaxLength(255).IsRequired();
                entity.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
                entity.HasIndex(x => x.TokenHash).IsUnique();
                entity.HasIndex(x => x.AccountId);

                entity.HasOne(x => x.Account)
                    .WithMany(x => x.PasswordResetTokens)
                    .HasForeignKey(x => x.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
        
    }
}
