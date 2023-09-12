namespace Cell_line_laboratory.Entities
{
    public class EquipmentInventory
    {
        public int Id { get; set; }  
        public string Product { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Quantity { get; set; }

        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
     

        public double Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}

    }
}
