using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class RateOfReturnsModel
    {
        public int TimelineId { get; set; }
        public double RateOfReturn { get; set; }
        public string label { get; set; }
    }
}
