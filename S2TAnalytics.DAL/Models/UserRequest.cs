using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserRequest
    {
        public int RequestType { get; set; }
        public string Details { get; set; }
        public int StatusID { get; set; }
    }
}
