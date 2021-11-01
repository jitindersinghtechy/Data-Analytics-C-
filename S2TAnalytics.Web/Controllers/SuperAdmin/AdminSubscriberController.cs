using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace S2TAnalytics.Web.Controllers.SuperAdmin
{
    [Authorize]
    [RoutePrefix("api/AdminSubscriber")]
    public class AdminSubscriberController : BaseController
    {
        // GET: AdminSubscriber
        private IAdminSubscriberService _adminSubscriberService;

        public AdminSubscriberController(IAdminSubscriberService adminSubscriberService, IUserService userService) : base(userService)
        {
            _adminSubscriberService = adminSubscriberService;
        }

        [HttpPost]
        [Route("GetSubscribers")]
        public IHttpActionResult GetSubscribers(PageRecordModel pageRecordModel)
        {
            try
            {
                var response = _adminSubscriberService.GetSubscribers(pageRecordModel);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("GetInvoiceHistory/{userId}")]
        public IHttpActionResult GetInvoiceHistory(string userId,PageRecordModel pageRecordModel)
        {
            try
            {
                var response = _adminSubscriberService.GetInvoiceHistory(userId, pageRecordModel);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("GetUserRequest/{userId}")]
        public IHttpActionResult GetUserRequest(string userId, PageRecordModel pageRecordModel)
        {
            try
            {
                var response = _adminSubscriberService.GetUserRequest(userId, pageRecordModel);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("GetSubscriberList/{userID}")]
        public IHttpActionResult GetSubscriberList(String userID)
        {
            try
            {
                var response = _adminSubscriberService.GetSubscriberList(userID);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateSubscriberContactInfo")]
        public IHttpActionResult UpdateSubscriberContactInfo(UserModel user)
        {
            try
            {
                var response = _adminSubscriberService.UpdateSubscriberContactInfo(user);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("updateUserActivation")]
        public IHttpActionResult updateUserActivation(UserModel user)
        {
            try
            {
                var response = _adminSubscriberService.updateUserActivation(user);
                return Ok(new { response = response });
                //return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("sendOverdueReminder")]
        public IHttpActionResult sendOverdueReminder(UserModel user)
        {
            try
            {
                var response = _adminSubscriberService.sendOverdueReminder(user);
                return Ok(new { response = response });
                //return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("UpdatePlans/{userId}/{planId}")]
        public IHttpActionResult UpdatePlans(string userId, int planId)
        {
            try
            {
                var response = _adminSubscriberService.UpdatePlans(userId, planId);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateUserActivation/{isActive}")]
        public IHttpActionResult UpdateUserActivation(bool isActive,List<string> userIds)
        {
            try
            {
                var response = _adminSubscriberService.UpdateUserActivation(userIds,isActive);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("DownloadInvoice/{userId}")]
        public IHttpActionResult DownloadInvoice(string userId, List<Guid> invoiceIds)
        {
            try
            {
                List<string> filename = new List<string>();
                var invoiceModelList = _adminSubscriberService.DownloadInvoice(userId,invoiceIds);
                if (invoiceModelList != null && invoiceModelList.Count != 0)
                {
                    foreach (InvoiceModel invoiceModel in invoiceModelList)
                    {
                        filename.Add(GenerateInvoicePdf(invoiceModel));
                    }
                    return Ok(new { fileName = filename });
                }
                else
                {
                    return Ok(new { fileName = filename });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string GenerateInvoicePdf(InvoiceModel invoiceModel)
        {
            try
            {
                string HtmlString;
                HtmlString = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Invoice.html"));
                // HtmlString = HtmlString.Replace("{{ClientName}}", invoiceModel.ClientName);

                string loc = HttpContext.Current.Server.MapPath(@"/Images/icon/");
                var compiler = new Helper.HtmlTemplate();
                HtmlString = compiler.Render(HtmlString, new { ClientName = invoiceModel.ClientName, Country = invoiceModel.Country, EmailId=invoiceModel.EmailId, InvoiceDate=invoiceModel.InvoiceDate, InvoiceNumber=invoiceModel.InvoiceNumber, PlanName=invoiceModel.PlanName, Price=invoiceModel.Price, TotalAccounts=invoiceModel.TotalAccounts,ApplicationFees=invoiceModel.ApplicationFee, SubTotal= invoiceModel.SubTotal, Total=invoiceModel.Total,
                    CreditedAmount = invoiceModel.CreditedAmount, EmailImage = loc+"Email_icon.png",CountryImage= loc+"website_icon.png",Logo= loc+"Simple2tradelogo.png",PlanLogo= loc+invoiceModel.PlanLogo
                });
                //if (Convert.ToInt32(invoiceModel.CreditedAmount) == 0)
                //{
                //    var creditAmountString = HtmlString.Remove();
                //    HtmlString = HtmlString.Remove();
                //}
                string path = HttpContext.Current.Server.MapPath(@"/Invoices");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Document document = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                string fileName = invoiceModel.InvoiceNumber + "_" + Guid.NewGuid().ToString() + ".pdf";
                string filePathAndName = "/Invoices/" + fileName;
                string fileSavingPath = HttpContext.Current.Server.MapPath(@filePathAndName);
                FileStream fileStream = new FileStream(fileSavingPath, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(document, fileStream);
                document.Open();
                HTMLWorker hw = new HTMLWorker(document);
                StringReader sr = new StringReader(HtmlString);
                //hw.Parse(new StringReader(HtmlString.ToString()));
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                document.Close();
                hw.Dispose();
                sr.Dispose();
                fileStream.Close();
                writer.Close();

                return filePathAndName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("ExtendUserTrial/{ExtendDays}")]
        public IHttpActionResult ExtendUserTrial(int ExtendDays,List<string> userIds)
        {
            try
            {
                var response = _adminSubscriberService.ExtendUserTrial(userIds, ExtendDays);
                return Ok(new { response = response });
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateSubscriberSummary/{selectedPlanId}")]
        public IHttpActionResult UpdateSubscriberSummary(int selectedPlanId, UserModel user)
        {
            //var result = _adminSubscriberService.UpdateSubscriberSummary(selectedPlanId, user);

            return Ok();
        }

        [HttpPost]
        [Route("GetUserNotifications")]
        public IHttpActionResult GetUserNotifications(PageRecordModel pageRecordModel)
        {
            try
            {
                var response = _adminSubscriberService.GetUserNotifications(pageRecordModel);
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("SendReminders/{reminderId}")]
        public IHttpActionResult SendReminders(List<string> userIds,int reminderId)
        {
            try
            {
                var response = _adminSubscriberService.SendReminders(userIds, reminderId);
                return Ok(new { response = response });
            }

            catch (Exception ex)
            {
                throw;
            }
        }

    }
}