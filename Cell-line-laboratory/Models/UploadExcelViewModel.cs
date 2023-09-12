using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Cell_line_laboratory.Models
{
    public class UploadExcelViewModel
    {
        [Required]
        [Display(Name = "Excel File")]
        public IFormFile File { get; set; }

        // You might include additional properties for other fields related to the upload process
    }
}
