using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
   public enum PaymentStatusEnum
    {
        [Display(Name = "Trial Period")]
        TrailPeriod = 1,
        [Display(Name = "Paid")]
        Paid = 2,
        [Display(Name = "Overdue")]
        Overdue = 3,
    }
}
