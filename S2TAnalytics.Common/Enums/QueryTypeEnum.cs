using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum QueryTypeEnum
    {
        [Display(Name = "General")]
        General = 1,
        [Display(Name = "Technical Issues")]
        TechnicalIssues = 2,
        [Display(Name = "Subscription")]
        Subscription = 3,
        [Display(Name = "Others")]
        Others = 4
    }
}
