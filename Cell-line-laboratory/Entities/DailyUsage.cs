namespace Cell_line_laboratory.Entities
{
    public class DailyUsage : LabUsage
    {
        //This specifies the Code of either of (CellLine, Plasmid, Chemical and Enzymes)
        public string CellLineCode { get; set; }
    }

}
