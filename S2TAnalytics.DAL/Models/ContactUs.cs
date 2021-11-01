using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class ContactUs:BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Query { get; set; }
        public int QueryType { get; set; }

    }
}
