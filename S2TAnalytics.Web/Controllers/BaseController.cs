using MongoDB.Bson;
using S2TAnalytics.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace S2TAnalytics.Web.Controllers
{
    public class BaseController : ApiController
    {
        private IUserService _userService;
        public BaseController(IUserService userService)
        {
            _userService = userService;
        }
        private string GetClaim(string type)
        {
            var identity = User.Identity as ClaimsIdentity;

            var claims = from c in identity.Claims
                         select new
                         {
                             subject = c.Subject.Name,
                             type = c.Type,
                             value = c.Value
                         };
            return claims.Single(c => c.type == type).value;
        }
        public Guid OrganizationID
        {
            get
            {
                return Guid.Parse(GetClaim("OrganizationID"));
            }
        }

        public ObjectId UserID
        {
            get
            {
                return ObjectId.Parse(GetClaim("UserID"));
            }
        }

        public int RoleID
        {
            get
            {
                return Convert.ToInt32(GetClaim("RoleID"));
            }
        }

        public List<ObjectId> DatasourceIDs
        {
            get
            {
                return _userService.GetSelectedDatasourceIds(UserID);
            }
        }

        public string[] UserGroups
        {
            get
            {
                var userGroup = GetClaim("UserGroups");
                string[] userGroups = userGroup.Split(',');
                if (RoleID == 2)
                {
                    if (userGroup == "")
                        return _userService.GetUserGroups(OrganizationID);
                    else
                        return userGroups;
                }
                return userGroups;
            }
        }

    }
}
