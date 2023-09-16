using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cell_line_laboratory.Entities;
using Cell_line_laboratory.Models;
using Microsoft.AspNetCore.Identity;


namespace Cell_line_laboratory.Data
{
    public class Cell_line_laboratoryContext : DbContext
    {
        public Cell_line_laboratoryContext(DbContextOptions<Cell_line_laboratoryContext> options)
            : base(options)
        {
        }

        public DbSet<Cell_line_laboratory.Entities.User> User { get; set; } = default!;
        public DbSet<Plasmid> PlasmidCollection { get; set; } = default!; // Add this DbSet

        public DbSet<EquipmentInventory> EquipmentInventory { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }

        public DbSet<Product> Products { get; set; }



        public DbSet<CellLine> CellLine { get; set; } = default!;
        public DbSet<DailyUsage> DailyUsage { get; set; }

        public DbSet<IdentityRole> Roles { get; set; } // Add this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure your entity relationships, indexes, keys, etc. here
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().HasIndex(e => e.Name);

            modelBuilder.Entity<Plasmid>()
                .HasKey(pc => pc.Id);

            // Apply configuration classes for data seeding
            modelBuilder.ApplyConfiguration(new SeedDataForUser());
            // Apply other configuration classes for other entities as needed

            modelBuilder.Entity<CellLine>()
                .HasKey(pc => pc.Id);

            // Add the following code to specify OnDelete behavior
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Equipment)
                .WithMany(e => e.Maintenances)
                .HasForeignKey(m => m.EquipmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EquipmentInventoryModel>()
                .HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);

        }

        // ... rest of your code


        public DbSet<Cell_line_laboratory.Entities.Enzyme>? Enzyme { get; set; }

        public DbSet<Cell_line_laboratory.Entities.Chemical>? Chemical { get; set; }

        public DbSet<Cell_line_laboratory.Entities.AssignTask>? AssignTask { get; set; }

        //public DbSet<Cell_line_laboratory.Entities.EquipmentInventory> EquipmentInventory { get; set; }

        //public DbSet<Cell_line_laboratory.Entities.Maintenance> Maintenances { get; set; }




    }
}
