using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserCard
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int PaymentType { get; set; }
    }
}
