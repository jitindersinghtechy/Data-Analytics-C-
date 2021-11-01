using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum SubscriberReminderEnum
    {
        [Display(Name = "Payment Overdue")]
        PaymentOverdue = 1,
        [Display(Name = "Trail Period")]
        TrailPeriod = 2,
        [Display(Name = "After Trail Period")]
        AfterTrailPeriod = 3,
    }
}
