using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IConfigureService
    {
        ServiceResponse Getusers(Guid organizationID);
        ServiceResponse GetDatasources(Guid organizationID);
        ServiceResponse SaveDatasource(DatasourceModel datasourceModel, ObjectId userId, Guid OrgnizationId);
        ServiceResponse UpdateDataTransformation(DatasourceModel datasourceModel, ObjectId userID, Guid organizationID);
        ServiceResponse DisconnectDatasource(DatasourceModel datasourceModel, ObjectId userId);
        ServiceResponse GetUsers(PageRecordModel pageRecordModel, out int userCount);
        ServiceResponse SaveUser(UserModel userModel);
        ServiceResponse UpdateUserAccess(bool IsBlock,List<string> emailIds);
        ServiceResponse DeleteUsers(List<string> emailIds, Guid organizatinId);
        ServiceResponse GetNotifications(ObjectId userId);
        ServiceResponse GetAllNotifications(ObjectId userId);
        ServiceResponse ReadNotifications(ObjectId userId);

        //Remove notifications
        ServiceResponse removeSingleNotification(ObjectId userId, Guid id);
        ServiceResponse removeAllNotification(ObjectId userId);

        ServiceResponse GetDataSourcesForUser(ObjectId userId, int RoleID, Guid organizationId);

        //Remove datasources
        ServiceResponse removeSingleDataSource(ObjectId userId, string id);
        ServiceResponse removeAllDataSource(ObjectId userId);

        ServiceResponse ChangeUserGroup(List<string> emailIds,List<string> userGroups);
        ServiceResponse ChangeDataSource(List<string> emailIds, List<string> dataSourceIds);
        ServiceResponse CheckForSynronizatedDataSource(string Id, ObjectId userId);
    }
}
