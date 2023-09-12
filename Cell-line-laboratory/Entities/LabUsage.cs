namespace Cell_line_laboratory.Entities
{
    public class LabUsage
    {
        public int Id { get; set; }
       
        public decimal Usage { get; set; }
        public decimal Balance { get; set; }
        public DateTime Date { get; set; }
        public string Username { get; set; }
        public string Comment { get; set; }
    }
}
