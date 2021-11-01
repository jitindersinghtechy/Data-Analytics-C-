using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum EmailNotificationSettingsEnum
    {
        [Display(Name = "Daily top performers' & pinned traders' trends")]
        Daily = 1,
        [Display(Name = "Weekly top performers' & pinned traders' trends")]
        Weekly = 2,
        [Display(Name = "Monthly top performers' & pinned traders")]
        Monthly = 3,
        [Display(Name = "Quarterly top performers' & pinned traders' trends")]
        Quarterly = 4,
        [Display(Name = "Half yearly top performers' & pinned traders' trends")]
        HalfYearly = 5,
        [Display(Name = "Yearly top performers' & pinned traders' trends")]
        Yearly = 6,
    }
}
