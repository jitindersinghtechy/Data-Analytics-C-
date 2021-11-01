using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class DailyEquity : BaseEntity
    {
        public int AccountNumber { get; set; }
        public Guid OrganizationId { get; set; }
        public List<EquityByDate> EquityByDate { get; set; }
    }
}
