using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum RequestStatusEnum
    {
        [Display(Name = "Open")]
        Open = 1,
        [Display(Name = "Closed")]
        Closed = 2,
        [Display(Name = "On Hold")]
        OnHold = 3
    }
}
