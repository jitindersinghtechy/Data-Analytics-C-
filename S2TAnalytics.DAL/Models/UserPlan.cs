using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserPlan
    {
        public int PlanID { get; set; }
        public double Price { get; set; }
        public int Days { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartedDate { get; set; }
        public bool IsCardDeductionError { get; set; }
        public string Message { get; set; }
        public int TermLengthId { get; set; }
    }
}
