using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
   public  class AccountDetailWithHighestValue
    {
        public ObjectId AccountId { get; set; }
        public double AverageNav { get; set; }
    }
}
