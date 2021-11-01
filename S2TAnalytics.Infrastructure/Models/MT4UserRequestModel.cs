//using MongoDB.Bson;
using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class MT4UserRequestModel
    {
        public MT4UserRequestModel  ()
        {

            TradesHistories = new List<TradeRecord>();
        }

        public string UserRequestId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Address { get; set; }
        public Int64 AgentAccount { get; set; }
        public string ApiData { get; set; }
        public double Balance { get; set; }
        public string City { get; set; }
        public string Comment { get; set; }
        public string Country { get; set; }
        public double Credit { get; set; }
        public string Email { get; set; }
        public Int64 Enable { get; set; }
        public Int64 EnableChangePassword { get; set; }
        public int EnableOTP { get; set; }
        public int EnableReadOnly { get; set; }
        public string Group { get; set; }
        public string UserId { get; set; }
        public double InterestRate { get; set; }
        public Int64 LastDate { get; set; }
        public Int64 LastIp { get; set; }
        public string LeadSource { get; set; }
        public int Leverage { get; set; }
        public int Login { get; set; }
        public Int64 Mqid { get; set; }
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
        public Int64 Regdate { get; set; }
        public int SendReports { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public double Taxes { get; set; }
        public uint Timestamp { get; set; }
        public Int64 UserColor { get; set; }
        public string ZipCode { get; set; }
        public List<TradeRecord> TradesHistories { get; set; }

        public List<MT4UserRequest> ToMT4UserRequest(List<MT4UserRequestModel> mT4UserRequestModels)
        {
            if (mT4UserRequestModels == null && mT4UserRequestModels.Count <= 0)
                return new List<MT4UserRequest>();
            return mT4UserRequestModels.Select(mT4UserRequestModel => new MT4UserRequest
            {
                Login = mT4UserRequestModel.Login,
                OrganizationId = mT4UserRequestModel.OrganizationId,
                Address = mT4UserRequestModel.Address,
                AgentAccount = mT4UserRequestModel.AgentAccount,
                ApiData = mT4UserRequestModel.ApiData,
                Balance = mT4UserRequestModel.Balance,
                City = mT4UserRequestModel.City,
                Comment = mT4UserRequestModel.Comment,
                Country = mT4UserRequestModel.Country,
                Credit = mT4UserRequestModel.Credit,
                Email = mT4UserRequestModel.Email,
                Enable = mT4UserRequestModel.Enable,
                EnableChangePassword = mT4UserRequestModel.EnableChangePassword,
                EnableOTP = mT4UserRequestModel.EnableOTP,
                EnableReadOnly = mT4UserRequestModel.EnableReadOnly,
                Group = mT4UserRequestModel.Group,
                UserId = mT4UserRequestModel.UserId,
                InterestRate = mT4UserRequestModel.InterestRate,
                LastDate = mT4UserRequestModel.LastDate,
                LastIp = mT4UserRequestModel.LastIp,
                LeadSource = mT4UserRequestModel.LeadSource,
                Leverage = mT4UserRequestModel.Leverage,
                Mqid = mT4UserRequestModel.Mqid,
                Name = mT4UserRequestModel.Name,
                OTPSecret = mT4UserRequestModel.OTPSecret,
                Password = mT4UserRequestModel.Password,
                PasswordInvestor = mT4UserRequestModel.PasswordInvestor,
                PasswordPhone = mT4UserRequestModel.PasswordPhone,
                Phone = mT4UserRequestModel.Phone,
                PrevBalance = mT4UserRequestModel.PrevBalance,
                PrevEquity = mT4UserRequestModel.PrevEquity,
                PrevMonthBalance = mT4UserRequestModel.PrevMonthBalance,
                PrevMonthEquity = mT4UserRequestModel.PrevMonthEquity,
                PublicKey = mT4UserRequestModel.PublicKey,
                Regdate = mT4UserRequestModel.Regdate,
                SendReports = mT4UserRequestModel.SendReports,
                State = mT4UserRequestModel.State,
                Status = mT4UserRequestModel.Status,
                Taxes = mT4UserRequestModel.Taxes,
                Timestamp = mT4UserRequestModel.Timestamp,
                UserColor = mT4UserRequestModel.UserColor,
                ZipCode = mT4UserRequestModel.ZipCode,
                TradesHistories = mT4UserRequestModel.TradesHistories
            }).ToList();
        }


        public MT4UserRequest ToMT4UserRequest(MT4UserRequestModel mT4UserRequestModel)
        {
            if (mT4UserRequestModel == null)
                return new MT4UserRequest();
            return new MT4UserRequest
            {
                Login = mT4UserRequestModel.Login,
                OrganizationId = mT4UserRequestModel.OrganizationId,
                Address = mT4UserRequestModel.Address,
                AgentAccount = mT4UserRequestModel.AgentAccount,
                ApiData = mT4UserRequestModel.ApiData,
                Balance = mT4UserRequestModel.Balance,
                City = mT4UserRequestModel.City,
                Comment = mT4UserRequestModel.Comment,
                Country = mT4UserRequestModel.Country,
                Credit = mT4UserRequestModel.Credit,
                Email = mT4UserRequestModel.Email,
                Enable = mT4UserRequestModel.Enable,
                EnableChangePassword = mT4UserRequestModel.EnableChangePassword,
                EnableOTP = mT4UserRequestModel.EnableOTP,
                EnableReadOnly = mT4UserRequestModel.EnableReadOnly,
                Group = mT4UserRequestModel.Group,
                UserId = mT4UserRequestModel.UserId,
                InterestRate = mT4UserRequestModel.InterestRate,
                LastDate = mT4UserRequestModel.LastDate,
                LastIp = mT4UserRequestModel.LastIp,
                LeadSource = mT4UserRequestModel.LeadSource,
                Leverage = mT4UserRequestModel.Leverage,
                Mqid = mT4UserRequestModel.Mqid,
                Name = mT4UserRequestModel.Name,
                OTPSecret = mT4UserRequestModel.OTPSecret,
                Password = mT4UserRequestModel.Password,
                PasswordInvestor = mT4UserRequestModel.PasswordInvestor,
                PasswordPhone = mT4UserRequestModel.PasswordPhone,
                Phone = mT4UserRequestModel.Phone,
                PrevBalance = mT4UserRequestModel.PrevBalance,
                PrevEquity = mT4UserRequestModel.PrevEquity,
                PrevMonthBalance = mT4UserRequestModel.PrevMonthBalance,
                PrevMonthEquity = mT4UserRequestModel.PrevMonthEquity,
                PublicKey = mT4UserRequestModel.PublicKey,
                Regdate = mT4UserRequestModel.Regdate,
                SendReports = mT4UserRequestModel.SendReports,
                State = mT4UserRequestModel.State,
                Status = mT4UserRequestModel.Status,
                Taxes = mT4UserRequestModel.Taxes,
                Timestamp = mT4UserRequestModel.Timestamp,
                UserColor = mT4UserRequestModel.UserColor,
                ZipCode = mT4UserRequestModel.ZipCode,
                TradesHistories = mT4UserRequestModel.TradesHistories
            };
        }


        public List<MT4UserRequestModel> ToMT4UserRequestModel(List<MT4UserRequest> mT4UserRequests)
        {
            if (mT4UserRequests == null && mT4UserRequests.Count <= 0)
                return new List<MT4UserRequestModel>();
            return mT4UserRequests.Select(mT4UserRequest=> new MT4UserRequestModel
            {
                UserRequestId = mT4UserRequest.Id.ToString(),
                Login = mT4UserRequest.Login,
                OrganizationId = mT4UserRequest.OrganizationId,
                Address = mT4UserRequest.Address,
                AgentAccount = mT4UserRequest.AgentAccount,
                ApiData = mT4UserRequest.ApiData,
                Balance = mT4UserRequest.Balance,
                City = mT4UserRequest.City,
                Comment = mT4UserRequest.Comment,
                Country = mT4UserRequest.Country,
                Credit = mT4UserRequest.Credit,
                Email = mT4UserRequest.Email,
                Enable = mT4UserRequest.Enable,
                EnableChangePassword = mT4UserRequest.EnableChangePassword,
                EnableOTP = mT4UserRequest.EnableOTP,
                EnableReadOnly = mT4UserRequest.EnableReadOnly,
                Group = mT4UserRequest.Group,
                UserId = mT4UserRequest.UserId,
                InterestRate = mT4UserRequest.InterestRate,
                LastDate = mT4UserRequest.LastDate,
                LastIp = mT4UserRequest.LastIp,
                LeadSource = mT4UserRequest.LeadSource,
                Leverage = mT4UserRequest.Leverage,
                Mqid = mT4UserRequest.Mqid,
                Name = mT4UserRequest.Name,
                OTPSecret = mT4UserRequest.OTPSecret,
                Password = mT4UserRequest.Password,
                PasswordInvestor = mT4UserRequest.PasswordInvestor,
                PasswordPhone = mT4UserRequest.PasswordPhone,
                Phone = mT4UserRequest.Phone,
                PrevBalance = mT4UserRequest.PrevBalance,
                PrevEquity = mT4UserRequest.PrevEquity,
                PrevMonthBalance = mT4UserRequest.PrevMonthBalance,
                PrevMonthEquity = mT4UserRequest.PrevMonthEquity,
                PublicKey = mT4UserRequest.PublicKey,
                Regdate = mT4UserRequest.Regdate,
                SendReports = mT4UserRequest.SendReports,
                State = mT4UserRequest.State,
                Status = mT4UserRequest.Status,
                Taxes = mT4UserRequest.Taxes,
                Timestamp = mT4UserRequest.Timestamp,
                UserColor = mT4UserRequest.UserColor,
                ZipCode = mT4UserRequest.ZipCode,
                TradesHistories = mT4UserRequest.TradesHistories
            }).ToList();
        }


        public MT4UserRequestModel ToMT4UserRequestModel(MT4UserRequest mT4UserRequest)
        {
            if (mT4UserRequest == null)
                return new MT4UserRequestModel();
            return new MT4UserRequestModel
            {
                UserRequestId = mT4UserRequest.Id.ToString(),
                Login = mT4UserRequest.Login,
                OrganizationId = mT4UserRequest.OrganizationId,
                Address = mT4UserRequest.Address,
                AgentAccount = mT4UserRequest.AgentAccount,
                ApiData = mT4UserRequest.ApiData,
                Balance = mT4UserRequest.Balance,
                City = mT4UserRequest.City,
                Comment = mT4UserRequest.Comment,
                Country = mT4UserRequest.Country,
                Credit = mT4UserRequest.Credit,
                Email = mT4UserRequest.Email,
                Enable = mT4UserRequest.Enable,
                EnableChangePassword = mT4UserRequest.EnableChangePassword,
                EnableOTP = mT4UserRequest.EnableOTP,
                EnableReadOnly = mT4UserRequest.EnableReadOnly,
                Group = mT4UserRequest.Group,
                UserId = mT4UserRequest.UserId,
                InterestRate = mT4UserRequest.InterestRate,
                LastDate = mT4UserRequest.LastDate,
                LastIp = mT4UserRequest.LastIp,
                LeadSource = mT4UserRequest.LeadSource,
                Leverage = mT4UserRequest.Leverage,
                Mqid = mT4UserRequest.Mqid,
                Name = mT4UserRequest.Name,
                OTPSecret = mT4UserRequest.OTPSecret,
                Password = mT4UserRequest.Password,
                PasswordInvestor = mT4UserRequest.PasswordInvestor,
                PasswordPhone = mT4UserRequest.PasswordPhone,
                Phone = mT4UserRequest.Phone,
                PrevBalance = mT4UserRequest.PrevBalance,
                PrevEquity = mT4UserRequest.PrevEquity,
                PrevMonthBalance = mT4UserRequest.PrevMonthBalance,
                PrevMonthEquity = mT4UserRequest.PrevMonthEquity,
                PublicKey = mT4UserRequest.PublicKey,
                Regdate = mT4UserRequest.Regdate,
                SendReports = mT4UserRequest.SendReports,
                State = mT4UserRequest.State,
                Status = mT4UserRequest.Status,
                Taxes = mT4UserRequest.Taxes,
                Timestamp = mT4UserRequest.Timestamp,
                UserColor = mT4UserRequest.UserColor,
                ZipCode = mT4UserRequest.ZipCode,
                TradesHistories = mT4UserRequest.TradesHistories
            };
        }


    }
}
