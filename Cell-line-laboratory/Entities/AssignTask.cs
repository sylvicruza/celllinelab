namespace Cell_line_laboratory.Entities
{
    public class AssignTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public string Designation { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AssignedBy { get; set; }
    }
}
