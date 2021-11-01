using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UserEmbedWidgetPermissionModel
    {
        public string WidgetId { get; set; }
        public string[] Domain { get; set; }
    }
}
