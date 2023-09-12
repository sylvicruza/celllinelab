using Cell_line_laboratory.Entities;

namespace Cell_line_laboratory.Models
{
    public class DailyUsageViewModel
    {
        public int UsageId { get; set; }
        public string Code { get; set; }  //This specifies the Code of either of (CellLine, Plasmid, Chemical and Enzymes)
        public List<DailyUsage> DailyUsages { get; set; } // List of DailyUsage for the search result
        public CellLine CellLineDetails { get; set; } // CellLine details for the usage form
        public Plasmid PlasmidDetails { get; set; } // Plasmid details for the usage form
        public Enzyme EnzymeDetails { get; set; } // Enzyme details for the usage form
        public Chemical ChemicalDetails { get; set; } // Chemical details for the usage form
        public decimal Usage { get; set; }
        public string Comment { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public string Username { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }

}
