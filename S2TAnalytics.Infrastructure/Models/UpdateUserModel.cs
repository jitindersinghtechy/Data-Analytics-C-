using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UpdateUserModel
    {
        public bool IsBlock { get; set; }
        public List<string> EmailIds { get; set; }
        public List<string> UserGroups { get; set; }
        public List<string> DataSourceIds { get; set; }
    }
}
