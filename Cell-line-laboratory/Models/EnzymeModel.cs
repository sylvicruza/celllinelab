using Cell_line_laboratory.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cell_line_laboratory.Models
{
    public class EnzymeModel
    {
        [Required]
        public string PlasmidCode { get; set; }

        [Required]
        public string AntibodyName { get; set; }

        public string CatalogueNo { get; set; }

        public string Note { get; set; }

    
        public string Maker { get; set; }


        public string Location { get; set; }

        public string Data { get; set; }
         
}
}
