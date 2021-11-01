using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum  PlansEnum
    {
        [Display(Name = "Dashboard with drildown (Trader statistics)")]
        Dashboard = 1,
        [Display(Name = "View Details Account Details")]
        AccountDetail = 2,
        [Display(Name = "Compare account performance")]
        Compare = 3,
        [Display(Name = "Pin account")]
        Pin_Account = 4,
        [Display(Name = "Send email notification (Performance Comparision)")]
        Performance_Comparison_Email_Notification = 5,
        [Display(Name = "Exclude accounts")]
        Exclude_Account = 6,
        [Display(Name = "Add report users")]
        Add_Report_User = 7,
        [Display(Name = "Embed dashboard widgets")]
        Embed_Widget = 8,
        [Display(Name = "API Data for the dashboard widgets")]
        API_Data_for_Widget = 9,
        [Display(Name = "Export dashboard, performers")]
        Export_Data = 10,
        //[Display(Name = "Account Statement")]
        //Account_Statement = 11
    }
}
