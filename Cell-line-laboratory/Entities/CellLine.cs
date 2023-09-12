using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cell_line_laboratory.Entities
{
    public class CellLine
    {
        public int Id { get; set; } // Primary key (auto-generated)

        [Display(Name = "Cell Line Code")]
        public string CellLineCode { get; set; }

        [Display(Name = "Genotype")]
        public string Genotype { get; set; }

        [Display(Name = "Parental Line")]
        public string ParentalLine { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Position")]
        public string Position { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        // Foreign key to User
        [Display(Name = "User")]
       // public int UserId { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }


        // Navigation property to User
        // public User User { get; set; }

        public bool IsMarkedForDeletion { get; set; }
        public DateTime? DeletionTimestamp { get; set; }
    }
}
