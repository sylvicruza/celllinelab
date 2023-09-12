using Cell_line_laboratory.Entities;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Cell_line_laboratory.Models
{
    public class CellLineModel
    {
        public int Id { get; set; } // Primary key (auto-generated)

        [Required(ErrorMessage = "Cell Line Code is required.")]
        [Display(Name = "Cell Line Code")]
        public string CellLineCode { get; set; }

        [Required(ErrorMessage = "Genotype is required.")]
        [Display(Name = "Genotype")]
        public string Genotype { get; set; }

        [Required(ErrorMessage = "Parental Line is required.")]
        [Display(Name = "Parental Line")]
        public string ParentalLine { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

    
        //[Range(1, 100, ErrorMessage = "Position must be between 1 and 100.")]
        [Display(Name = "Position")]
        public string Position { get; set; }


        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public string UserName { get; set; } // User's name


        //// Foreign key to User
        //public int UserId { get; set; }
        //public User User { get; set; }

        public List<int> SelectedPositions { get; set; }


    }

}
