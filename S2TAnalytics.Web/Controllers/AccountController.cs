using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Web.Models;
using System.Web.Http;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;

namespace S2TAnalytics.Web.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private IUserService _userService;
        private IAuthenticateService _authenticateService;
        public AccountController(IUserService userService,IAuthenticateService authenticateService)
        {
            _userService = userService;
            _authenticateService = authenticateService;
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            User user = new DAL.Models.User();
            return Ok();
        }

        [HttpPost]
        [Route("SignUp")]
        public IHttpActionResult SignUp(UserViewModel userVM)
        {

            var userModel = new UserViewModel().ToUserModel(userVM);
           var  response= _userService.AddNewUser(userModel);
            return Ok(response);
        }

        [HttpGet]
        [Route("Confirm/{email}/{code}")]
        public IHttpActionResult EmailConfirmed(string email,string code)
        {
            //var response= _authenticateService.CheckEmailToken(email, code);
            var response = _userService.SetEmailConfirmed(email, code);
            //User user = new DAL.Models.User();
            return Ok(response);
        }

        [HttpPost]
        [Route("SetPassword")]
        public IHttpActionResult SetPassword(UserViewModel user)
        {
            //var response= _authenticateService.CheckEmailToken(email, code);
            var response = _userService.SetPassword(user.EmailID, user.Password);
            //User user = new DAL.Models.User();
            return Ok(response);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IHttpActionResult ForgotPassword(UserViewModel user)
        {
            //var response= _authenticateService.CheckEmailToken(email, code);
            var response = _userService.ForgotPassword(user.EmailID);
            //User user = new DAL.Models.User();
            return Ok(response);
        }

        [HttpPost]
        [Route("ContactUs")]
        public IHttpActionResult ContactUs(ContactModel contactModel)
        {
            //var response= _authenticateService.CheckEmailToken(email, code);
            var response = _userService.ContactUs(contactModel);
            //User user = new DAL.Models.User();
            return Ok(response);
        }

        [HttpGet]
        [Route("GetData")]
        public IHttpActionResult GetData()
        {
            var response = _userService.GetData();
            return Ok(response);
        }
    }
}
