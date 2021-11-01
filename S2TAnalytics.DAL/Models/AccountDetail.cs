using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class AccountDetail : BaseEntity
    {
        public AccountDetail()
        {
            this.AccountStats = new List<AccountStats>();
            this.AccountTransactionHistories = new List<AccountTransactionHistory>();
            this.InstrumentStats= new List<InstrumentStats>();
        }
        public Guid OrganizationId { get; set; }
        //public int DataSourceId { get; set; }
        public ObjectId DataSourceId { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public int Leverage { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public double CityLong { get; set; }
        public double CityLat { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public string UserGroup { get; set; }
        public string ActiveSince { get; set; }
        public string LastActiveOn { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string Status { get; set; }
        public bool IsExclude { get; set; }
        
        public List<AccountStats> AccountStats { get; set; }
        public List<AccountTransactionHistory> AccountTransactionHistories { get; set; }
        public List<InstrumentStats> InstrumentStats { get; set; }

    }
}
