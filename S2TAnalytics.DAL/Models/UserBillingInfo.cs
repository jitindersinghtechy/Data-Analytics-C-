using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserBillingInfo
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string ISO { get; set; }
        public string State { get; set; }
        public string City { get; set; }        
        public string ZipCode { get; set; }
        public int CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
