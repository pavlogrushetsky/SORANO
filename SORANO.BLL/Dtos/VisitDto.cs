using System;
using System.Collections.Generic;

namespace SORANO.BLL.Dtos
{
    public class VisitDto : BaseDto
    {
        public string Code { get; set; }

        public DateTime Date { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public List<string> Visitors { get; set; }
    }
}