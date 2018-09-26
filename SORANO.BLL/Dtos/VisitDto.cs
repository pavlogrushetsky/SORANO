using System;

namespace SORANO.BLL.Dtos
{
    public class VisitDto : BaseDto
    {
        public string Code { get; set; }

        public DateTime Date { get; set; }

        public int LocationID { get; set; }
    }
}