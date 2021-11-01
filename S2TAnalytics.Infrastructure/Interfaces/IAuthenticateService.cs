using S2TAnalytics.Common.Helper;
using S2TAnalytics.Common.Utilities;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
   public interface IAuthenticateService
    {
        ServiceResponse CheckEmailToken(string email, string token);
        AuthenticationResponse AuthenticateUser(string email, string password, ref bool isExpired);
        void InsertPlans();
    }
}
