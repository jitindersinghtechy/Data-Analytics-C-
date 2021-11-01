using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace S2TAnalytics.Common.Helper
{
    public static class ReadConfiguration
    {
        //Smtp Email keys
        public static int EmailTokenExpirationDays { get { return Convert.ToInt16(ConfigurationManager.AppSettings["EmailTokenExpirationDays"]); } }
        public static string HostName { get { return ConfigurationManager.AppSettings["HostName"]; } }
        public static string FromName { get { return ConfigurationManager.AppSettings["FromName"]; } }
        public static string FromEmail { get { return ConfigurationManager.AppSettings["FromEmail"]; } }
        public static string SmtpAccount { get { return ConfigurationManager.AppSettings["SmtpAccount"]; } }
        public static string SmtpPassword { get { return ConfigurationManager.AppSettings["SmtpPassword"]; } }
        public static string ConnectionString { get { return ConfigurationManager.AppSettings["MongoDBConectionString"]; } }
        public static string DataBaseName { get { return ConfigurationManager.AppSettings["MongoDBDatabaseName"]; } }
        public static string EnableSSL { get { return ConfigurationManager.AppSettings["EnableSSL"]; } }
        public static string TempFolderPath { get { return ConfigurationManager.AppSettings["TempFolderPath"]; } }
        public static int PageSize { get { return Convert.ToInt16(ConfigurationManager.AppSettings["PageSize"]); } }
        public static int SmtpServerPort { get { return Convert.ToInt16(ConfigurationManager.AppSettings["SmtpServerPort"]); } }
        public static string MT4Connector { get { return HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["MT4Connector"].ToString()); } }
    }
}
