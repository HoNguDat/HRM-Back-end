using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM_Common.Models.ViewModels.Authenticate
{
    public class TokenViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
