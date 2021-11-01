using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum PaymentTypeEnum
    {
        [Display(Name = "Credit Card")]
        CreditCard = 1,
        [Display(Name = "Debit Card")]
        DebitCard = 2,
     
    }
}
