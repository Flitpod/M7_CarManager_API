using M7_CarManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace M7_CarManager.Data
{
    public class ApiDbContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _configuration;
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Car> Cars { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> opt, IConfiguration configuration) : base(opt)
        {
            _configuration = configuration;
        }

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
                Email = _configuration["Email"],
                EmailConfirmed = true,
                UserName = _configuration["UserName"],
                FirstName = _configuration["FirstName"],
                LastName = _configuration["LastName"],
                NormalizedUserName = _configuration["NormalizedUserName"],
                NormalizedEmail = _configuration["Email"].ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            user.PasswordHash = passwordHasher.HashPassword(user, _configuration["Password"]);

            builder.Entity<AppUser>().HasData(user);
            builder.Entity<Car>()
                .HasOne(car => car.Owner)
                .WithMany()
                .HasForeignKey(car => car.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Car>()
                .HasData(new Car() { Model = "Opel Astra", PlateNumber = "AAA-111", Price = 2000 });

            base.OnModelCreating(builder);
        }
    }
}
