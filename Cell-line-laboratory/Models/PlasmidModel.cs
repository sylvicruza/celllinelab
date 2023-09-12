using System;
using System.ComponentModel.DataAnnotations;

namespace Cell_line_laboratory.Models
{
    public class PlasmidModel
    {
        [Required]
        public string PlasmidCode { get; set; }

        [Required]
        public string PlasmidName { get; set; }

        public string Origin { get; set; }

        [Required]
        public string PlasmidMapLink { get; set; }

        public string Note { get; set; }
    }
}
