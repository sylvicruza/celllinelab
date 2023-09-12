namespace Cell_line_laboratory.Entities
{
    public class Antibody
    {
        public int Id { get; set; }
        public string PlasmidCode { get; set; }
        public string AntibodyName { get; set; }
        public string CatalogueNo { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Maker { get; set; }
        public string Note { get; set; }

        public string Status { get; set; }

        public string Location { get; set; }

        public string Data { get; set; }
        public bool IsMarkedForDeletion { get; set; }
        public DateTime? DeletionTimestamp { get; set; }
    }
}
