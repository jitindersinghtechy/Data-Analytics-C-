using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserSubscriptionDeductionQueue:BaseEntity
    {
        public ObjectId UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime LastDeductionDate { get; set; }

        public int TermLengthId { get; set; }
    }
}
