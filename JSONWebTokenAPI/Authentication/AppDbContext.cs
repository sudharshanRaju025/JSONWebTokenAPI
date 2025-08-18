using JSONWebTokenAPI.Authentication;
using JSONWebTokenAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace JSONWebTokenAPI.Authentication
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {  }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(builder =>
            {
                builder.HasKey(rt => rt.Token);
               
                builder.HasOne(rt => rt.User)
                       .WithMany()
                       .HasForeignKey(rt => rt.UserId);
            });
        }
        public DbSet<User> users { get; set; }
    }
}
