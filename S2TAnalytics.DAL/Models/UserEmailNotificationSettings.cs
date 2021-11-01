using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class UserEmailNotificationSettings
    {
        public int NotificationSettingsId { get; set; }
        public bool HasAccess { get; set; }
    }
}
