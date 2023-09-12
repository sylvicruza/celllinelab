using Cell_line_laboratory.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Cell_line_laboratory.Data
{
    public class SeedDataForUser : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Add seed data to the database
            var password = "Password123";
            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = passwordHasher.HashPassword(null, password);

            var user = new User
            {
                Id = 1,
                Email = "superadmin@gmail.com",
                Password = hashedPassword,
                Name = "John Doe",
                Role = "SuperUser",
                UserType = "SuperAdmin",
                CreatedAt = DateTime.Now,
                Status = "Active",
            };

            builder.HasData(user);
        }
    }

}
