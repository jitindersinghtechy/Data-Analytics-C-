using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web;
using System.IO;
using S2TAnalytics.Web.Helper;
//using mvc = System.Web.Mvc;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/UserAccount")]
    public class UserAccountController : BaseController
    {
        public readonly IUserAccountService _userAccountService;
        public UserAccountController(IUserAccountService userAccountService, IUserService userService) : base(userService)
        {
            _userAccountService = userAccountService;
        }

        [HttpPost]
        [Route("SaveCard")]
        public IHttpActionResult SaveCard(CardModel cardModel)
        {
            return Ok(_userAccountService.SaveCard(UserID, cardModel));
        }

        [HttpGet]
        [Route("GetDefaultData")]
        public IHttpActionResult GetDefaultData()
        {
            return Ok(_userAccountService.GetDefaultData(UserID,RoleID));
        }

        [HttpPost]
        [Route("DeleteCards")]
        public IHttpActionResult DeleteCards(List<Guid> CardIds)
        {
            return Ok(_userAccountService.DeleteCards(CardIds, UserID));
        }

        [HttpGet]
        [Route("GetPlanDetails")]
        public IHttpActionResult GetPlanDetails()
        {
            var response = _userAccountService.GetPlanDetails(UserID);
            return Ok(response);
        }



        [HttpGet]
        [Route("GetPromocodeDiscount/{PromoCode}")]
        public IHttpActionResult GetPromocodeDiscount(string PromoCode)
        {
            var response = _userAccountService.GetPromocodeDiscount(PromoCode);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetSubscriptionPlans")]
        public IHttpActionResult GetSubscriptionPlans()
        {
            var response = _userAccountService.GetSubscriptionPlans(UserID);
            return Ok(response);
        }

        [HttpPost]
        [Route("RequestPlan")]
        public IHttpActionResult RequestPlan(RequestPlanModel requestPlanModel)
        {
            var response = _userAccountService.RequestPlan(UserID, requestPlanModel.PlanId, requestPlanModel.TermLengthId, requestPlanModel.Promocode);
            return Ok(response);
        }

        [HttpPost]
        [Route("ActivateCard/{cardId}/{isActive}")]
        public IHttpActionResult ActivateCard(string cardId, bool isActive)
        {
            var response = _userAccountService.ActivateCard(cardId, UserID, isActive);
            return Ok(response);
        }

        [HttpPost]
        [Route("SaveBillingAddress")]
        public IHttpActionResult SaveBillingAddress(UserBillingInfoModel userBillingInfoModel)
        {
             var response = _userAccountService.SaveBillingAddress(UserID, userBillingInfoModel);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetUserBillingAddresses")]
        public IHttpActionResult GetUserBillingAddresses()
        {
            var response = _userAccountService.GetUserBillingAddresses(UserID);
            return Ok(response);
        }

        [HttpPost]
        [Route("ActivateBillingAddress/{addressId}/{isActive}")]
        public IHttpActionResult ActivateBillingAddress(string addressId, bool isActive)
        {
            var response = _userAccountService.ActivateBillingAddress(addressId, UserID, isActive);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetUserBillingAddressById/{addressId}")]
        public IHttpActionResult GetUserBillingAddressById(string addressId)
        {
            var response = _userAccountService.GetUserBillingAddressById(UserID, addressId);
            return Ok(response);
        }

        [HttpPost]
        [Route("ChangeSettings/{emailNotificationSettingsId}/{hasAccess}")]
        public IHttpActionResult ChangeSettings(int emailNotificationSettingsId, bool hasAccess)
        {
            var response = _userAccountService.ChangeSettings(emailNotificationSettingsId, UserID, hasAccess);
            return Ok(response);
        }

        [HttpPost]
        [Route("ChangeUserDetails")]
        public IHttpActionResult ChangeUserDetails(UserModel userModel)
        {
            var response = _userAccountService.ChangeUserDetails(userModel, UserID);
            return Ok(response);
        }

        [HttpPost]
        [Route("ChangePassword/{email}/{oldPassword}/{newPassword}")]
        public IHttpActionResult ChangePassword(string email, string oldPassword, string newPassword)
        {
            var response = _userAccountService.ChangePassword(email, oldPassword, newPassword);
            return Ok(response);
        }

        [HttpPost]
        [Route("GenerateInvoice/{invoiceMonth}/{invoiceYear}")]
        public IHttpActionResult GenerateInvoice(string invoiceMonth, string invoiceYear)
        {
            List<string> filename = new List<string>();
            var invoiceModelList = _userAccountService.GenerateInvoice(UserID, invoiceMonth, invoiceYear);
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
                return Ok(new { fileName = filename});
            }
            //return Ok(response);
            
        }
        private string GenerateInvoicePdf(InvoiceModel invoiceModel)
        {
            try
            {
                string HtmlString;
                HtmlString = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/Invoice.html"));
                // HtmlString = HtmlString.Replace("{{ClientName}}", invoiceModel.ClientName);

                string loc = HttpContext.Current.Server.MapPath(@"/Images/icon/");
                var compiler = new HtmlTemplate();
                HtmlString = compiler.Render(HtmlString, new { ClientName = invoiceModel.ClientName, Country = invoiceModel.Country, EmailId=invoiceModel.EmailId, InvoiceDate=invoiceModel.InvoiceDate, InvoiceNumber=invoiceModel.InvoiceNumber, PlanName=invoiceModel.PlanName, Price=invoiceModel.Price, TotalAccounts=invoiceModel.TotalAccounts,ApplicationFees=invoiceModel.ApplicationFee, SubTotal= invoiceModel.SubTotal, Total=invoiceModel.Total,
                    CreditedAmount = invoiceModel.CreditedAmount, ServiceFee= invoiceModel.ServiceFee, InfrastructureCost=invoiceModel.InfrastructureCost, Discount=invoiceModel.Discount, EmailImage = loc+"Email_icon.png",CountryImage= loc+"website_icon.png",Logo= loc+"Simple2tradelogo.png",PlanLogo= loc+invoiceModel.PlanLogo
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
        [Route("EmailInvoice/{invoiceMonth}/{invoiceYear}")]
        public IHttpActionResult EmailInvoice(string invoiceMonth, string invoiceYear,List<string> files)
        {
            return Ok(_userAccountService.EmailInvoice(files, UserID,invoiceMonth, invoiceYear));
        }

        [HttpGet]
        [Route("PreRequestPlan/{planId}")]
        public IHttpActionResult PreRequestPlan(int planId)
        {
            var response = _userAccountService.PreRequestPlan(UserID, planId);
            return Ok(response);
        }
    }
}