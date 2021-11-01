using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Utilities
{
    public class AnalyticsResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public Object Countries { get; set; }
        public Object Cities { get; set; }
        public Object Groups { get; set; }
        public Object Instruments { get; set; }
        public Object Accounts { get; set; }
        public Object Timelines { get; set; }
        public Object Data { get; set; }
        public string WidgetId { get; set; }
    }
}