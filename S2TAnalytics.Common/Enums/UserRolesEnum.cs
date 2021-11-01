using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum UserRolesEnum
    {
        [Display(Name = "Super Admin")]
        SuperAdmin = 1,
        [Display(Name = "Admin")]
        OrganizationAdmin = 2,
        [Display(Name = "Report User")]
        ReportUser = 3,
    }
}
