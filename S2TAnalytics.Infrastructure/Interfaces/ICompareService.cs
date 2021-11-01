using MongoDB.Bson;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface ICompareService
    {
        ServiceResponse GetCompareData(Guid OrganationId, ObjectId USerID);
        ServiceResponse GetSingleAccount(string AccontNumber, int TimeLineId, Guid OrganationId);
        ServiceResponse GetSingleAccountByAccountID(string AccontNumber, int TimeLineId, Guid OrganationId);

    }
}
