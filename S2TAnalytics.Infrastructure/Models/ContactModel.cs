using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class ContactModel
    {
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Query { get; set; }
        public string QueryType { get; set; }
        public ContactUs ToContactUs(ContactModel contactModel)
        {

            if (contactModel == null)
                return null;

            return new ContactUs
            {

                Name = contactModel.Name,
                Email = contactModel.EmailId,
                CountryCode = contactModel.CountryCode,
                PhoneNumber = contactModel.PhoneNumber,
                Query = contactModel.Query,
                QueryType = contactModel.QueryType.Decrypt()
            };

        }
    }
}
