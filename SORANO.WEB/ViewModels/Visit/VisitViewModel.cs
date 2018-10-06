using System.Collections.Generic;
using System.Linq;

namespace SORANO.WEB.ViewModels.Visit
{
    public class VisitViewModel
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string Date { get; set; }

        public string DateStandard { get; set; }

        public int LocationID { get; set; }

        public string LocationName { get; set; }

        public List<string> Codes => Code.ToList().Select(c => c.ToString()).ToList();
    }
}