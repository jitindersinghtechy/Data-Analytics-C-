using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Helper
{
    public static class MT4ELTHelper
    {
        public static ClrWrapper clrWrapper;
        public static void CreateWrapper(ClrWrapper _clrWrapper)
        {
            clrWrapper = _clrWrapper;
        }

        public static ConnectionParameters GetCredentials(int login, string password, string server)
        {
            return new ConnectionParameters { Login = login, Password = password, Server = server };
        }

        //Account Summary for all accounts
        public static List<UserRecord> GetAccountSummaryByAccount()
        {
            try
            {
                return clrWrapper.UsersRequest().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Account Summary by account
        public static UserRecord GetAccountSummaryByAccount(int account)
        {
            try
            {
                return clrWrapper.UsersRequest().Where(a => a.Login == account).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///To get list of Transactions of all accounts
        public static IList<TradeRecord> GetUserHistoryByAccount(int account, DateTime from, DateTime to)
        {
            try
            {
                return clrWrapper.TradesUserHistory(account, (uint)(Int32)(from.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, (uint)(Int32)(to.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Orders (Market and Pending both) by account
        public static IList<TradeRecord> GetOrdersByAccount(int account)
        {
            try
            {
                return clrWrapper.TradesRequest().Where(a => a.Login == account).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Orders (Market and Pending both) for all accounts
        public static IList<TradeRecord> GetOrdersByAll()
        {
            try
            {
                return clrWrapper.TradesRequest();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
