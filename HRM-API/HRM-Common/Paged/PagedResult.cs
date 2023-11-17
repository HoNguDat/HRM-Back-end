using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Paged
{
    public class PagedResult<T> where T : class
    {
        public PagedResult()
        {
            Results = new List<T>();
        }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public IEnumerable<T> Results { get; set; }
        public object Object { get; set; }
    }
}
