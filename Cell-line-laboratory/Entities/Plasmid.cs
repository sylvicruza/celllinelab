namespace Cell_line_laboratory.Entities
{
    public class Plasmid
    {
        public int Id { get; set; }
        public string PlasmidCode { get; set; }
        public string PlasmidName { get; set; }
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string PlasmidMapLink { get; set; }
        public string Note { get; set; }

        public string Status { get; set; }
        public bool IsMarkedForDeletion { get; set; }
        public DateTime? DeletionTimestamp { get; set; }
    }
}
