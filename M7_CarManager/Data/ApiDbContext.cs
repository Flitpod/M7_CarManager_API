using M7_CarManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace M7_CarManager.Data
{
    public class ApiDbContext : IdentityDbContext<AppUser>
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> opt) : base(opt)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>()
                .HasData(
                    new { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                    new { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER" }
                );

            // test user seed
            PasswordHasher<AppUser> passwordHasher = new PasswordHasher<AppUser>();
            AppUser user = new AppUser()
            {
                Id = Guid.NewGuid().ToString(),
                Email = "coding_helper@outlook.com",
                EmailConfirmed = true,
                UserName = "coding_helper@outlook.com",
                FirstName = "Coding",
                LastName = "Helper",
                NormalizedUserName = "CODING_HELPER@OUTLOOK.COM",
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "password1");

            builder.Entity<AppUser>().HasData(user);

            base.OnModelCreating(builder);
        }
    }
}
