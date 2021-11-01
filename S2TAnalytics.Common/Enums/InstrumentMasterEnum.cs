using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum InstrumentMasterEnum
    {
        [Display(Name = "EUR/USD")]
        EURUSD = 1,
        [Display(Name = "GBP/SGD")]
        GBPSGD = 2,
        [Display(Name = "SGD/AUD")]
        SGDAUD = 3,
    }

}
