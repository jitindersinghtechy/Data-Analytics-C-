using GoogleMaps.LocationServices;
using MongoDB.Bson;
using MongoDB.Driver;
using P23.MetaTrader4.Manager;
using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Infrastructure.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace S2TAnalytics.Infrastructure.Services
{
    public class ConfigureService : IConfigureService
    {
        public readonly IUnitOfWork _unitOfWork;
        public ConfigureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public ServiceResponse Getusers(Guid organizationID)
        {
            var userGroups = GetDistinctUserGroups(organizationID);
            var userRoles = GetUserRolesFromEnum();
            var dataSources = GetDatasourcesLogin(organizationID);//GetDataSourcesFromEnum();

            var response = new ServiceResponse()
            {
                Success = true,
                MultipleData = new Dictionary<string, object>(){
                    { "userGroups", userGroups },
                    { "userRoles", userRoles },
                    { "dataSources", dataSources },
                }
            };
            return response;

        }

        public List<string> GetDistinctUserGroups(Guid organizationID)
        {
            return _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.OrganizationId == organizationID).Select(x => x.UserGroup).Distinct().ToList();
        }

        public List<DatasourceModel> GetDatasourcesLogin(Guid organizationID)
        {
            var configuredDatasources = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.OrganizationId == organizationID).ToList();
            return (new DatasourceModel().ToDatasourceModel(configuredDatasources)).ToList();

        }

        private static List<EnumHelper> GetUserRolesFromEnum()
        {
            //Dictionary<string, string> userRoles = new Dictionary<string, string>();
            var userRoles = new List<EnumHelper>();
            foreach (UserRolesEnum userRole in Enum.GetValues(typeof(UserRolesEnum)))
            {
                if ((int)userRole != 1 && (int)userRole != 2)
                {
                    var role = new EnumHelper
                    {
                        stringValue = ((int)userRole).Encrypt(),
                        DisplayName = ((UserRolesEnum)Enum.ToObject(typeof(UserRolesEnum), (int)userRole)).GetEnumDisplayName()
                    };
                    //timeLines.Add(timeline);
                    userRoles.Add(role);
                }
            }

            return userRoles;
        }

        private static List<EnumHelper> GetDataSourcesFromEnum()
        {
            //Dictionary<string, string> userRoles = new Dictionary<string, string>();
            var dataSources = new List<EnumHelper>();
            foreach (DataSourcesEnum dataSource in Enum.GetValues(typeof(DataSourcesEnum)))
            {
                var source = new EnumHelper
                {
                    stringValue = ((int)dataSource).Encrypt(),
                    DisplayName = ((DataSourcesEnum)Enum.ToObject(typeof(DataSourcesEnum), (int)dataSource)).GetEnumDisplayName()
                };
                //timeLines.Add(timeline);
                dataSources.Add(source);
            }

            return dataSources;
        }

        public ServiceResponse GetDatasources(Guid organizationID)
        {
            try
            {
                var dataSources = GetDataSourcesFromEnum();
                var configuredDatasources = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.OrganizationId == organizationID).ToList();
                var inProcessDatasource = configuredDatasources.Where(d => !d.IsConnected).SingleOrDefault();
                DataSourceQueue queueDatasource = null;
                if (inProcessDatasource != null)
                {
                    queueDatasource = _unitOfWork.DataSourceQueueRepository.GetAll().Where(d => d.DatasourceId == inProcessDatasource.Id).Single();
                }
                var response = new ServiceResponse()
                {
                    Success = true,
                    MultipleData = new Dictionary<string, object>(){
                    { "ConfiguredDatasources", new DatasourceModel().ToDatasourceModel(configuredDatasources) },
                    { "DataSources", dataSources },
                    {"queueDatasource", queueDatasource }
                }
                };
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ServiceResponse SaveDatasource(DatasourceModel datasourceModel, ObjectId userId, Guid OrganizationId)
        {
            try
            {
                if (!IsAlreadyExistConnection(datasourceModel, OrganizationId))
                {
                    ClrWrapper clrWrapper = new ClrWrapper(new ConnectionParameters { Login = datasourceModel.LoginId, Password = datasourceModel.Password, Server = datasourceModel.Server }, ReadConfiguration.MT4Connector);
                    Datasource datasource = new Datasource()
                    {
                        DatasourceId = datasourceModel.DatasourceId.Decrypt(),
                        User = datasourceModel.User,
                        Server = datasourceModel.Server,
                        Password = datasourceModel.Password,
                        LoginId = datasourceModel.LoginId,
                        Name = datasourceModel.Name,
                        AccessToken = datasourceModel.AccessToken,
                        OrganizationId = OrganizationId,
                        IsConfigured = true,
                        IsConnected = false,
                        RateOfReturns = datasourceModel.RateOfReturns
                    };

                    DataSourceQueue dataSourceQueue = new DataSourceQueue()
                    {
                        Server = datasourceModel.Server,
                        Password = datasourceModel.Password,
                        LoginId = datasourceModel.LoginId,
                        OrganizationId = OrganizationId,
                        Status = Convert.ToInt32(DataSourceStatusEnum.Pending)
                    };


                    if (string.IsNullOrEmpty(datasourceModel.Id))
                    {
                        _unitOfWork.DatasourceRepository.Add(datasource);
                        dataSourceQueue.DatasourceId = datasource.Id;
                        _unitOfWork.DataSourceQueueRepository.Add(dataSourceQueue);
                    }
                    else
                    {
                        datasource.Id = ObjectId.Parse(datasourceModel.Id);
                        _unitOfWork.DatasourceRepository.Update(datasource);

                        var existedDataSourceQueue = _unitOfWork.DataSourceQueueRepository.GetAll().Where(x => x.DatasourceId == datasource.Id).SingleOrDefault();
                        dataSourceQueue.Id = existedDataSourceQueue.Id;
                        dataSourceQueue.DatasourceId = datasource.Id;
                        _unitOfWork.DataSourceQueueRepository.Update(dataSourceQueue);

                    }

                    DataSourceQueue datasourceQueue = _unitOfWork.DataSourceQueueRepository.GetAll().Where(d => d.OrganizationId == OrganizationId && d.Server == datasourceModel.Server && d.LoginId == datasourceModel.LoginId && d.Status == (int)DataSourceStatusEnum.Pending).Single();

                    Thread thread = new Thread(() => GetMT4UserRequests(datasourceQueue, clrWrapper));
                    thread.Start();

                    AddNotification(userId, "Data Synchronization started.");
                    var dataSources = GetDatasourcesLogin(datasourceModel.OrganizationId);
                    var dataSourcesEnum = GetDataSourcesFromEnum();
                    return new ServiceResponse
                    {
                        MultipleData = new Dictionary<string, object>()
                        {
                            { "dataSources", dataSources },
                            { "dataSourceQueue", new DataSourceQueueModel().ToDataSourceQueueModel(dataSourceQueue) },
                            { "dataSourcesEnum",dataSourcesEnum }
                        },
                        Success = true,
                        //Message = "MT4 datasource configured successfully."
                        Message = "Your data source has been added successfully."
                    };
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = "Connection already configured." };
                }
            }
            catch (Exception ex)
            {
                AddNotification(userId, "Failed to configure MT4 datasource");
                return new ServiceResponse { Success = false, Message = "Kindly enter correct values for the fields." };
            }
        }



        public ServiceResponse UpdateDataTransformation(DatasourceModel datasourceModel, ObjectId userId, Guid OrganizationId)
        {
            try
            {
                if (!IsAlreadyExistConnection(datasourceModel, OrganizationId))
                {
                    new ClrWrapper(new ConnectionParameters { Login = datasourceModel.LoginId, Password = datasourceModel.Password, Server = datasourceModel.Server }, ReadConfiguration.MT4Connector);
                    Datasource datasource = new Datasource()
                    {
                        DatasourceId = datasourceModel.DatasourceId.Decrypt(),
                        User = datasourceModel.User,
                        Server = datasourceModel.Server,
                        Password = datasourceModel.Password,
                        LoginId = datasourceModel.LoginId,
                        Name = datasourceModel.Name,
                        AccessToken = datasourceModel.AccessToken,
                        OrganizationId = OrganizationId,
                        IsConfigured = true,
                        IsConnected = false,
                        RateOfReturns = datasourceModel.RateOfReturns
                    };

                    DataSourceQueue dataSourceQueue = new DataSourceQueue()
                    {
                        Server = datasourceModel.Server,
                        Password = datasourceModel.Password,
                        LoginId = datasourceModel.LoginId,
                        OrganizationId = OrganizationId,
                        Status = Convert.ToInt32(DataSourceStatusEnum.Transformation)
                    };


                    if (string.IsNullOrEmpty(datasourceModel.Id))
                    {
                        _unitOfWork.DatasourceRepository.Add(datasource);
                        dataSourceQueue.DatasourceId = datasource.Id;
                        _unitOfWork.DataSourceQueueRepository.Add(dataSourceQueue);
                    }
                    else
                    {
                        datasource.Id = ObjectId.Parse(datasourceModel.Id);
                        _unitOfWork.DatasourceRepository.Update(datasource);

                        var existedDataSourceQueue = _unitOfWork.DataSourceQueueRepository.GetAll().Where(x => x.DatasourceId == datasource.Id).SingleOrDefault();
                        dataSourceQueue.Id = existedDataSourceQueue.Id;
                        dataSourceQueue.DatasourceId = datasource.Id;
                        _unitOfWork.DataSourceQueueRepository.Update(dataSourceQueue);

                    }

                    DataSourceQueue datasourceQueue = _unitOfWork.DataSourceQueueRepository.GetAll().Where(d => d.OrganizationId == OrganizationId && d.Server == datasourceModel.Server && d.LoginId == datasourceModel.LoginId && d.Status == (int)DataSourceStatusEnum.Transformation).Single();
                    Thread thread = new Thread(() => TransformData(datasourceQueue));
                    thread.Start();

                    AddNotification(userId, "Transformation started.");
                    var dataSources = GetDatasourcesLogin(datasourceModel.OrganizationId);
                    var dataSourcesEnum = GetDataSourcesFromEnum();
                    return new ServiceResponse
                    {
                        MultipleData = new Dictionary<string, object>()
                        {
                            { "dataSources", dataSources },
                            { "dataSourceQueue", new DataSourceQueueModel().ToDataSourceQueueModel(dataSourceQueue) },
                            { "dataSourcesEnum",dataSourcesEnum }
                        },
                        Success = true,
                        Message = "Transformation completed successfully."
                    };
                }
                else
                {
                    return new ServiceResponse { Success = false, Message = "Connection already configured." };
                }
            }
            catch (Exception ex)
            {
                AddNotification(userId, "Failed to configure MT4 datasource");
                return new ServiceResponse { Success = false, Message = "Failed to configure datasource." };
            }
        }


        private bool IsAlreadyExistConnection(DatasourceModel datasourceModel, Guid OrganizationId)
        {
            var dataSource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Server == datasourceModel.Server && x.LoginId == datasourceModel.LoginId && x.Password == datasourceModel.Password && x.OrganizationId == OrganizationId && x.IsConnected).SingleOrDefault();
            return dataSource != null ? true : false;
        }

        public ServiceResponse DisconnectDatasource(DatasourceModel datasourceModel, ObjectId userId)
        {
            ObjectId id = ObjectId.Parse(datasourceModel.Id);
            Datasource datasource = _unitOfWork.DatasourceRepository.GetAll().Where(d => d.Id == id).Single();
            datasource.IsConnected = false;
            _unitOfWork.DatasourceRepository.Update(datasource);
            AddNotification(userId, "MT4 datasource disconnected successfully");
            return new ServiceResponse { Success = true, Message = "Disconnected successfully." };
        }

        public ServiceResponse SaveUser(UserModel userModel)
        {
            if (!IsEmailExist(userModel.EmailID))
            {
                List<ObjectId> datsourceIds = new List<ObjectId>();
                foreach (var item in userModel.DatasourceIds)
                {
                    datsourceIds.Add(ObjectId.Parse(item));
                }
                var user = new User()
                {
                    OrganizationID = userModel.OrganizationID,
                    EmailID = userModel.EmailID,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    DatasourceIds = datsourceIds,
                    SelectedDatasourceIds = datsourceIds,
                    UserGroups = userModel.UserGroups,
                    RoleID = userModel.RoleID.Decrypt(),
                    IsActive = true
                };
                user.EmailTokenExpiration = DateTime.UtcNow.AddDays(ReadConfiguration.EmailTokenExpirationDays);
                user.EmailToken = CommonFunction.GenerateToken();
                _unitOfWork.UserRepository.Add(user);


                var isSent = this.SendEmail(user);
                userModel = new UserModel().ToUserModel(user);

                return new ServiceResponse { Success = true, Message = "User added successfully. " };
            }
            return new ServiceResponse { Data = userModel, Success = false, Message = ErrorMessage.Email_Already_Exist };
        }

        public bool SendEmail(User model)
        {
            try
            {
                MailHelper mailHelper = new MailHelper();
                mailHelper.ToEmail = model.EmailID;

                mailHelper.Subject = "Complete Registration";
                string AccountLoginUrl = CommonFunction.GetSetPasswordUrl(model.EmailID, model.EmailToken, 1);
                mailHelper.Body = CommonFunction.SetPasswordMailBody(AccountLoginUrl, model.FirstName + " " + model.LastName);

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
        public ServiceResponse GetUsers(PageRecordModel pageRecordModel, out int userCount)
        {
            var filter = pageRecordModel.Filters.Where(x => x.Key == "Name").SingleOrDefault();
            string name = filter.Value == null ? "" : filter.Value;
            // pageRecordModel.PageNumber = 1;
            // pageRecordModel.PageSize = 50;
            List<User> users;
            //if (name == "")
            //{
            userCount = _unitOfWork.UserRepository.GetAll().Where(z => z.FirstName.Contains(name) && z.OrganizationID == pageRecordModel.OrganizationID && (z.RoleID != 1 && z.RoleID != 2)).Count();
            var expression = GetExpression(pageRecordModel.SortBy);
            users = _unitOfWork.UserRepository.GetPagedRecordsLinq(pageRecordModel, z => z.FirstName.Contains(name) && z.OrganizationID == pageRecordModel.OrganizationID && (z.RoleID != 1 && z.RoleID != 2), expression, pageRecordModel.SortOrder);
            //}
            //else
            //{
            //    users = _unitOfWork.UserRepository.GetPagedRecordsLinq(pageRecordModel, z => z.OrganizationID == pageRecordModel.OrganizationID && (z.RoleID != 1 && z.RoleID != 2) && (z.Name).Contains(name), z => z.FirstName);
            //}
            var usersModel = users.Select(n => new UserModel()
            {
                FirstName = n.FirstName,
                LastName = n.LastName,
                EmailID = n.EmailID,
                RoleName = GetRoleName(),
                Datasources = GetDatasourceNames(n.DatasourceIds),
                UserGroups = n.UserGroups,
                IsActive = n.IsActive,
                RoleID = n.RoleID.Encrypt()
            });

            return new ServiceResponse { Data = usersModel, Success = true };
        }

        private Expression<Func<User, string>> GetExpression(string sortBy)
        {
            switch (sortBy)
            {
                case "Email":
                    return x => x.EmailID;
                default:
                    return x => x.FirstName;
            }
        }

        public ServiceResponse DeleteUsers(List<string> emailIds, Guid organizatinId)
        {
            _unitOfWork.UserRepository.Delete(x => emailIds.Contains(x.EmailID) && x.OrganizationID == organizatinId);

            return new ServiceResponse { Success = true, Message = "User(s) deleted successfully." };
        }

        public void AddNotification(ObjectId userId, string notification)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single();
            user.Notifications.Add(new Notification()
            {
                id = Guid.NewGuid(),
                Text = notification,
                Date = DateTime.Now,
                IsRead = false
            });
            _unitOfWork.UserRepository.Update(user);
        }

        public ServiceResponse GetNotifications(ObjectId userId)
        {
            try
            {
                return new ServiceResponse
                {
                    MultipleData = new Dictionary<string, object>()
                {
                    { "notifications",_unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SelectMany(u => u.Notifications).OrderByDescending(o => o.Date).Take(5).ToList() },
                    { "notificationCount",_unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).ToList().SelectMany(u => u.Notifications.ToList().Where(x=>x.IsRead==false)).Count() }
                },
                    Success = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ServiceResponse GetAllNotifications(ObjectId userId)
        {
            return new ServiceResponse { Data = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SelectMany(u => u.Notifications).OrderByDescending(o => o.Date).ToList(), Success = true };
        }

        public ServiceResponse ReadNotifications(ObjectId userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
                user.Notifications.ForEach(x => x.IsRead = true);
                _unitOfWork.UserRepository.Update(user);
                return new ServiceResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // remove notifications....
        public ServiceResponse removeSingleNotification(ObjectId userId, Guid id)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            user.Notifications.RemoveAll(x => x.id == id);
            _unitOfWork.UserRepository.Update(user);

            return new ServiceResponse { Success = true, Message = "User(s) deleted successfully." };
        }
        public ServiceResponse removeAllNotification(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            user.Notifications.Clear();
            _unitOfWork.UserRepository.Update(user);
            return new ServiceResponse { Success = true, Message = "User(s) deleted successfully." };
        }



        public ServiceResponse UpdateUserAccess(bool IsBlock, List<string> emailIds)
        {
            var users = _unitOfWork.UserRepository.GetAll().Where(x => emailIds.Contains(x.EmailID)).ToList();
            users.ForEach(x => x.IsActive = !IsBlock);
            foreach (var user in users)
            {
                _unitOfWork.UserRepository.Update(user);
            }

            return new ServiceResponse { Success = true, Message = "User(s) " + (IsBlock ? "blocked" : "Unblocked") + " successfully." };
        }

        private string GetRoleName()
        {
            return ((UserRolesEnum)Enum.ToObject(typeof(UserRolesEnum), (int)UserRolesEnum.ReportUser)).GetEnumDisplayName();
        }

        private List<string> GetDatasourceNames(List<ObjectId> datasourceIds)
        {
            List<string> datasources = new List<string>();

            return _unitOfWork.DatasourceRepository.GetAll().Where(x => datasourceIds.Contains(x.Id)).ToList().Select(x => x.Name).ToList();
            //foreach (DataSourcesEnum dataSource in Enum.GetValues(typeof(DataSourcesEnum)))
            //{
            //    if (datasourceIds.Contains((int)dataSource))
            //    {
            //        datasources.Add(((DataSourcesEnum)Enum.ToObject(typeof(DataSourcesEnum), (int)dataSource)).GetEnumDisplayName());
            //    }
            //}
            //return datasources;
        }
        private bool IsEmailExist(string email)
        {
            return _unitOfWork.UserRepository.GetAll().Any(m => m.EmailID == email);
        }

        public ServiceResponse GetDataSourcesForUser(ObjectId userId, int RoleID, Guid organizationId)
        {
            List<DatasourceModel> datasources = new List<DatasourceModel>();
            if (RoleID == 2)
            {
                datasources = _unitOfWork.DatasourceRepository.GetAll().Where(u => u.OrganizationId == organizationId).ToList().Select(u => new DatasourceModel { Id = u.Id.ToString(), Name = u.Name }).ToList();
            }
            else
            {
                var DataSourceIds = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SelectMany(x => x.DatasourceIds).ToList();
                datasources = _unitOfWork.DatasourceRepository.GetAll().Where(u => DataSourceIds.Contains(u.Id)).ToList().Select(u => new DatasourceModel { Id = u.Id.ToString(), Name = u.Name }).ToList();
            }

            var selectedDatasourceIds = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).Single().SelectedDatasourceIds;
            //((UserRolesEnum)Enum.ToObject(typeof(UserRolesEnum), (int)userRole)).GetEnumDisplayName();
            return new ServiceResponse
            {
                Data = datasources,
                MultipleData = new Dictionary<string, object>(){
                        { "selectedDatsources", selectedDatasourceIds }
                    },
                Success = true
            };
        }

        // remove notifications....
        public ServiceResponse removeSingleDataSource(ObjectId userId, string id)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            user.DatasourceIds.RemoveAll(x => x == ObjectId.Parse(id));
            _unitOfWork.UserRepository.Update(user);

            return new ServiceResponse { Success = true, Message = "DataSource(s) deleted successfully." };
        }
        public ServiceResponse removeAllDataSource(ObjectId userId)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.Id == userId).SingleOrDefault();
            user.DatasourceIds.Clear();
            _unitOfWork.UserRepository.Update(user);
            return new ServiceResponse { Success = true, Message = "DataSource(s) deleted successfully." };
        }

        public ServiceResponse ChangeUserGroup(List<string> emailIds, List<string> userGroups)
        {
            var users = _unitOfWork.UserRepository.GetAll().Where(x => emailIds.Contains(x.EmailID)).ToList();
            //users.ForEach(x => (x.UserGroups.Contains(userGroup))?x.UserGroups.ToList(): x.UserGroups=x.UserGroups.Add(userGroup));
            foreach (var user in users)
            {
                foreach (var userGroup in userGroups)
                {
                    if (!user.UserGroups.Contains(userGroup))
                        user.UserGroups.Add(userGroup);
                }
                //userGroups.ForEach(x => (user.UserGroups.Contains(x)) ? user.UserGroups : AddUserGroup(user.UserGroups,x));
                //{
                //    user.UserGroups.Add(x);
                //}
                _unitOfWork.UserRepository.Update(user);
            }

            return new ServiceResponse { Success = true, Message = "User(s) updated successfully." };
        }

        //public List<string> AddUserGroup(List<string> userGroups,string userGroup)
        //{
        //    userGroups.Add(userGroup);
        //    return userGroups;
        //}
        public ServiceResponse ChangeDataSource(List<string> emailIds, List<string> dataSourceIds)
        {
            var users = _unitOfWork.UserRepository.GetAll().Where(x => emailIds.Contains(x.EmailID)).ToList();
            //users.ForEach(x =>  x.DatasourceIds.Add(ObjectId.Parse(dataSourceId)));
            foreach (var user in users)
            {
                foreach (var dataSourceId in dataSourceIds)
                {
                    if (!user.DatasourceIds.Contains(ObjectId.Parse(dataSourceId)))
                        user.DatasourceIds.Add(ObjectId.Parse(dataSourceId));
                }
                //if (!user.DatasourceIds.Contains(ObjectId.Parse(dataSourceId)))
                //{
                //    user.DatasourceIds.Add(ObjectId.Parse(dataSourceId));
                //}
                _unitOfWork.UserRepository.Update(user);
            }

            return new ServiceResponse { Success = true, Message = "User(s) updated successfully." };
        }

        public ServiceResponse CheckForSynronizatedDataSource(string Id, ObjectId userId)
        {
            var dataSourceQueue = _unitOfWork.DataSourceQueueRepository.GetAll().Where(x => x.Id == (ObjectId.Parse(Id))).SingleOrDefault();
            //if (dataSourceQueue.Status == Convert.ToInt32(DataSourceStatusEnum.Success))
            //{
            //    var dataSource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == dataSourceQueue.DatasourceId).SingleOrDefault();
            //    dataSource.IsConnected = true;
            //    _unitOfWork.DatasourceRepository.Update(dataSource);
            //    AddNotification(userId, "MT4 datasource configured successfully");
            //}
            return new ServiceResponse { Data = dataSourceQueue, Success = true, Message = "Syncronizing." };

        }

        #region MT4 ELT Methods

        #region MT4

        private void TransformData(DataSourceQueue dataSource)
        {

            try
            {
                dataSource.Status = (int)DataSourceStatusEnum.InProcess;
                _unitOfWork.DataSourceQueueRepository.Update(dataSource);

                MT4UserRequestsToAccountDetails(dataSource, false);

                dataSource.Status = (int)DataSourceStatusEnum.Success;
                _unitOfWork.DataSourceQueueRepository.Update(dataSource);
                var userDataSource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == dataSource.DatasourceId).Single();
                userDataSource.IsConnected = true;
                _unitOfWork.DatasourceRepository.Update(userDataSource);
                AddNotification(dataSource.OrganizationId, "Transformation completed successfully");
            }
            catch (Exception ex)
            {
                dataSource.Status = (int)DataSourceStatusEnum.Failed;

                _unitOfWork.DataSourceQueueRepository.Update(dataSource);
                AddNotification(dataSource.OrganizationId, "Failed to configure MT4 datasource");
            }

        }
        private void GetMT4UserRequests(DataSourceQueue dataSource, ClrWrapper clrWrapper)
        {
            try
            {
                dataSource.Status = (int)DataSourceStatusEnum.InProcess;
                _unitOfWork.DataSourceQueueRepository.Update(dataSource);
                MT4ELTHelper.CreateWrapper(clrWrapper);

                var MT4Users = MT4ELTHelper.GetAccountSummaryByAccount();
                var insertMT4Userrequests = new List<MT4UserRequest>();
                foreach (var user in MT4Users)
                {
                    var userRequest = new MT4UserRequest
                    {
                        Login = user.Login,
                        OrganizationId = dataSource.OrganizationId,
                        Address = user.Address,
                        AgentAccount = user.AgentAccount,
                        ApiData = user.ApiData,
                        Balance = user.Balance,
                        City = string.IsNullOrEmpty(user.City) ? "N/A" : Char.ToUpperInvariant(user.City[0]) + user.City.Substring(1).ToLower(),
                        Comment = user.Comment,
                        Country = (string.IsNullOrEmpty(user.Country) || user.Country == "null") ? "N/A" : Char.ToUpperInvariant(user.Country[0]) + user.Country.Substring(1).ToLower(),
                        Credit = user.Credit,
                        Email = user.Email,
                        Enable = user.Enable,
                        EnableChangePassword = user.EnableChangePassword,
                        EnableOTP = user.EnableOTP,
                        EnableReadOnly = user.EnableReadOnly,
                        Group = user.Group,
                        UserId = user.Id,
                        InterestRate = user.InterestRate,
                        LastDate = user.LastDate,
                        LastIp = user.LastIp,
                        LeadSource = user.LeadSource,
                        Leverage = user.Leverage,
                        Mqid = user.Mqid,
                        Name = user.Name == "" ? user.Login.ToString() : user.Name,
                        OTPSecret = user.OTPSecret,
                        Password = user.Password,
                        PasswordInvestor = user.PasswordInvestor,
                        PasswordPhone = user.PasswordPhone,
                        Phone = user.Phone,
                        PrevBalance = user.PrevBalance,
                        PrevEquity = user.PrevEquity,
                        PrevMonthBalance = user.PrevMonthBalance,
                        PrevMonthEquity = user.PrevMonthEquity,
                        PublicKey = user.PublicKey,
                        Regdate = user.Regdate,
                        SendReports = user.SendReports,
                        State = user.State,
                        Status = user.Status,
                        Taxes = user.Taxes,
                        Timestamp = user.Timestamp,
                        UserColor = Convert.ToInt64(user.UserColor),
                        ZipCode = user.ZipCode,

                    };
                    var tradesUserHistory = MT4ELTHelper.GetUserHistoryByAccount(user.Login, new DateTime(1970, 1, 1), DateTime.UtcNow);
                    foreach (var tradesHistory in tradesUserHistory)
                    {
                        userRequest.TradesHistories.Add(tradesHistory);
                    }
                    insertMT4Userrequests.Add(userRequest);
                }
                if (insertMT4Userrequests.Any())
                {
                    _unitOfWork.MT4UserRequestRepository.AddMultiple(insertMT4Userrequests);
                }
                MT4UserRequestsToAccountDetails(dataSource, true);
                dataSource.Status = (int)DataSourceStatusEnum.Success;
                _unitOfWork.DataSourceQueueRepository.Update(dataSource);
                var userDataSource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == dataSource.DatasourceId).Single();
                userDataSource.IsConnected = true;
                _unitOfWork.DatasourceRepository.Update(userDataSource);
                AddNotification(dataSource.OrganizationId, "MT4 datasource configured successfully");
            }
            catch (Exception ex)
            {
                dataSource.Status = (int)DataSourceStatusEnum.Failed;

                string filePath = @"C:\Rohit_Mohit\Error.txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }

                _unitOfWork.DataSourceQueueRepository.Update(dataSource);
                AddNotification(dataSource.OrganizationId, "Failed to configure MT4 datasource");
            }
        }

        private void MT4UserRequestsToAccountDetails(DataSourceQueue dataSource, bool calculateDaillyEquity)
        {
            var userRequests = _unitOfWork.MT4UserRequestRepository.GetAll().Where(x => x.OrganizationId == dataSource.OrganizationId).ToList();
            if (calculateDaillyEquity)
            {
                CalculateDailyNav(userRequests);
            }

            List<AccountDetail> insertAccountDetail = new List<AccountDetail>();

            foreach (var userRequest in userRequests)
            {

                string accountNumber = userRequest.Login.ToString();
                var existingAccountDetail = _unitOfWork.AccountDetailRepository.GetAll().Where(x => x.AccountNumber == accountNumber && x.OrganizationId == dataSource.OrganizationId).FirstOrDefault();
                if (existingAccountDetail == null)
                {

                    AccountDetail account = new AccountDetail();
                    account.OrganizationId = dataSource.OrganizationId;
                    //account.DataSourceId = (int)DataSourcesEnum.MT4;
                    account.DataSourceId = dataSource.DatasourceId;
                    account.AccountNumber = userRequest.Login.ToString();
                    account.Name = userRequest.Name;
                    account.Balance = Math.Round(userRequest.Balance, 2);
                    account.Leverage = userRequest.Leverage;
                    account.Country = userRequest.Country;
                    account.CountryCode = CountryCodeByCountryName(userRequest.Country);
                    account.City = userRequest.City;
                    var latLong = GetCityLongLat(userRequest.City, userRequest.Country);
                    account.CityLat = latLong.Where(f => f.Key == "Lat").Single().Value;
                    account.CityLong = latLong.Where(f => f.Key == "Long").Single().Value;
                    //account.State = userRequest.State;
                    //account.StateCode = GetStateNameByLatLong(account.CityLat, account.CityLong);
                    account.Address = userRequest.Address;
                    account.Email = userRequest.Email;
                    account.Phone = userRequest.Phone;
                    account.Comment = userRequest.Comment;
                    account.UserGroup = userRequest.Group;
                    account.ActiveSince = userRequest.TradesHistories.Count > 0 ? SecondsToDate(userRequest.TradesHistories[0].OpenTime).ToString() : "";
                    account.LastActiveOn = SecondsToDate(userRequest.LastDate).ToString();
                    //account.CreatedBy = 
                    account.CreatedOn = DateTime.UtcNow.ToString();
                    account.Status = userRequest.Status;

                    account.AccountStats = AddAcountStats(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage, dataSource);
                    account.InstrumentStats = AddInstrumentStats(userRequest.Name, userRequest.TradesHistories);
                    //account.AccountTransactionHistories = AddAccountTransactionHistoryStats();
                    //unitOfWork.AccountDetailRepository.Add(account);
                    insertAccountDetail.Add(account);
                }
                else
                {
                    existingAccountDetail.AccountStats = AddAcountStats(userRequest.TradesHistories, userRequest.Login, userRequest.Leverage, dataSource);
                    //existingAccountDetail.InstrumentStats = AddInstrumentStats(userRequest.Name, userRequest.TradesHistories);
                    _unitOfWork.AccountDetailRepository.Update(existingAccountDetail);
                }
            }
            if (insertAccountDetail.Any())
            {
                _unitOfWork.AccountDetailRepository.AddMultiple(insertAccountDetail);
            }
        }

        public string CountryCodeByCountryName(string countryName)
        {

            try
            {
                if (countryName == "United arab emirates")
                {
                    countryName = "U.A.E.";
                    var a = "";
                }
                var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
                var englishRegion = regions.FirstOrDefault(region => region.EnglishName.Contains(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(countryName)));
                if (englishRegion == null)
                {
                    return null;
                }
                else
                    return englishRegion.TwoLetterISORegionName;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public Dictionary<string, double> GetCityLongLat(string cityName, string countryName)
        {
            try
            {
                //if (countryName == "Australia")
                //{
                //    cityName = "New South Wales";
                //}
                if (cityName == "N/A")
                {
                    return new Dictionary<string, double>() {
                        { "Lat", 0 },
                        { "Long", 0 },
                    };
                }

                var address = cityName + ", " + countryName;
                var locationService = new GoogleLocationService();
                var point = locationService.GetLatLongFromAddress(address);

                if (cityName == "N/A" || point == null)
                {
                    return new Dictionary<string, double>() {
                        { "Lat", 0 },
                        { "Long", 0 },
                    };
                }

                var latitude = point.Latitude;
                var longitude = point.Longitude;
                return new Dictionary<string, double>() {
                         { "Lat", latitude },
                         { "Long", longitude },
                    };
            }
            catch (Exception ex)
            {
                string filePath = @"C:\Rohit_Mohit\Error.txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }

                return new Dictionary<string, double>() {
                    { "Lat", 0 },
                    { "Long", 0 },
                };
            }
        }

        public string GetStateNameByLatLong(double latitude, double longitude)
        {
            var Address_administrative_area_level_1 = "";
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + latitude + "," + longitude + "&sensor=false");
                XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
                if (element.InnerText == "ZERO_RESULTS")
                {
                    return ("No data available for the specified location");
                }
                else
                {
                    element = doc.SelectSingleNode("//GeocodeResponse/result/formatted_address");

                    string longname = "";
                    string shortname = "";
                    string typename = "";
                    bool fHit = false;

                    XmlNodeList xnList = doc.SelectNodes("//GeocodeResponse/result/address_component");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            longname = xn["long_name"].InnerText;
                            shortname = xn["short_name"].InnerText;
                            typename = xn["type"].InnerText;

                            fHit = true;
                            switch (typename)
                            {
                                case "administrative_area_level_1":
                                    {
                                        Address_administrative_area_level_1 = longname;
                                        break;
                                    }
                                default:
                                    fHit = false;
                                    break;
                            }

                            if (fHit)
                            {
                                Console.Write(typename);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("\tL: " + longname + "\tS:" + shortname + "\r\n");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        }

                        catch (Exception e)
                        {
                            //Node missing either, longname, shortname or typename
                            fHit = false;
                            Console.Write(" Invalid data: ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("\tX: " + xn.InnerXml + "\r\n");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                    }
                    //Console.ReadKey();
                    return (element.InnerText);
                }
            }
            catch (Exception ex)
            {
                return ("(Address lookup failed: ) " + ex.Message);
            }
        }
        public List<InstrumentStats> AddInstrumentStats(string name, List<TradeRecord> trades)
        {
            var instrumentName = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).Select(x => x.Symbol).Distinct().ToList();
            var AccountStatsList = new List<InstrumentStats>();
            foreach (TimeLineEnum timeline in Enum.GetValues(typeof(TimeLineEnum)))
            {
                foreach (var item in instrumentName.Select((Value, Index) => new { Value, Index }))
                {
                    InstrumentStats accountStats = new InstrumentStats();
                    accountStats.TimeLineId = (int)timeline;
                    accountStats.AccountStatsId = timeline;
                    accountStats.CreatedBy = name;
                    accountStats.InstrumentId = item.Index + 1;
                    accountStats.InstrumentName = item.Value;
                    accountStats.Volume = GetVolumeOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    accountStats.Profit = GetProfitOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    accountStats.Loss = GetLossOfInstrumentByTimelineId((int)timeline, trades, item.Value);
                    accountStats.Status = true;
                    if (accountStats.Volume > 0)
                    {
                        AccountStatsList.Add(accountStats);
                    }
                }
            }

            return AccountStatsList;
        }

        private double GetVolumeOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var volume = trades.Where(x => x.OpenTime >= startSeconds && x.OpenTime < endSeconds && (x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell)
                                            && x.Symbol == instrumentName).Sum(x => x.Volume);
                return volume;
            }
            else
            {
                return 0;
            }
        }

        private double GetProfitOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var profit = trades.Where(x => x.OpenTime >= startSeconds && x.OpenTime < endSeconds && (x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell)
                                            && x.Symbol == instrumentName && x.Profit > 0).Sum(x => x.Profit);
                return profit;
            }
            else
            {
                return 0;
            }
        }

        private double GetLossOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var loss = trades.Where(x => x.OpenTime >= startSeconds && x.OpenTime < endSeconds && (x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell)
                                        && x.Symbol == instrumentName && x.Profit < 0).Sum(x => x.Profit);
                return loss;
            }
            else
            {
                return 0;
            }
        }


        private void CalculateDailyNav(List<MT4UserRequest> userRequests)
        {
            //try
            //{
            List<DailyEquity> insertDailyEquityOfUser = new List<DailyEquity>();
            foreach (var userRequest in userRequests)
            {
                var existingtDailyEquities = _unitOfWork.DailyEquityRepository.GetAll().Where(x => x.AccountNumber == userRequest.Login && x.OrganizationId == userRequest.OrganizationId).FirstOrDefault();
                if (existingtDailyEquities == null)
                {
                    if (userRequest.TradesHistories.Count() > 0)
                    {
                        var equityByDates = new List<EquityByDate>();
                        var startDate = SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date;

                        var beginingDate = startDate;
                        var endDate = startDate.AddDays(1);
                        //var balance = userRequest.TradesHistories[0].Profit;
                        var balance = new double();
                        var balanaceWithoutWithdrawn = new double();
                        var id = 1;
                        var previousDateNav = new double();
                        do
                        {
                            if (userRequest.Login == 441327)
                            {
                                var b = "";
                            }
                            var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            var buyTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy)
                                                            .Sum(x => x.Profit);
                            var sellTrades = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell)
                                                            .Sum(x => x.Profit);
                            var balanaceTypeWithWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                            && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit)).Sum(x => x.Profit);
                            var balanaceTypesWithoutWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds)
                                                            && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit > 0).Sum(x => x.Profit);


                            //var balanaceWithdrawn = new double();

                            //if (startDate != beginingDate)
                            //{
                            //balanaceWithdrawn = userRequest.TradesHistories.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Balance).Sum(x => x.Profit);
                            //}
                            balance = balance + balanaceTypeWithWithdrawn + buyTrades + sellTrades;
                            balanaceWithoutWithdrawn = balanaceWithoutWithdrawn + balanaceTypesWithoutWithdrawn + buyTrades + sellTrades;

                            if (balance == 0 && startDate == SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                            {
                            }
                            else
                            {
                                var sharpeRatioReturn = new double();
                                if (startDate != SecondsToDate(userRequest.TradesHistories[0].OpenTime).Date)
                                {
                                    sharpeRatioReturn = previousDateNav == 0 ? 0 : (balance - previousDateNav) / previousDateNav;
                                    previousDateNav = balance;
                                }
                                else
                                {
                                    sharpeRatioReturn = 0;
                                    previousDateNav = balance;
                                }

                                if (balance < 0 || balanaceWithoutWithdrawn < 0)
                                {

                                }

                                var equityByDate = new EquityByDate();
                                equityByDate.date = startDate.ToString();
                                equityByDate.Equity = Math.Round(balance, 2);
                                equityByDate.EquityWithoutWithdrawn = Math.Round(balanaceWithoutWithdrawn, 2);
                                equityByDate.id = id;
                                //equityByDate.SharpeRatioReturn = Math.Round(sharpeRatioReturn, 2);
                                equityByDate.SharpeRatioReturn = sharpeRatioReturn;
                                equityByDates.Add(equityByDate);
                            }

                            startDate = startDate.AddDays(1);
                            endDate = endDate.AddDays(1);
                            id++;
                        } while (startDate < DateTime.Today);

                        DailyEquity dailyEquityOfUser = new DailyEquity();
                        dailyEquityOfUser.AccountNumber = userRequest.Login;
                        dailyEquityOfUser.EquityByDate = equityByDates;
                        dailyEquityOfUser.OrganizationId = userRequest.OrganizationId;
                        //unitOfWork.DailyEquityRepository.Add(dailyEquityOfUser);
                        insertDailyEquityOfUser.Add(dailyEquityOfUser);
                    }
                }
            }
            if (insertDailyEquityOfUser.Any())
            {
                _unitOfWork.DailyEquityRepository.AddMultiple(insertDailyEquityOfUser);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }

        public List<AccountStats> AddAcountStats(List<TradeRecord> trades, int accountNumber, int leverage, DataSourceQueue dataSourceQueue)
        {
            //try
            //{
            if (accountNumber == 447927)
            {

            }
            var dailyNavs = _unitOfWork.DailyEquityRepository.GetAll().Where(x => x.AccountNumber == accountNumber && x.OrganizationId == dataSourceQueue.OrganizationId).FirstOrDefault();
            var AccountStatsList = new List<AccountStats>();
            var datasource = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.Id == dataSourceQueue.DatasourceId).Single();
            foreach (TimeLineEnum timeline in Enum.GetValues(typeof(TimeLineEnum)))
            {
                if ((int)timeline == 45)
                {

                }
                AccountStats accountStats = new AccountStats();
                accountStats.AccountId = timeline;
                accountStats.StatringBalance = GetStartingBalanceByTimeline((int)timeline, dailyNavs, trades);
                accountStats.Deposit = GetDepositsByTimeline((int)timeline, dailyNavs, trades);
                accountStats.Withdrawn = GetWithdrawnByTimeline((int)timeline, dailyNavs, trades);
                accountStats.ProfitLoss = GetProfitLossByTimeline((int)timeline, dailyNavs, trades);
                accountStats.BestPL = CalculateBestPLForTimeline((int)timeline, trades);
                accountStats.CreatedBy = accountNumber;
                accountStats.Leverage = leverage;
                accountStats.CreatedOn = DateTime.UtcNow.ToString();
                accountStats.DD = dailyNavs == null ? 0 : CalculateDDForTimeline((int)timeline, dailyNavs, trades);
                accountStats.NAV = dailyNavs == null ? 0 : CalculateNavForTimeline((int)timeline, dailyNavs); //Math.Round(dailyNavs.NAVByDate[dailyNavs.NAVByDate.Count - 1].NAV, 2);
                accountStats.ROI = dailyNavs == null ? 0 : CalculateROIForTimeline((int)timeline, dailyNavs, trades);
                accountStats.SharpRatio = dailyNavs == null ? 0 : CalculateSharpRatioForTimeline((int)timeline, dailyNavs, dataSourceQueue.DatasourceId, datasource, trades);
                accountStats.AvgTrade = dailyNavs == null ? 0 : CalculateAvgTradesForTimeline((int)timeline, trades);
                accountStats.Status = true;
                accountStats.TimeLineId = (int)timeline;
                //accountStats.UpdatedBy = MathCalculations.GenerateRandomNo(2);
                //accountStats.UpdatedOn = GenerateRandomDate();
                accountStats.WINRate = CalculateWINForTimeline((int)timeline, trades);
                accountStats.WorstPL = CalculateWorstPLForTimeline((int)timeline, trades);
                //_unitOfWork.AccountStatsRepository.Add(accountStats);
                AccountStatsList.Add(accountStats);
            }

            return AccountStatsList;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }

        private double GetStartingBalanceByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (dailyEquity != null && dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);


                var startingBalance = dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.Equity).FirstOrDefault();
                return startingBalance;
            }
            else
            {
                return 0;
            }
        }

        private double GetDepositsByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var deposits = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit > 0).Sum(x => x.Profit);
                return deposits;
            }
            else
            {
                return 0;
            }
        }

        private double GetWithdrawnByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var withdrawn = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit) && x.Profit < 0).Sum(x => x.Profit);
                return withdrawn;
            }
            else
            {
                return 0;
            }
        }
        private double GetProfitLossByTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                var profitLoss = buyTrades + sellTrades;
                return profitLoss;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateAvgTradesForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);
                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Count();
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Count();
                var numberOfDays = (endDate - startDate).TotalDays;
                var avgTrades = Math.Round(((buyTrades + sellTrades) / numberOfDays), 2);
                return avgTrades;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateSharpRatioForTimeline(int timelineId, DailyEquity dailyEquity, ObjectId datasourceId, Datasource datasource, List<TradeRecord> trades)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate).AddDays(-1);

                var firstTradeTime = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).OrderBy(x => x.OpenTime).Select(x => x.OpenTime).FirstOrDefault();
                var firstTradeDate = new DateTime(1970, 1, 1, 0, 0, 1, 0).AddSeconds(firstTradeTime).Date;
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                {
                    if (startDate != firstTradeDate)
                    {
                        startDate = firstTradeDate;
                    }
                }
                else
                {
                    if (startDate < firstTradeDate && firstTradeDate < endDate)
                    {
                        startDate = firstTradeDate;
                    }
                }

                var riskFreeRate = Timelines.MasterTimelines.Any(x => x == timelineId) ? datasource.RateOfReturns.Where(x => x.TimelineId == timelineId).FirstOrDefault().RateOfReturn
                                        : Convert.ToDouble(ConfigurationManager.AppSettings["RiskFreeRateForSharpeRatio"]);
                var NavsForStatndardDeviaition = dailyEquity.EquityByDate.Where(x => Convert.ToDateTime(x.date) >= startDate && Convert.ToDateTime(x.date) <= endDate)
                                                .Select(x => x.SharpeRatioReturn).ToList();
                var startingNav = dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ?
                                  dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ? 0 : dailyEquity.EquityByDate[0].EquityWithoutWithdrawn :
                                  dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();

                var endingNav = dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault() == 0 ? 0 :
                                                          dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                if (startingNav == 0 || startingNav == endingNav || NavsForStatndardDeviaition.Count == 0)
                    return 0;
                else
                {
                    var portfolioReturn = (endingNav - startingNav) / startingNav;
                    //var standardDev = calculateStandardDeviation(new List<double> { startingNav, endingNav });
                    var standardDev = calculateStandardDeviation(NavsForStatndardDeviaition);

                    var sharpeRatio = Math.Round((portfolioReturn - (Convert.ToDouble(riskFreeRate) / 100)) / standardDev, 2);
                    return sharpeRatio;
                }
            }
            else
            {
                return 0;
            }
        }

        private double calculateStandardDeviation(List<double> navsList)
        {
            double average = navsList.Average();
            double sumOfSquaresOfDifferences = navsList.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / navsList.Count);
            return sd;
        }

        private double CalculateNavForTimeline(int timelineId, DailyEquity dailyEquity)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();

                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var endingNav = dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.Equity).FirstOrDefault() == 0 ? 0 :
                                                          dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).Select(x => x.Equity).FirstOrDefault();
                return endingNav;
            }
            else
            {
                return 0;
            }
        }

        private string CalculateBestPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var bestBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderByDescending(x => x.Profit).FirstOrDefault();
                var bestSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderByDescending(x => x.Profit).FirstOrDefault();

                if (bestBuyTrades == null && bestSellTrades == null)
                    return "0";
                else if (bestBuyTrades == null && bestSellTrades != null)
                    return Math.Round(bestSellTrades.Profit, 2).ToString();
                else if (bestBuyTrades != null && bestSellTrades == null)
                    return Math.Round(bestBuyTrades.Profit, 2).ToString();
                else
                    return bestBuyTrades.Profit > bestSellTrades.Profit ? Math.Round(bestBuyTrades.Profit, 2).ToString() : Math.Round(bestSellTrades.Profit, 2).ToString();
            }
            else
            {
                return "0";
            }
        }

        private string CalculateWorstPLForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var worstBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).OrderBy(x => x.Profit).FirstOrDefault();
                var worstSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).OrderBy(x => x.Profit).FirstOrDefault();

                if (worstBuyTrades == null && worstSellTrades == null)
                    return "0";
                else if (worstBuyTrades == null && worstSellTrades != null)
                    return Math.Round(worstSellTrades.Profit, 2).ToString();
                else if (worstBuyTrades != null && worstSellTrades == null)
                    return Math.Round(worstBuyTrades.Profit, 2).ToString();
                else
                    return worstBuyTrades.Profit > worstSellTrades.Profit ? Math.Round(worstSellTrades.Profit, 2).ToString() : Math.Round(worstBuyTrades.Profit, 2).ToString();
            }
            else
            {
                return "0";
            }
        }

        private double CalculateWINForTimeline(int timelineId, List<TradeRecord> trades)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Count();
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Count();

                var positiveBuyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy && x.Profit > 0).Count();
                var positiveSellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell && x.Profit > 0).Count();
                var winRate = new double();
                if (buyTrades == 0 && sellTrades == 0)
                    winRate = 0;
                else
                    winRate = Math.Round((Convert.ToDouble(positiveSellTrades + positiveBuyTrades) / Convert.ToDouble(buyTrades + sellTrades)) * 100, 2);

                return winRate;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateDDForTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (dailyEquity.EquityByDate.Count > 0)
            {
                if (dailyEquity.AccountNumber == 442842)
                {
                    var a = "";
                }
                var startDate = new DateTime();
                var endDate = new DateTime();

                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var firstTradeTime = trades.Where(x => x.Cmd == TradeCommand.Buy || x.Cmd == TradeCommand.Sell).OrderBy(x => x.OpenTime).Select(x => x.OpenTime).FirstOrDefault();
                var firstTradeDate = new DateTime(1970, 1, 1, 0, 0, 1, 0).AddSeconds(firstTradeTime).Date;
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                {
                    if (startDate != firstTradeDate)
                    {
                        startDate = firstTradeDate;
                    }
                }
                else
                {
                    if (startDate < firstTradeDate && firstTradeDate < endDate)
                    {
                        startDate = firstTradeDate;
                    }
                }

                var startIndex = dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null ? 0 :
                                                             dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == startDate.ToString()).FirstOrDefault());

                var endIndex = dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault()) < 0 ? 0 :
                                                           dailyEquity.EquityByDate.IndexOf(dailyEquity.EquityByDate.Where(x => x.date == endDate.ToString()).FirstOrDefault());

                var NavsByTimeline = dailyEquity.EquityByDate.Skip(dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null ? 0 : startIndex + 1).Take(endIndex - startIndex).ToList();

                var lowestNav = NavsByTimeline.OrderBy(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                var lowestIndex = NavsByTimeline.IndexOf(lowestNav);
                var dd = new double();
                if (lowestIndex > 0)
                {
                    var NAVBeforeLowestIndex = NavsByTimeline.Take(lowestIndex + 1);
                    var highestNAV = NAVBeforeLowestIndex.OrderByDescending(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                    if (highestNAV.EquityWithoutWithdrawn != 0)
                    {
                        dd = Math.Round(((lowestNav.EquityWithoutWithdrawn - highestNAV.EquityWithoutWithdrawn) / highestNAV.EquityWithoutWithdrawn) * 100, 2);
                    }
                    else
                    {
                        dd = 0;
                    }
                }
                else
                    dd = 0;
                return dd;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateROIForTimeline(int timelineId, DailyEquity dailyEquity, List<TradeRecord> trades)
        {
            if (trades.Count > 0 && dailyEquity.EquityByDate.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date);
                else
                    startDate = GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = GetEndDateByTimeLineID(timelineId, startDate);

                var startingBalance = new double();
                if (dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).FirstOrDefault() == null)
                {
                    startingBalance = dailyEquity.EquityByDate[0].EquityWithoutWithdrawn;
                    startDate = Convert.ToDateTime(dailyEquity.EquityByDate[0].date).AddDays(1).Date;
                }
                else
                {
                    startingBalance = dailyEquity.EquityByDate.Where(x => x.date == startDate.AddDays(-1).ToString()).Select(x => x.EquityWithoutWithdrawn).FirstOrDefault();
                }

                var startSeconds = (uint)(Int32)(startDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var endSeconds = (uint)(Int32)(endDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                var buyTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Buy).Sum(x => x.Profit);
                var sellTrades = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && x.Cmd == TradeCommand.Sell).Sum(x => x.Profit);
                var Deposits = trades.Where(x => (x.OpenTime >= startSeconds && x.OpenTime < endSeconds) && (x.Cmd == TradeCommand.Balance || x.Cmd == TradeCommand.Credit)
                            && x.Profit > 0).Sum(x => x.Profit);


                var tradesAndDeposits = Deposits + buyTrades + sellTrades;
                var endingBalance = startingBalance + tradesAndDeposits;

                var roi = Math.Round(((endingBalance - (startingBalance + Deposits)) / (startingBalance + Deposits)) * 100, 2);

                if (startingBalance == 0)
                    return 0;
                else
                    return roi;
            }
            else
            {
                return 0;
            }
        }

        private DateTime GetStartDateByTimeLineID(int timelineId, DateTime startDate)
        {
            switch (timelineId)
            {
                #region Master
                case 1:
                    var year = new DateTime(startDate.Year, 1, 1);
                    startDate = year.AddYears(-1).Date;
                    break;
                case 2:
                    //var month = new DateTime(startDate.Year, startDate.Month, 1);
                    var month = GetStartingDateofHalfYear(startDate);
                    startDate = month.AddMonths(-6).Date;
                    break;
                case 3:
                    //month = new DateTime(startDate.Year, startDate.Month, 1);
                    month = GetStartingDateofQuater(startDate);
                    startDate = month.AddMonths(-3).Date;
                    break;
                case 4:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddMonths(-1).Date;
                    break;
                //case 5:
                //    var daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                //    startDate = startDate.AddDays(daysOfWeek).AddDays(-14).Date;
                //    break;
                case 6:
                    var daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-7).Date;
                    break;
                //case 45:
                //    startDate = startDate.Date;
                //    break;
                case 46:
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                    break;
                case 47:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).Date;
                    break;
                case 48:
                    startDate = GetStartingDateofQuater(startDate);
                    break;
                case 49:
                    startDate = new DateTime(startDate.Year, 1, 1);
                    break;
                #endregion

                #region Months
                case 62:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-24).Date;
                    break;
                case 63:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-23).Date;
                    break;
                case 64:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-22).Date;
                    break;
                case 65:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-21).Date;
                    break;
                case 66:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-20).Date;
                    break;
                case 67:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-19).Date;
                    break;
                case 68:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-18).Date;
                    break;
                case 69:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-17).Date;
                    break;
                case 70:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-16).Date;
                    break;
                case 71:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-15).Date;
                    break;
                case 72:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-14).Date;
                    break;
                case 73:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-13).Date;
                    break;
                case 7:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-12).Date;
                    break;
                case 8:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-11).Date;
                    break;
                case 9:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-10).Date;
                    break;
                case 10:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-9).Date;
                    break;
                case 11:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-8).Date;
                    break;
                case 12:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-7).Date;
                    break;
                case 13:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-6).Date;
                    break;
                case 14:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-5).Date;
                    break;
                case 15:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-4).Date;
                    break;
                case 16:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-3).Date;
                    break;
                case 17:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-2).Date;
                    break;
                case 18:
                    year = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = year.AddMonths(-1).Date;
                    break;
                #endregion

                #region Weekly
                case 50:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-168).Date;
                    break;
                case 51:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-161).Date;
                    break;
                case 52:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-154).Date;
                    break;
                case 53:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-147).Date;
                    break;
                case 54:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-140).Date;
                    break;
                case 55:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-133).Date;
                    break;
                case 56:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-126).Date;
                    break;
                case 57:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-119).Date;
                    break;
                case 58:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-112).Date;
                    break;
                case 59:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-105).Date;
                    break;
                case 60:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-98).Date;
                    break;
                case 61:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-91).Date;
                    break;
                case 19:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-84).Date;
                    break;
                case 20:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-77).Date;
                    break;
                case 21:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-70).Date;
                    break;
                case 22:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-63).Date;
                    break;
                case 23:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-56).Date;
                    break;
                case 24:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-49).Date;
                    break;
                case 25:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-42).Date;
                    break;
                case 26:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-35).Date;
                    break;
                case 27:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-28).Date;
                    break;
                case 28:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-21).Date;
                    break;
                case 29:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-14).Date;
                    break;
                case 30:
                    month = new DateTime(startDate.Year, startDate.Month, 1);
                    startDate = month.AddDays(-7).Date;
                    break;
                #endregion

                #region Daily
                case 31:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-14).Date;
                    break;
                case 32:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-13).Date;

                    break;
                case 33:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-12).Date;

                    break;
                case 34:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-11).Date;
                    break;
                case 35:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-10).Date;

                    break;
                case 36:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-9).Date;
                    break;
                case 37:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-8).Date;
                    break;
                case 38:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-7).Date;
                    break;
                case 39:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-6).Date;
                    break;
                case 40:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-5).Date;
                    break;
                case 41:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-4).Date;
                    break;
                case 42:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-3).Date;
                    break;
                case 43:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-2).Date;
                    break;
                case 44:
                    daysOfWeek = DayOfWeek.Monday - startDate.DayOfWeek;
                    startDate = startDate.AddDays(daysOfWeek).AddDays(-1).Date;
                    break;
                #endregion


                default:
                    break;
            }
            return startDate;
        }

        private DateTime GetEndDateByTimeLineID(int timelineId, DateTime startDate)
        {
            var endDate = new DateTime();
            switch (timelineId)
            {
                #region Master Timelines
                case 1:
                    endDate = startDate.AddYears(1).Date;
                    break;

                case 2:
                    endDate = startDate.AddMonths(6).Date;
                    break;

                case 3:
                    endDate = startDate.AddMonths(3).Date;
                    break;

                case 4:
                    endDate = startDate.AddMonths(1).Date;
                    break;

                case 5:
                    endDate = startDate.AddDays(14).Date;
                    break;

                case 6:
                    endDate = startDate.AddDays(7).Date;
                    break;
                #endregion

                #region Monthly
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                    endDate = startDate.AddMonths(1).Date;
                    break;
                #endregion

                #region Weekly
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                    endDate = startDate.AddDays(7).Date;
                    break;
                #endregion

                #region Daily
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                    endDate = startDate.AddDays(2).Date;
                    break;
                #endregion

                //case 45:
                //    endDate = startDate.AddDays(1).Date;
                //    break;

                default:
                    break;
            }
            return endDate;
        }

        public DateTime GetStartingDateofQuater(DateTime dtNow)
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

        public DateTime GetStartingDateofHalfYear(DateTime dtNow)
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

        public DateTime SecondsToDate(Int64 seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds).ToUniversalTime();
        }
        #endregion


        private void AddNotification(Guid organizationID, string notification)
        {
            var user = _unitOfWork.UserRepository.GetAll().Where(u => u.OrganizationID == organizationID && u.RoleID == 2).Single();
            user.Notifications.Add(new Notification()
            {
                id = Guid.NewGuid(),
                Text = notification,
                Date = DateTime.Now,
                IsRead = false
            });
            _unitOfWork.UserRepository.Update(user);
        }

        #endregion
    }
}
