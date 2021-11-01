using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserEmbedWidgetPermission
    {
        public int WidgetId { get; set; }
        public string[] Domain { get; set; }
    }
}
