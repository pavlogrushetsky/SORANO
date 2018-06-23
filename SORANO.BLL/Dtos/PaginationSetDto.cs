using System.Collections.Generic;
using System.Linq;

namespace SORANO.BLL.Dtos
{
    public class PaginationSetDto<T>
    {
        public int Page { get; set; }

        public int Count => Items?.Count() ?? 0;

        public double TotalPages { get; set; }

        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}