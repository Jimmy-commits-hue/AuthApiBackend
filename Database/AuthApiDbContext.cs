using Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Web.Database
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
            //modelBuilder.Entity<User>().HasIndex(e => e.customNumber).IsUnique();

            modelBuilder.Entity<User>().HasMany(u => u.emailVerification).WithOne(u => u.user).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(attempts => attempts.loginAttempts).WithOne(u => u.user).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(refresh => refresh.refreshToken).WithOne(u => u.user).
                HasForeignKey(u => u.userId).OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);

        }

        public AuthApiDbContext(DbContextOptions<AuthApiDbContext> options) : base(options) { }

    }

}