using Microsoft.AspNetCore.Identity;

namespace Cell_line_laboratory.Entities
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }     //SuperUser
        public string UserType { get; set; } //SuperAdmin
        public string Status { get; set; } //Active/Inactive/Blocked
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Add a navigation property for the relationship with CellLines
     
        public ICollection<CellLine> CellLines { get; set; }

    }
}
