using Cell_line_laboratory.Entities;

namespace Cell_line_laboratory.Models
{
    public class AssignTaskViewModel
    {

        //public int UserId { get; set; }

        public ICollection<int> UserId { get; set; }

        public string Designation { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AssignedBy { get; set; }
    }
}
