using System.ComponentModel;

namespace Cell_line_laboratory.Entities
{
    public class EquipmentInventory
    {
        public int Id { get; set; }


        [DisplayName("Product Category")]

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [DisplayName("Product Code")]
        public string? ProductCode { get; set; }

        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        [DisplayName("Product Description")]
        public string? ProductDescription { get; set; }

        public string? Vendor { get; set; }

        [DisplayName("Serial Number")]
        public string? SerialNumber { get; set; }

        public int? Quantity { get; set; }

        [DisplayName("Last Maintenance Date")]
        public DateTime? LastMaintenanceDate { get; set; }

        [DisplayName("Next Maintenance Date")]
        public DateTime? NextMaintenanceDate { get; set; }

        [DisplayName("Amount / Price")]
        public decimal? Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}


        // Foreign key for the creator user
        public string? CreatedBy { get; set; }

        // Navigation property for Maintenance records
        public List<Maintenance> Maintenances { get; set; }

    }


}
