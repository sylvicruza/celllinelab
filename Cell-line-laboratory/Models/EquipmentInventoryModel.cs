using Cell_line_laboratory.Entities;
using System.Web.Mvc;

namespace Cell_line_laboratory.Models
{
    public class EquipmentInventoryModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public string? Vendor { get; set; }
        public string? SerialNumber { get; set; }
        public int? Quantity { get; set; }
        public decimal? Amount { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // User who created the equipment
        public string? CreatedBy { get; set; }





    }


}
