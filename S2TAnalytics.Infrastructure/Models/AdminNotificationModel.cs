using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class AdminNotificationModel
    {
        public string userId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid NotificationId { get; set; }
        public string Notification { get; set; }
        public DateTime Date { get; set; }
    }
}
