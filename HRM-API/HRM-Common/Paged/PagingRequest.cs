using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Paged
{
    public abstract class PagingRequest
    {
        public PagingRequest()
        {
            Page = 1;
            PageSize = 10;
            Column = "";
            Direction = "desc";
        }

        public int Page { get; set; }
        public int PageSize { get; set; }

        /// <summary>
        /// field
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// asc | desc
        /// </summary>
        public string Direction { get; set; }
    }
}
