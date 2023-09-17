namespace Cell_line_laboratory.Models
{
    public class MaintenanceDto
    {
        public int Id { get; set; }
       // public string ProductName { get; set; }

        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime? NextMaintenance { get; set; }

        // User who performed the maintenance
        public string MaintainedBy { get; set; }
    }

}
