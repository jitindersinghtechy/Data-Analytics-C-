using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Infrastructure.Services;
using System.Configuration;
using S2TAnalytics.DAL.Models;
using MongoDB.Bson;
using System.IO;
//using System.Xml;
using HtmlAgilityPack;
using S2TAnalytics.Infrastructure.Models;
using SendGrid;
using S2TAnalytics.Common.Enums;

namespace PerformanceComparisonNotification
{
    public class Program
    {
        private static UnitOfWork unitOfWork = new UnitOfWork(ConfigurationManager.AppSettings["ELTConnectionString"], ConfigurationManager.AppSettings["DBName"]);
        private static PerformanceComparisonService _performanceComparisonService = new PerformanceComparisonService(unitOfWork);

        static void Main(string[] args)
        {
            GetUserAndPerformance();
        }

        private static void GetUserAndPerformance()
        {
            try
            {
                var users = _performanceComparisonService.GetAllUsers();
                var rootDirectory = Path.Combine(Environment.CurrentDirectory.Remove(Environment.CurrentDirectory.LastIndexOf("bin")));
                //var mainTemplate = File.ReadAllText(rootDirectory, "Templates\\performanceComparision.html");
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(rootDirectory + "Templates\\performanceComparision.html");

                foreach (var user in users)
                {
                    var dataSourceIds = GetDataSourceIds(user);
                    var timelines = GetTimelineIds(user);
                    foreach (var timeline in timelines)
                    {

                        GetTopFivePerformersByDataSourceIds(dataSourceIds, timeline.Value, user.Id, doc, timeline.Key);
                        var html = doc.DocumentNode.InnerHtml;
                        //var html = doc._lastparentnode.InnerHtml;
                        //html += GetPinnedPerformersByDataSourceIds(user.Id, timeline.Value);
                        var mail = sendMail(user, html, timeline.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static List<ObjectId> GetDataSourceIds(User user)
        {
            var result = new List<ObjectId>();

            if (user.RoleID == 2)
            {
                result = _performanceComparisonService.GetDataSourceIdsByOrganisationId(user.OrganizationID);
            }
            else
            {
                result = user.DatasourceIds;
            }
            return result;
        }

        private static Dictionary<string, Dictionary<string, int>> GetTimelineIds(User user)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();
            var startAndEndTimelines = new Dictionary<string, int>();
            var today = DateTime.Today.Date;
            var startingDateofMonth = new DateTime(today.Year, today.Month, 1);
            var startingDateofQuater = GetStartingDateofQuater(today);
            var startingDateofHalfYear = GetStartingDateofHalfYear(today);
            var startingDateofYear = new DateTime(today.Year, 1, 1);

            //Today 
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.Daily))
            {
                startAndEndTimelines.Add("StartTimelineId", 44);
                startAndEndTimelines.Add("EndTimelineId", 43);
                result.Add("Daily", startAndEndTimelines);
            }

            //Weekly
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.Weekly))
            {
                startAndEndTimelines = new Dictionary<string, int>();
                if (today.DayOfWeek == DayOfWeek.Saturday)
                {
                    startAndEndTimelines.Add("StartTimelineId", 30);
                    startAndEndTimelines.Add("EndTimelineId", 29);
                    result.Add("Weekly", startAndEndTimelines);
                }
            }

            //Monthly
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.Monthly))
            {
                if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.Daily))
                {
                    startAndEndTimelines = new Dictionary<string, int>();
                    if (today == startingDateofMonth)
                    {
                        startAndEndTimelines.Add("StartTimelineId", 18);
                        startAndEndTimelines.Add("EndTimelineId", 17);
                        result.Add("Monthly", startAndEndTimelines);
                    }
                }
            }

            //Quaterly
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.Quarterly))
            {
                startAndEndTimelines = new Dictionary<string, int>();
                if (today == startingDateofQuater)
                {
                    startAndEndTimelines.Add("StartTimelineId", 3);
                    startAndEndTimelines.Add("EndTimelineId", 3);
                    result.Add("Quarterly", startAndEndTimelines);
                }
            }

            //Half Yearly
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.HalfYearly))
            {
                startAndEndTimelines = new Dictionary<string, int>();
                if (today == startingDateofHalfYear)
                {
                    startAndEndTimelines.Add("StartTimelineId", 2);
                    startAndEndTimelines.Add("EndTimelineId", 2);
                    result.Add("HalfYearly", startAndEndTimelines);
                }
            }

            //Yearly
            if (user.UserEmailNotificationSettings.Any(x => x.NotificationSettingsId == (int)EmailNotificationSettingsEnum.HalfYearly))
            {
                startAndEndTimelines = new Dictionary<string, int>();
                if (today == startingDateofYear)
                {
                    startAndEndTimelines.Add("StartTimelineId", 1);
                    startAndEndTimelines.Add("EndTimelineId", 1);
                    result.Add("Yearly", startAndEndTimelines);
                }
            }
            return result;
        }

        public static DateTime GetStartingDateofQuater(DateTime dtNow)
        {
            var result = new DateTime();
            if (dtNow.Month >= 1 && dtNow.Month <= 3)
            {
                result = DateTime.Parse("1/1/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 4 && dtNow.Month < 7)
            {
                result = DateTime.Parse("4/1/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 7 && dtNow.Month < 10)
            {
                result = DateTime.Parse("7/1/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 10 && dtNow.Month <= 12)
            {
                result = DateTime.Parse("10/1/" + dtNow.Year.ToString());
            }
            return result;
        }

        public static DateTime GetStartingDateofHalfYear(DateTime dtNow)
        {
            var result = new DateTime();
            if (dtNow.Month >= 1 && dtNow.Month < 4)
            {
                result = DateTime.Parse("1/1/" + dtNow.Year.ToString());
            }
            else if (dtNow.Month >= 7 && dtNow.Month <= 12)
            {
                result = DateTime.Parse("7/1/" + dtNow.Year.ToString());
            }
            return result;
        }

        private static string GetTopFivePerformersByDataSourceIds(List<ObjectId> dataSourceIds, Dictionary<string, int> timeline, ObjectId userId,
                                                                  HtmlAgilityPack.HtmlDocument doc, string type)
        {
            var sortBy = ConfigurationManager.AppSettings["SortPerformersBy"];
            var topPerformersComparisionModels = _performanceComparisonService.GetTopFivePerformersByDataSourceIds(dataSourceIds, timeline, sortBy);
            var toppinnedPerformersComparisionModels = _performanceComparisonService.GetTopPinnedPerformersByUserId(userId, timeline, sortBy);
            var lowpinnedPerformersComparisionModels = _performanceComparisonService.GetLowPinnedPerformersByUserId(userId, timeline, sortBy);
            generateHtml(sortBy, topPerformersComparisionModels, toppinnedPerformersComparisionModels, lowpinnedPerformersComparisionModels, doc, type);
            return "";
            //var topPerformers = topPerformersComparisionModels.Where(x => x.Key == "PerformerModels").Select(x => x.Value).FirstOrDefault();
            //var comparePerformerModel = topPerformersComparisionModels.Where(x => x.Key == "ComparePerformerModels").Select(x => x.Value).FirstOrDefault();
            //return generateHtml(sortBy, topPerformers, comparePerformerModel, doc);
        }

        //private static string GetPinnedPerformersByDataSourceIds(ObjectId userId, Dictionary<string, int> timeline)
        //{
        //    var sortBy = ConfigurationManager.AppSettings["SortPerformersBy"];
        //    var pinnedPerformersComparisionModels = _performanceComparisonService.GetPinnedPerformersByUserId(userId, timeline, sortBy);
        //    if (pinnedPerformersComparisionModels != null)
        //    {
        //        var pinnedPerformers = pinnedPerformersComparisionModels.Where(x => x.Key == "PerformerModels").Select(x => x.Value).FirstOrDefault();
        //        var comparePerformerModel = pinnedPerformersComparisionModels.Where(x => x.Key == "ComparePerformerModels").Select(x => x.Value).FirstOrDefault();
        //        return generateHtml(sortBy, pinnedPerformers, comparePerformerModel);
        //    }
        //    else
        //    {
        //        return "No Pinned User Found.";
        //    }
        //}

        private static string generateHtml(string sortBy, Dictionary<string, List<PerformersModel>> topPerformersComparisionModels, Dictionary<string, List<PerformersModel>> toppinnedPerformersComparisionModels,
                                           Dictionary<string, List<PerformersModel>> lowpinnedPerformersComparisionModels, HtmlAgilityPack.HtmlDocument doc, string type)
        {
            HtmlNode reportHeader = doc.DocumentNode.SelectSingleNode("//*[@class=\"_report-header\"]");
            reportHeader.InnerHtml = type.ToUpper() + " REPORT";

            if (type.ToLower() == "daily")
            {
                HtmlNode type1 = doc.DocumentNode.SelectSingleNode("//*[@class=\"_timeline\"]");
                type1.InnerHtml = "DAY";
                HtmlNode reportTime = doc.DocumentNode.SelectSingleNode("//*[@class=\"_report-time\"]");
                reportTime.InnerHtml = DateTime.Today.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            if (type.ToLower() == "monthly")
            {
                HtmlNode type1 = doc.DocumentNode.SelectSingleNode("//*[@class=\"_timeline\"]");
                type1.InnerHtml = "MONTH";
                HtmlNode reportTime = doc.DocumentNode.SelectSingleNode("//*[@class=\"_report-time\"]");
                reportTime.InnerHtml = DateTime.Today.AddMonths(-1).ToString("MMM") + "-" + DateTime.Today.ToString("MMM") + " " + DateTime.Now.Year;
            }

            #region Top Performers
            var topPerformers = topPerformersComparisionModels.Where(x => x.Key == "PerformerModels").Select(x => x.Value).FirstOrDefault();
            var comparePerformerModel = topPerformersComparisionModels.Where(x => x.Key == "ComparePerformerModels").Select(x => x.Value).FirstOrDefault();
            foreach (var topPerformer in topPerformers.Select((x, i) => new { Value = x, Index = i }))
            {
                var performer = topPerformer.Value;
                var index = topPerformer.Index;
                var compareModel = comparePerformerModel.Where(x => x.AccountDetailId == performer.AccountDetailId).FirstOrDefault();
                var isGrowth = false;
                if (sortBy == "MaxDD")
                {
                    isGrowth = performer.DD > compareModel.DD;
                }
                else if (sortBy == "WinRate")
                {
                    isGrowth = performer.WINRate > compareModel.WINRate;
                }
                else if (sortBy == "SharpRatio")
                {
                    isGrowth = Convert.ToDouble(performer.SharpRatio) > Convert.ToDouble( compareModel.SharpRatio);
                }
                else
                {
                    isGrowth = performer.ROI > compareModel.ROI;
                }

                if (index == 0)
                {
                    HtmlNode performerName = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-name\"]");
                    performerName.InnerHtml = performer.PerformerName;
                    //performerName.InnerText = performer.PerformerName;

                    HtmlNode performerAccount = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-account\"]");
                    performerAccount.InnerHtml = performer.AccountNumber;

                    HtmlNode performerEquity = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-equity\"]");
                    performerEquity.InnerHtml = performer.NAV.ToString();

                    HtmlNode performerWin = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-win\"]");
                    performerWin.InnerHtml = performer.WINRate.ToString();

                    HtmlNode performerROI = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-roi\"]");
                    performerROI.InnerHtml = performer.ROI.ToString();

                    HtmlNode performerDetailsLink = doc.DocumentNode.SelectSingleNode("//*[@class=\"_performer-details\"]");
                    performerDetailsLink.Attributes.Where(x => x.Name == "href").FirstOrDefault().Value = "http://analyticsv1.azurewebsites.net/#/UserDetails/" + performer.AccountDetailId;
                }

                HtmlNode parent = doc.DocumentNode.SelectSingleNode("//*[@class=\"container590 _top-performer-" + index + "\"]");

                var performanceIcon = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_performer-performance")).FirstOrDefault();
                performanceIcon.Attributes.Where(x => x.Name == "src").FirstOrDefault().Value = "http://analyticsv1.azurewebsites.net/Images/templateImages/" + (isGrowth ? "UP_arrow.png" : "Down_arrow.png");

                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-name")).FirstOrDefault().InnerHtml = performer.PerformerName;
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-account")).FirstOrDefault().InnerHtml = "(" + performer.AccountNumber + ")";
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-1")).FirstOrDefault().InnerHtml = performer.ROI.ToString();
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml;
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-2")).FirstOrDefault().InnerHtml = performer.AccountNumber;
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml;
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-3")).FirstOrDefault().InnerHtml = performer.NAV.ToString();
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml;
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-4")).FirstOrDefault().InnerHtml = performer.WINRate.ToString();
                parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml;
            }
            #endregion

            #region Top Pinned
            if (toppinnedPerformersComparisionModels != null)
            {
                var topPinnedPerformers = toppinnedPerformersComparisionModels.Where(x => x.Key == "PerformerModels").Select(x => x.Value).FirstOrDefault();
                var comparetopPinnedPerformerModel = toppinnedPerformersComparisionModels.Where(x => x.Key == "ComparePerformerModels").Select(x => x.Value).FirstOrDefault();
                foreach (var topPinned in topPinnedPerformers.Select((x, i) => new { Value = x, Index = i }))
                {
                    var performer = topPinned.Value;
                    var index = topPinned.Index;
                    var compareModel = comparetopPinnedPerformerModel.Where(x => x.AccountDetailId == performer.AccountDetailId).FirstOrDefault();
                    var isGrowth = false;
                    if (sortBy == "MaxDD")
                    {
                        isGrowth = performer.DD > compareModel.DD;
                    }
                    else if (sortBy == "WinRate")
                    {
                        isGrowth = performer.WINRate > compareModel.WINRate;
                    }
                    else if (sortBy == "SharpRatio")
                    {
                        isGrowth = Convert.ToDouble(performer.SharpRatio) > Convert.ToDouble(compareModel.SharpRatio);
                    }
                    else
                    {
                        isGrowth = performer.ROI > compareModel.ROI;
                    }
                    HtmlNode parent = doc.DocumentNode.SelectSingleNode("//*[@class=\"container590 _top-pinned-" + index + "\"]");

                    var performanceIcon = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_performer-performance")).FirstOrDefault();
                    performanceIcon.Attributes.Where(x => x.Name == "src").FirstOrDefault().Value = "http://analyticsv1.azurewebsites.net/Images/templateImages/" + (isGrowth ? "UP_arrow.png" : "Down_arrow.png");

                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-name")).FirstOrDefault().InnerHtml = performer.PerformerName;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-account")).FirstOrDefault().InnerHtml = "(" + performer.AccountNumber + ")";
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-1")).FirstOrDefault().InnerHtml = performer.ROI.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-2")).FirstOrDefault().InnerHtml = performer.AccountNumber;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-3")).FirstOrDefault().InnerHtml = performer.NAV.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-4")).FirstOrDefault().InnerHtml = performer.WINRate.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml;

                }
            }


            #endregion

            #region Low Pinned
            if (lowpinnedPerformersComparisionModels != null)
            {
                var lowPinnedPerformers = lowpinnedPerformersComparisionModels.Where(x => x.Key == "PerformerModels").Select(x => x.Value).FirstOrDefault();
                var comparelowPinnedPerformerModel = lowpinnedPerformersComparisionModels.Where(x => x.Key == "ComparePerformerModels").Select(x => x.Value).FirstOrDefault();
                foreach (var lowPinned in lowPinnedPerformers.Select((x, i) => new { Value = x, Index = i }))
                {
                    var performer = lowPinned.Value;
                    var index = lowPinned.Index;
                    var compareModel = comparelowPinnedPerformerModel.Where(x => x.AccountDetailId == performer.AccountDetailId).FirstOrDefault();
                    var isGrowth = false;
                    if (sortBy == "MaxDD")
                    {
                        isGrowth = performer.DD > compareModel.DD;
                    }
                    else if (sortBy == "WinRate")
                    {
                        isGrowth = performer.WINRate > compareModel.WINRate;
                    }
                    else if (sortBy == "SharpRatio")
                    {
                        isGrowth = Convert.ToDouble(performer.SharpRatio) > Convert.ToDouble(compareModel.SharpRatio);
                    }
                    else
                    {
                        isGrowth = performer.ROI > compareModel.ROI;
                    }
                    HtmlNode parent = doc.DocumentNode.SelectSingleNode("//*[@class=\"container590 _low-pinned-" + index + "\"]");

                    var performanceIcon = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_performer-performance")).FirstOrDefault();
                    performanceIcon.Attributes.Where(x => x.Name == "src").FirstOrDefault().Value = "http://analyticsv1.azurewebsites.net/Images/templateImages/" + (isGrowth ? "UP_arrow.png" : "Down_arrow.png");

                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-name")).FirstOrDefault().InnerHtml = performer.PerformerName;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-account")).FirstOrDefault().InnerHtml = "(" + performer.AccountNumber + ")";
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-1")).FirstOrDefault().InnerHtml = performer.ROI.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-1")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-2")).FirstOrDefault().InnerHtml = performer.AccountNumber;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-2")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-3")).FirstOrDefault().InnerHtml = performer.NAV.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-3")).FirstOrDefault().InnerHtml;
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-value-4")).FirstOrDefault().InnerHtml = performer.WINRate.ToString();
                    parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml = parent.Descendants().Where(x => x.Attributes.Any(y => y.Name == "class" && y.Value == "_top-performer-label-4")).FirstOrDefault().InnerHtml;
                }
            }

            #endregion

            return "";
        }

        public static bool sendMail(User user, string html, string type)
        {
            try
            {
                SendEmail(user, html, type).Wait();
                //MailHelper mailHelper = new MailHelper();
                ////mailHelper.ToEmail = user.EmailID;
                //mailHelper.ToEmail = "mohit@mailinator.com";
                ////mailHelper.ToEmail = "mohit.dhawan@techbitsolution.com";

                //mailHelper.Subject = "Top Performers " + type;
                //mailHelper.Body = html;

                //mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static async Task SendEmail(User user, string html, string type)
        {
            //try
            //{
            var message = new SendGridMessage();

            string htmlContent = string.Empty;

            message.AddTo("111@mailinator.com");

            message.From = new MailAddress("contest@oanda-toptrader.com", "Simple2Trade");
            message.Subject = "Top Performers " + type;
            message.Html = html;

            var credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
            var transportWeb = new Web(credentials);

            if (transportWeb != null)
            {
                await transportWeb.DeliverAsync(message);
            }

            //StringBuilder strBody = new StringBuilder();
            //string MailTo = this.ToEmail;

            //MailMessage mailObj = new MailMessage();
            //mailObj.From = new MailAddress(ReadConfiguration.FromEmail, ReadConfiguration.FromName);
            //mailObj.Subject = this.Subject;
            //mailObj.Body = this.Body;
            //mailObj.To.Add(this.ToEmail);
            //if (this.AttachmentPath != null)
            //    foreach (string filePath in this.AttachmentPath)
            //    {
            //        if (System.IO.File.Exists(filePath))
            //        {
            //            System.Net.Mail.Attachment attachment;
            //            attachment = new System.Net.Mail.Attachment(filePath);
            //            mailObj.Attachments.Add(attachment);
            //        }
            //    }
            //mailObj.IsBodyHtml = true;
            //mailObj.Priority = MailPriority.High;
            //SmtpClient SMTPServer = new SmtpClient(ReadConfiguration.HostName);
            //SMTPServer.Port = Convert.ToInt32(ReadConfiguration.SmtpServerPort);
            //SMTPServer.UseDefaultCredentials = false;
            ////  SMTPServer.Credentials = auth;
            //SMTPServer.Credentials = new System.Net.NetworkCredential(ReadConfiguration.SmtpAccount, ReadConfiguration.SmtpPassword);
            //SMTPServer.EnableSsl = Convert.ToBoolean(ReadConfiguration.EnableSSL);
            //SMTPServer.Send(mailObj);
            //return true;
            // }
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //    return false;
            //}
        }

    }
}
