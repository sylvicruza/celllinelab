namespace Cell_line_laboratory.Entities
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime? NextMaintenance { get; set; }

        // Foreign key for the user who performed the maintenance
        public string MaintainedById { get; set; }


        // Navigation property for EquipmentInventory
        public EquipmentInventory Equipment { get; set; }
    }

}
