using Cell_line_laboratory.Data;

namespace Cell_line_laboratory.DTOs
{
    public class EnzymeCountDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int UsedCount { get; set; }
        public int SemiUsedCount { get; set; }
        public int UnusedCount { get; set; }

        //public EnzymeCountDto GetEnzymeCounts(Cell_line_laboratoryContext context, string status)
        //{
        //    var count = context.Enzyme.Count(cl => cl.Status == status && cl.IsMarkedForDeletion == false);

        //    switch (status)
        //    {
        //        case "Used":
        //            UsedCount = count;
        //            break;
        //        case "Semi-Used":
        //            SemiUsedCount = count;
        //            break;
        //        case "Unused":
        //            UnusedCount = count;
        //            break;
        //    }

        //    return this;
        //}

        public EnzymeCountDto GetEnzymeCounts(Cell_line_laboratoryContext context, string status, int year, int month)
        {
            var count = context.Enzyme
                                .Count(cl => cl.Status == status && cl.IsMarkedForDeletion == false && cl.Date.Year == year && cl.Date.Month == month);

            switch (status)
            {
                case "Used":
                    UsedCount = count;
                    break;
                case "Semi-Used":
                    SemiUsedCount = count;
                    break;
                case "Unused":
                    UnusedCount = count;
                    break;
            }

            Year = year;
            Month = month;


            return this;
        }
    }
}