using MongoDB.Bson;
using P23.MetaTrader4.Manager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalyticsELT.Models.MT4
{
    public class UserRequest
    {
        public UserRequest()
        {

            TradesHistories = new List<TradeRecord>();
        }

        public ObjectId _id { get; set; }
        public string Address { get; set; }
        public int AgentAccount { get; set; }
        public string ApiData { get; set; }
        public double Balance { get; set; }
        public string City { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }
        public double Credit { get; set; }
        public string Email { get; set; }
        public int Enable { get; set; }
        public int EnableChangePassword { get; set; }
        public int EnableOTP { get; set; }
        public int EnableReadOnly { get; set; }
        public string Group { get; set; }
        public string UserId { get; set; }
        public double InterestRate { get; set; }
        public uint LastDate { get; set; }
        public int LastIp { get; set; }
        public string LeadSource { get; set; }
        public int Leverage { get; set; }
        public int Login { get; set; }
        public uint Mqid { get; set; }
        public string Name { get; set; }
        public string OTPSecret { get; set; }
        public string Password { get; set; }
        public string PasswordInvestor { get; set; }
        public string PasswordPhone { get; set; }
        public string Phone { get; set; }
        public double PrevBalance { get; set; }
        public double PrevEquity { get; set; }
        public double PrevMonthBalance { get; set; }
        public double PrevMonthEquity { get; set; }
        public string PublicKey { get; set; }
        public uint Regdate { get; set; }
        public int SendReports { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public double Taxes { get; set; }
        public uint Timestamp { get; set; }
        public uint UserColor { get; set; }
        public string ZipCode { get; set; }
        public List<TradeRecord> TradesHistories { get; set; }

    }
}
