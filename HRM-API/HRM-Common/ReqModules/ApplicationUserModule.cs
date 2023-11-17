using HRM_Common.Paged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.ReqModules
{
    public class GetApplicationUserModule : PagingRequest
    {
        public string? Keyword { get; set; }
    }
}
