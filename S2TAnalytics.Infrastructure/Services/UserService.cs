using MongoDB.Bson;
using MongoDB.Driver;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Common.Utilities;
using S2TAnalytics.DAL.Interfaces;

using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.Repository;

using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace S2TAnalytics.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly DAL.Repository.BaseRepository _baseRepository;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //public User Get(int id)
        //{

        //    try
        //    {

        //        //var user = _unitOfwork. GetCollection<User>().Find(Builders<User>.Filter.Eq("UserID", id)).FirstOrDefault();
        //        var user = _unitOfwork.UserRepository.GetAll().ToList();
        //            //GetOne(Builders<User>.Filter.Eq("UserID", id));

        //    }
        //    catch (System.Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return new User();
        //}
        public List<UserModel> GetAll()
        {
            var users = _unitOfWork.UserRepository.GetAll();
            return new UserModel().ToUserModel(users.ToList());
        }
        public ServiceResponse GetData()
        {
            var queries = new List<EnumHelper>();
            foreach (QueryTypeEnum query in Enum.GetValues(typeof(QueryTypeEnum)))
            {
                var source = new EnumHelper
                {
                    stringValue = ((int)query).Encrypt(),
                    DisplayName = ((QueryTypeEnum)Enum.ToObject(typeof(QueryTypeEnum), (int)query)).GetEnumDisplayName()
                };
                //timeLines.Add(timeline);
                queries.Add(source);
            }

            return new ServiceResponse { Data = queries, Success = true };
        }
        public ServiceResponse SetEmailConfirmed(string email, string token)
        {
            //var IsEmailTokenas = _unitOfWork.UserRepository.GetAll().ToList();
            var IsEmailToken = _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email && (m.EmailToken != null || m.EmailTokenExpiration != null) && !m.IsEmailConfirmed && m.EmailTokenExpiration >= DateTime.UtcNow && m.EmailToken.Equals(token));
            if (IsEmailToken)
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email).FirstOrDefault();
                user.IsEmailConfirmed = true;
                user.EmailTokenExpiration = null;
                user.EmailToken = null;
                _unitOfWork.UserRepository.Update(user);

                var isSentWelcome = this.SendWelcomeEmail(user);

                return new ServiceResponse { Message = UserMessage.Email_Confirmed, Success = true };
            }
            return new ServiceResponse { Message = ErrorMessage.Link_Expired, Success = false };
        }
        public ServiceResponse SetPassword(string email, string Password)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email).FirstOrDefault();
            if (user != null)
            {
                if (user.EmailToken == null) { return new ServiceResponse { Message = ErrorMessage.Invalid_Token, Success = false }; }
                if (user.EmailTokenExpiration == null) { return new ServiceResponse { Message = UserMessage.EmailToken_Exipred, Success = false }; }

                user.Password = Password.PasswordEncrypt();
                user.IsEmailConfirmed = true;
                user.EmailToken = null;
                user.EmailTokenExpiration = null;
                _unitOfWork.UserRepository.Update(user);

                var isSentWelcome = this.SendWelcomeEmail(user);
                return new ServiceResponse { Message = UserMessage.Set_Password, Success = true };
            }
            else
            {
                return new ServiceResponse { Message = UserMessage.Password_Error, Success = false };
            }
        }
        public ServiceResponse ForgotPassword(string email)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email).FirstOrDefault();
            if (user != null)
            {
                user.EmailTokenExpiration = DateTime.UtcNow.AddDays(ReadConfiguration.EmailTokenExpirationDays);
                user.EmailToken = CommonFunction.GenerateToken();
                _unitOfWork.UserRepository.Update(user);
                var isSent = this.SendForgotEmail(user);
                return new ServiceResponse { Message = UserMessage.Forgot_Mail_Send, Success = true };
            }
            else
            {
                return new ServiceResponse { Message = UserMessage.Email_NotConfirmed, Success = false };
            }
        }
        public ServiceResponse ContactUs(ContactModel contactModel)
        {
            var contactUs = new ContactModel().ToContactUs(contactModel);
            _unitOfWork.ContactUsRepository.Add(contactUs);
            MailHelper mailHelper = new MailHelper();
            var admin = _unitOfWork.UserRepository.GetAll().Where(x => x.RoleID == 1).FirstOrDefault();
            if (admin != null)
            {
                //int queryId = Convert.ToInt32(contactModel.QueryType.Decrypt());
                //mailHelper.ToEmail = contactUs.Email;
                mailHelper.ToEmail = admin.EmailID;
                mailHelper.Subject = "Thank you for getting in touch!";
                mailHelper.Body = CommonFunction.ContactUsMailBody(contactModel.Name, contactModel.Query);
                mailHelper.SendEmail();

                mailHelper.ToEmail = contactModel.EmailId;
                mailHelper.Subject = "Thank you for getting in touch!";
                mailHelper.Body = CommonFunction.ContactUsMailBody(contactModel.Name, contactModel.Query);
                mailHelper.SendEmail();
            }
            return new ServiceResponse { Success = true, Message = "Request sent successfully. We will contact you soon!" };
        }
        public User GetUserByEmail(string email)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();

            return user;
        }
        public bool IsEmailExist(string email)
        {
            return _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email);
            //var IsUser= _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email);
            //return IsUser;
        }
        //public bool SetEmailConfirmed(string email,string token)
        //{

        //    var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email).FirstOrDefault();
        //    user.IsEmailConfirmed = true;
        //    //_unitOfWork.UserRepository.UpdateOne();
        //    return user.IsEmailConfirmed;

        //}
        public ServiceResponse AddNewUser(UserModel userModel)
        {

            if (!IsEmailExist(userModel.EmailID))
            {
                var user = new UserModel().ToUser(userModel);
                user.EmailTokenExpiration = DateTime.Now.AddDays(ReadConfiguration.EmailTokenExpirationDays);
                user.EmailToken = CommonFunction.GenerateToken();
                user.RoleID = 2;
                var plan = _unitOfWork.SubscriptionPlanRepository.GetAll().Where(p => p.PlanID == 2).Single();
                user.UserPlans.Add(new UserPlan() { Days = plan.Days, IsActive = true, PlanID = plan.PlanID, Price = plan.Price, StartedDate = DateTime.Now });
                user.UserEmailNotificationSettings.Add(new UserEmailNotificationSettings() { NotificationSettingsId = 1, HasAccess = true });
                user.UserEmailNotificationSettings.Add(new UserEmailNotificationSettings() { NotificationSettingsId = 2, HasAccess = true });

                user.UserEmailNotificationSettings.Add(new UserEmailNotificationSettings() { NotificationSettingsId = 3, HasAccess = true });
                user.IsActive = true;
                user.IsEmailConfirmed = false;
                // user.Datasources.Add(userDataSource);
                // user.Datasources.Add(userDataSource1);


                _unitOfWork.UserRepository.Add(user);

                var isSent = this.SendEmail(user);
                if (isSent)
                {
                    // AddDummyAcountDetails(user.OrganizationID);
                }
                userModel = new UserModel().ToUserModel(user);

                return new ServiceResponse { Data = userModel, Success = true, Message = CommonMessage.SentEmail };
            }
            return new ServiceResponse { Data = userModel, Success = false, Message = ErrorMessage.Email_Already_Exist };
        }
        public bool SendForgotEmail(User model)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "Forgot Password";
                string AccountLoginUrl = CommonFunction.GetSetPasswordUrl(model.EmailID, model.EmailToken, 1);
                mailHelper.Body = CommonFunction.ForgotPasswordMailBody(AccountLoginUrl, model.FirstName + " " + model.LastName);

                mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SendEmail(User model)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "Confirm Email";
                string AccountLoginUrl = CommonFunction.GetResetPasswordUrl(model.EmailID, model.EmailToken, 1);
                mailHelper.Body = CommonFunction.ConfigureNewPasswordMailBody(AccountLoginUrl, model.FirstName + " " + model.LastName);

                mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendWelcomeEmail(User model)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "Welcome";
                string AccountLoginUrl = CommonFunction.GetWelcomeUrl();
                mailHelper.Body = CommonFunction.WelcomeMailBody(AccountLoginUrl, model.FirstName + " " + model.LastName);

                mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendOTPEmail(User model)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "OTP";
                mailHelper.Body = CommonFunction.OTPMailBody(model.FirstName + " " + model.LastName, model.OTP);
                //mailHelper.Body = "your OTP is " + model.OTP;

                mailHelper.SendEmail();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public void AddDummyAcountDetails(Guid orgID)
        {
            var countries = new List<string> { "AU", "AU", "AU", "AU", "IN", "IN", "IN", "IN", "US", "US", "US", "US" };
            var userGroup = new List<string> { "AUS_GROUP", "AUS_GROUP", "AUS_GROUP", "AUS_GROUP", "IND_GROUP", "IND_GROUP", "IND_GROUP", "IND_GROUP", "US_GROUP", "US_GROUP", "US_GROUP", "US_GROUP" };
            var names = new List<string> { "Aadesh", "Aadarsh", "Aadhishankar", "Aadit", "Aagman", "Aagney", "Aahva", "Aakarshan", "Abhay", "Abhi", "Abhijat", "Abhijit", "Abhik", "Abhilash", "Abhinandan", "Abhinav", "Abhinivesh", "Abhiram", "Abhiroop", "Abhirut", "Abhisar", "Abhishek", "Abhyas", "Achal", "Achalraj" };
            var cities = new List<string> { "Melbourne", "Melbourne", "Brisbane", "Brisbane", "Mumbai", "Mumbai", "Chandigarh", "Chandigarh", "California", "California", "NewYork", "NewYork" };
            int count = 12;
            int timeLineId = 2;//Year
            foreach (var item in countries.Select((Value, Index) => new { Value, Index }))
            {
                AccountDetail account = new AccountDetail();
                account.AccountNumber = MathCalculations.GenerateRandomNo(6).ToString();
                account.Balance = MathCalculations.GenerateRandomNo(6);
                account.OrganizationId = orgID;
                //account.DataSourceId = 1;
                account.Name = names[item.Index];//it's diff  each time
                account.Country = item.Value;//it's same 4 Times
                account.City = cities[item.Index];//it's same 2 times
                account.UserGroup = userGroup[item.Index];//it's same 4 Times
                account.AccountStats = AddDummyAcountStats(names[item.Index]);

                account.Leverage = MathCalculations.GenerateRandomNo(2);
                if (item.Index > 3)
                    timeLineId = 7;//"1 Week (Current Week)
                if (item.Index > 7)
                    timeLineId = 5;// 1 Month(Current Calender Month)
                //account.InstrumentStats = AddInstrumentStats(timeLineId);
                account.InstrumentStats = AddInstrumentStats(names[item.Index]);

                account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
                _unitOfWork.AccountDetailRepository.Add(account);
                count++;
            }

            //account = new AccountDetail();
            //account.AccountNumber = 2222;
            //account.Balance= 2222;
            //account.OrganizationId = new Guid("d5cc177f-5293-6247-bed7-838438c810a3");
            //account.DataSourceId = "2";
            //account.Name = "Shakeel";
            //account.Leverage = "2:50";
            //account.Country = "AUS";
            //account.City= "Ropar";
            //account.UserGroup = "AUS_1";
            //account.AccountStats = AddDummyAcountStats();
            //account.InstrumentStats = AddInstrumentStats();
            //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
            //_unitOfWork.AccountDetailRepository.Add(account);

            //account = new AccountDetail();
            //account.AccountNumber = 3333;
            //account.Balance= 3333;
            //account.OrganizationId = new Guid("d5cc177f-5293-6247-bed7-838438c810a3");
            //account.DataSourceId = "1";
            //account.Name = "Mohit";
            //account.Leverage = "5:50";
            //account.Country = "IN";
            //account.City = "Melbourne";
            //account.UserGroup = "IN_1";
            //account.AccountStats = AddDummyAcountStats();
            //account.InstrumentStats = AddInstrumentStats();
            //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
            //_unitOfWork.AccountDetailRepository.Add(account);

            //account = new AccountDetail();
            //account.AccountNumber = 4444;
            //account.Balance= 4444;
            //account.OrganizationId = new Guid("d5cc177f-5293-6247-bed7-838438c810a3");
            //account.DataSourceId = "4";
            //account.Name = "Aeby";
            //account.Leverage = "4:50";
            //account.Country = "AUS";
            //account.City = "New York";
            //account.UserGroup = "AUS_2";
            //account.AccountStats = AddDummyAcountStats();
            //account.InstrumentStats = AddInstrumentStats();
            //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
            //_unitOfWork.AccountDetailRepository.Add(account);

            // AddDummyTimeLine();
        }

        public bool SaveEmbedWidgetPermission(ObjectId userId, int widgetId, string[] domains)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId && u.RoleID == 2).SingleOrDefault();
            if (user != null)
            {
                var embedWidget = user.EmbedWidgetPermissions.Where(ew => ew.WidgetId == widgetId).SingleOrDefault();
                if (embedWidget == null)
                {
                    UserEmbedWidgetPermission userEmbedWidget = new UserEmbedWidgetPermission();
                    userEmbedWidget.WidgetId = widgetId;
                    userEmbedWidget.Domain = domains;
                    user.EmbedWidgetPermissions.Add(userEmbedWidget);
                }
                else
                {
                    user.EmbedWidgetPermissions.Remove(embedWidget);
                    embedWidget.Domain = domains;
                    user.EmbedWidgetPermissions.Add(embedWidget);

                }
                _unitOfWork.UserRepository.Update(user);
                return true;
            }
            return false;
        }

        public ServiceResponse GetWidgetPermission(ObjectId userId, int widgetId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId && u.RoleID == 2).SingleOrDefault();
            if (user != null)
            {
                var embedWidget = user.EmbedWidgetPermissions.Where(ew => ew.WidgetId == widgetId).SingleOrDefault();
                return new ServiceResponse { Data = embedWidget, Success = true };
            }
            return new ServiceResponse { Success = false, Message = ErrorMessage.User_Not_Exist };
        }


        public bool IsAuthenticatedDomain(ObjectId userId, int widgetId, string domain)
        {
            var embedWidget = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId && u.RoleID == 2).Single().EmbedWidgetPermissions.Where(e => e.WidgetId == widgetId).SingleOrDefault();
            if (embedWidget == null)
                return false;
            return embedWidget.Domain.Contains(domain);
        }
        public List<AccountStats> AddDummyAcountStats(string name)
        {
            var AccountStatsList = new List<AccountStats>();

            for (int i = 1; i <= 45; i++)
            {
                AccountStats accountStats = new AccountStats();

                accountStats.AccountId = i.ToString();
                accountStats.StatringBalance = MathCalculations.GenerateRandomNo(6);
                accountStats.BestPL = MathCalculations.GenerateRandomNo(4).ToString();
                accountStats.CreatedBy = name;
                accountStats.Leverage = i;
                accountStats.CreatedOn = GenerateRandomDate();
                accountStats.DD = MathCalculations.GenerateRandomNo(3);
                accountStats.NAV = MathCalculations.GenerateRandomNo(3);
                accountStats.ROI = MathCalculations.GenerateRandomNo(3);
                accountStats.SharpRatio = MathCalculations.GenerateRandomNo(6);
                accountStats.Status = true;
                //accountStats.DD =  MathCalculations.GenerateRandomNo(2);
                accountStats.TimeLineId = i;
                accountStats.UpdatedBy = MathCalculations.GenerateRandomNo(2);
                accountStats.UpdatedOn = GenerateRandomDate();
                accountStats.WINRate = MathCalculations.GenerateRandomNo(3);
                accountStats.WorstPL = MathCalculations.GenerateRandomNo(2).ToString();

                //_unitOfWork.AccountStatsRepository.Add(accountStats);
                AccountStatsList.Add(accountStats);
            }

            return AccountStatsList;
        }
        public List<AccountTransactionHistory> AddAccountTransactionHistoryStats()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6 }.ToList();

            var AccountStatsList = new List<AccountTransactionHistory>();

            int count = 11;
            foreach (var item in list)
            {
                AccountTransactionHistory accountStats = new AccountTransactionHistory();

                accountStats.AccountId = "1";
                accountStats.Amount = 20000 + count;
                accountStats.ClosePrice = count;
                accountStats.CreatedBy = "Rohit";
                accountStats.Expiry = "1:50";
                accountStats.CreatedOn = "12/8/2016  7:10:05 PM";
                accountStats.Instrument = "Small";
                accountStats.Interest = 11111 + count;
                accountStats.LowerBound = "SS";
                accountStats.OpenPrice = 4444 + count;
                accountStats.Status = true;
                accountStats.ProfitLoss = 777 + count;
                accountStats.Side = "Side";
                accountStats.UpdatedBy = 11 + count;
                accountStats.UpdatedOn = "12/8/2016  7:10:05 PM";
                accountStats.StopLoss = "Near";
                accountStats.TakeProfit = 5555 + count;
                accountStats.TicketId = 11;
                accountStats.TrailingStop = "EUR";
                accountStats.Volume = 5555 + count;
                accountStats.UpperBound = "UpperBound";
                accountStats.UpdatedOn = "12/8/2016  7:10:05 PM";
                accountStats.UpdatedBy = "Mohit";
                accountStats.TransactionType = "Full";

                //_unitOfWork.AccountStatsRepository.Add(accountStats);
                AccountStatsList.Add(accountStats);

                ++count;
            }

            return AccountStatsList;
        }
        public List<InstrumentStats> AddInstrumentStats(string name)
        {
            var instrumentName = new List<string> { "EUR/USD", "GBP/SGD", "SGD/AUD" }.ToList();
            var timeLine = new List<int> { 1, 2, 3 }.ToList();

            var AccountStatsList = new List<InstrumentStats>();

            int count = 11;
            for (int i = 1; i <= 45; i++)
            {
                foreach (var item in instrumentName.Select((Value, Index) => new { Value, Index }))
                {
                    InstrumentStats accountStats = new InstrumentStats();

                    accountStats.AccountStatsId = i.ToString();
                    accountStats.BuyRate = MathCalculations.GenerateRandomNo(3);
                    accountStats.CreatedBy = name;
                    accountStats.CreatedOn = GenerateRandomDate();
                    accountStats.InstrumentId = item.Index + 1;
                    //InstrumentMasterEnum.EURUSD.ToString();
                    accountStats.Volume = MathCalculations.GenerateRandomNo(2);
                    accountStats.WINRate = MathCalculations.GenerateRandomNo(2);
                    accountStats.NAV = MathCalculations.GenerateRandomNo(2);
                    accountStats.ROI = MathCalculations.GenerateRandomNo(2);
                    accountStats.Status = false;

                    accountStats.TimeLineId = i;
                    accountStats.UpdatedBy = name;
                    accountStats.UpdatedOn = GenerateRandomDate();
                    AccountStatsList.Add(accountStats);

                    ++count;
                }
            }

            return AccountStatsList;
        }
        public void AddDummyTimeLine()
        {
            //var list = new int[1, 2, 23, 4, 5, 66, 66, 6, 5];
            //foreach (var item in list)
            //{
            TimeLineMaster timeLineMaster = new TimeLineMaster();

            timeLineMaster.TimeLine = "Overall";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Year (YTD)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "6 Months (Last 6 Calender months)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "1 Quarter (Last 3 Calender Months)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "1 Month (Current Calender Month)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "2 Weeks (Last 2 Weeks)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "1 Week (Current Week)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "7th Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "6th Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "5th Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "4th Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "3th Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "2nd Last Day";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous 1 Day (Previous day)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Today";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous Year (1 Year)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous 3 Months/ Prev Quarter";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous Month (1 Month)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous 2 Weeks";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);
            timeLineMaster = new TimeLineMaster();
            timeLineMaster.TimeLine = "Previous Week (1 Week)";
            _unitOfWork.TimeLineMasterRepository.Add(timeLineMaster);



            //   }
        }
        public string GenerateRandomDate()
        {
            DateTime start = new DateTime(1995, 1, 1);
            Random gen = new Random();
            int range = ((TimeSpan)(DateTime.Today - start)).Days;
            return start.AddDays(gen.Next(range)).ToString();
        }
        public void SetSelectedDatasources(List<string> Ids, ObjectId Id)
        {
            List<ObjectId> datasourceIds = new List<ObjectId>();
            foreach (var id in Ids)
            {
                datasourceIds.Add(ObjectId.Parse(id));
            }
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == Id).Single();
            user.SelectedDatasourceIds = datasourceIds;
            _unitOfWork.UserRepository.Update(user);
        }

        public void UpdateSelectedDataSources(string datasourceId, ObjectId userID)
        {
            var id = ObjectId.Parse(datasourceId);
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userID).Single();
            if (user.SelectedDatasourceIds == null)
            {
                var SelectedDatasourceIds = new List<ObjectId> { id };
                user.SelectedDatasourceIds = SelectedDatasourceIds;
                _unitOfWork.UserRepository.Update(user);
            }
            else if (!user.SelectedDatasourceIds.Any(x => x == id))
            {
                user.SelectedDatasourceIds.Add(id);
                _unitOfWork.UserRepository.Update(user);
            }
        }

        public List<ObjectId> GetSelectedDatasourceIds(ObjectId Id)
        {
            var SelectedDatasourceIds = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == Id).Single().SelectedDatasourceIds;
            if (SelectedDatasourceIds != null)
            {
                return _unitOfWork.DatasourceRepository.GetAll().Where(x => SelectedDatasourceIds.Contains(x.Id) && x.IsConnected == true).Select(x => x.Id).ToList();
            }
            else
            {
                return new List<ObjectId>();
            }
        }
        public AuthenticationResponse GenerateOTP(string email, string password)
        {
            var OTP = MathCalculations.GenerateRandomNo(6).ToString();
            var user = _unitOfWork.UserRepository.GetAll().Where(m => m.EmailID == email.Trim()).FirstOrDefault();
            if (user != null)
            {
                var isLogin = CommonFunction.ComparePassword(password, user.Password);
                if (isLogin)
                {
                    user.OTP = OTP;
                    _unitOfWork.UserRepository.Update(user);
                    var isSent = this.SendOTPEmail(user);
                    if (isSent)
                    {
                        return new AuthenticationResponse { Success = true, Message = CommonMessage.SentOTP };
                    }
                }
            }
            return new AuthenticationResponse { Message = ErrorMessage.Invalid_Credentials, Success = false };
        }

        public string[] GetUserGroups(Guid organizationID)
        {
            return _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == organizationID).Select(x => x.UserGroup).Distinct().ToArray();
        }


    }
}
