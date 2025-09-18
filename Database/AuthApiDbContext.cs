using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Database
{

    public class AuthApiDbContext : DbContext
    {

        public DbSet<User> User { get; set; }

        public DbSet<EmailVerification> EmailVerification { get; set; }

        public DbSet<LoginAttempts> LoginAttempt { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasIndex(e => e.IdNumber).IsUnique();

            modelBuilder.Entity<User>().HasMany(u => u.EmailVerification).WithOne(u => u.User).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(attempts => attempts.LoginAttempts).WithOne(u => u.User).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(refresh => refresh.RefreshToken).WithOne(u => u.User).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

        }

        public AuthApiDbContext(DbContextOptions<AuthApiDbContext> options) : base(options) { }

    }

}