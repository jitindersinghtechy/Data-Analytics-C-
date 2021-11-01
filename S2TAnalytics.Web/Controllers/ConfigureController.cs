using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Interfaces;
using S2TAnalytics.Infrastructure.Models;
using S2TAnalytics.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;

namespace S2TAnalytics.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Configure")]
    public class ConfigureController : BaseController
    {
        public readonly IConfigureService _configureService;
        public readonly IUserService _userService;

        public ConfigureController(IConfigureService configureService, IUserService userService) : base(userService)
        {
            _configureService = configureService;
            _userService = userService;
        }
        [HttpGet]
        [Route("GetUsers")]
        public IHttpActionResult GetUsers()
        {
            var result = _configureService.Getusers(OrganizationID);
            return Ok(result);
        }

        [HttpPost]
        [Route("AddUser")]
        public IHttpActionResult AddUser(UserModel userModel)
        {
            userModel.OrganizationID = OrganizationID;

            return Ok(_configureService.SaveUser(userModel));
        }

        [HttpPost]
        [Route("GetUsersList")]
        public IHttpActionResult GetUsersList(PageRecordModel pageRecordModel)
        {
            int userCount = 0;
            pageRecordModel.OrganizationID = OrganizationID;

            return Ok(new { users = _configureService.GetUsers(pageRecordModel, out userCount), count = userCount });
        }


        [HttpGet]
        [Route("GetDatasources")]
        public IHttpActionResult GetDatasources()
        {
            var result = _configureService.GetDatasources(OrganizationID);
            return Ok(result);
        }

        [HttpPost]
        [Route("SaveDatasource")]
        public IHttpActionResult SaveDatasource(DatasourceModel datasourceModel)
        {
            datasourceModel.OrganizationId = OrganizationID;
            var result = _configureService.SaveDatasource(datasourceModel, UserID, OrganizationID);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateDataTransformation")]
        public IHttpActionResult UpdateDataTransformation(DatasourceModel datasourceModel)
        {
            datasourceModel.OrganizationId = OrganizationID;
            var result = _configureService.UpdateDataTransformation(datasourceModel, UserID, OrganizationID);
            return Ok(result);
        }

        [HttpPost]
        [Route("DeleteUsers")]
        public IHttpActionResult DeleteUsers(List<string> emailIds)
        {
            return Ok(_configureService.DeleteUsers(emailIds, OrganizationID));
        }

        [HttpPost]
        [Route("UpdateUserAccess")]
        public IHttpActionResult UpdateUserAccess(UpdateUserModel userModel)
        {
            return Ok(_configureService.UpdateUserAccess(userModel.IsBlock, userModel.EmailIds));
        }

        [HttpGet]
        [Route("GetNotifications")]
        public IHttpActionResult GetNotifications()
        {
            return Ok(_configureService.GetNotifications(UserID));
        }

        [HttpGet]
        [Route("GetAllNotifications")]
        public IHttpActionResult GetAllNotifications()
        {
            return Ok(_configureService.GetAllNotifications(UserID));
        }

        [HttpPost]
        [Route("ReadNotifications")]
        public IHttpActionResult ReadNotifications()
        {
            return Ok(_configureService.ReadNotifications(UserID));
        }

        [HttpGet]
        [Route("RemoveSingleNotification/{id}")]
        public IHttpActionResult RemoveSingleNotification(Guid id)
        {
            return Ok(_configureService.removeSingleNotification(UserID, id));
        }

        [HttpGet]
        [Route("RemoveAllNotification")]
        public IHttpActionResult RemoveAllNotification()
        {
            return Ok(_configureService.removeAllNotification(UserID));
        }

        [HttpPost]
        [Route("DisconnectDatasource")]
        public IHttpActionResult DisconnectDatasource(DatasourceModel datasourceModel)
        {
            return Ok(_configureService.DisconnectDatasource(datasourceModel, UserID));
        }

        [HttpGet]
        [Route("GetDataSourcesForUser")]
        public IHttpActionResult GetDataSourcesForUser()
        {
            return Ok(_configureService.GetDataSourcesForUser(UserID, RoleID, OrganizationID));
        }

        [HttpGet]
        [Route("RemoveSingleDataSource/{id}")]
        public IHttpActionResult RemoveSingleDataSource(string id)
        {
            return Ok(_configureService.removeSingleDataSource(UserID, id));
        }

        [HttpGet]
        [Route("RemoveAllDataSource")]
        public IHttpActionResult RemoveAllDataSource()
        {
            return Ok(_configureService.removeAllDataSource(UserID));
        }

        [HttpPost]
        [Route("FilterDatasource")]
        public IHttpActionResult FilterDatasource(List<string> datasourceIds)
        {
            _userService.SetSelectedDatasources(datasourceIds, UserID);

            return Ok(true);
        }

        [HttpPost]
        [Route("UpdateSelectedDataSources/{datasourceId}")]
        public IHttpActionResult UpdateSelectedDataSources(string datasourceId)
        {
            _userService.UpdateSelectedDataSources(datasourceId, UserID);
            return Ok(true);
        }
        

        [HttpPost]
        [Route("ChangeUserGroup")]
        public IHttpActionResult ChangeUserGroup(UpdateUserModel userModel)
        {
            return Ok(_configureService.ChangeUserGroup(userModel.EmailIds, userModel.UserGroups));
        }

        [HttpPost]
        [Route("ChangeDataSource")]
        public IHttpActionResult ChangeDataSource(UpdateUserModel userModel)
        {
            return Ok(_configureService.ChangeDataSource(userModel.EmailIds, userModel.DataSourceIds));
        }

        [HttpGet]
        [Route("CheckForSynronizatedDataSource/{id}")]
        public IHttpActionResult CheckForSynronizatedDataSource(string id)
        {
            return Ok(_configureService.CheckForSynronizatedDataSource(id, UserID));
        }
    }
}
