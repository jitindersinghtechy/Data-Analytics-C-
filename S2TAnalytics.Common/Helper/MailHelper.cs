using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace S2TAnalytics.Common.Helper
{
    public class MailHelper
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtmlBody { get; set; }
        public bool ReadBodyFromFile { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<string> AttachmentPath { get; set; }

        public string Month { get; set; }
        public string Year { get; set; }
        public bool SendEmail()
        {
            //try
            //{

            var message = new SendGridMessage();

            string htmlContent = string.Empty;

            message.AddTo(this.ToEmail);

            //message.From = new MailAddress("contest@oanda-toptrader.com", "Simple2Trade");
            message.From = new MailAddress("contact@tradeneo.io", "TradeNeo");
            message.Subject = this.Subject;
            message.Html = this.Body;
            if (this.AttachmentPath != null)
            {
                var a = this.AttachmentPath.Count;
                int count = 0;
                foreach (string filePath in this.AttachmentPath)
                {
                    if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(@filePath)))
                    {
                        var file = new MemoryStream(System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath(@filePath)));
                        // byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                        if (a == 1)
                            message.AddAttachment(file, "Invoice_" + Month + "_" + Year + ".pdf");
                        else
                        {
                            count++;
                            message.AddAttachment(file, "Invoice_" + Month + "_" + Year + "_" + count + ".pdf");
                        }
                    }
                }
            }
            var credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
            var transportWeb = new Web(credentials);

            if (transportWeb != null)
            {
                try
                {
                    transportWeb.DeliverAsync(message);
                }
                catch (Exception ex)
                {

                }
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
            return true;
            // }
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //    return false;
            //}
        }

        //public static void SendEmailWithField(EmailType eType, string to, SendMailParameter SM, string fullName = "", string uName = "", string url = "")
        //{
        //    string LanguageType = "en";
        //    PAMMDB db = new PAMMDB();
        //    try
        //    {


        //        var message = new SendGridMessage();

        //        string htmlContent = string.Empty;
        //        foreach (var Email in to.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            message.AddTo(Email);
        //            var EmailLanguage = db.User.Where(c => c.EmailAddress == to).Select(c => c.Lang).FirstOrDefault();
        //            if (EmailLanguage != null && EmailLanguage != "")
        //                LanguageType = EmailLanguage;
        //        }

        //        message.From = new MailAddress("contest@oanda-toptrader.com", "Simple2Trade");
        //        switch (eType)
        //        {
        //            case EmailType.TL_RegWelcome:
        //                message.Subject = "Welcome!";
        //                htmlContent = GetEmailHtml(eType, LanguageType);

        //                htmlContent = htmlContent.Replace("{{NAME}}", fullName == null || fullName.Trim().Length <= 0 ? uName : fullName);
        //                htmlContent = htmlContent.Replace("{{UNAME}}", fullName == null || fullName.Trim().Length <= 0 ? uName : fullName);
        //                message.Html = htmlContent.Replace("{{MYLINK}}", url);
        //                break;



        //        }

        //        var credentials = new NetworkCredential(ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
        //        var transportWeb = new Web(credentials);

        //        if (transportWeb != null)
        //        {
        //            transportWeb.DeliverAsync(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        public enum EmailType
        {
            //Tl_  represent, for whome this email is ? all start with 'Tl_' -> send to TradeLeader

            TL_RegWelcome = 1,


        }
        public class SendMailParameter
        {


            public string TLName { get; set; }

        }

    }
}
