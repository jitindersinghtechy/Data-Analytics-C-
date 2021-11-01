using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class EquityByDate
    {
        public int id { get; set; }
        public string date { get; set; }
        public double Equity { get; set; }
        public double EquityWithoutWithdrawn { get; set; }
        public double SharpeRatioReturn { get; set; }

    }
}
