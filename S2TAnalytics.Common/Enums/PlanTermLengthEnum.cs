using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum PlanTermLengthEnum
    {
        [Display(Name = "1 Month")]
        OneMonth = 1,
        [Display(Name = "1 Year")]
        OneYear = 2,
        //[Display(Name = "2 Year")]
        //TwoYear = 3,
        //[Display(Name = "3 Year")]
        //ThreeYear = 4,
    }
}
