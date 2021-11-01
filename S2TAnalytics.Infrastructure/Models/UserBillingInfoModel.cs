using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class UserBillingInfoModel
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string ISO { get; set; }
        public List<UserBillingInfoModel> ToUserBillingModel(List<UserBillingInfo> userBillingInfos)
        {
            if (userBillingInfos == null)
                return null;

            return userBillingInfos.Select(d => new UserBillingInfoModel()
            {
                Id = d.Id,
                Address = d.Address,
                Country = d.Country,
                ISO = d.ISO,
                State = d.State,
                City = d.City,
                ZipCode = d.ZipCode,
                CountryCode=d.CountryCode,
                PhoneNumber=d.PhoneNumber,
                IsActive = d.IsActive
               
            }).ToList();
        }
    }
}
