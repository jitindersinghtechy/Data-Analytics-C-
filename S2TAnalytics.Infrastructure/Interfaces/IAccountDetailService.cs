using S2TAnalytics.Common.Helper;
using S2TAnalytics.Common.Utilities;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Interfaces
{
    public interface IAccountDetailService
    {
        AnalyticsResponse GetInstrumentStats(Guid orgId, int timelineId, string[] userGroups, string instrument, string country = "", string city = "");
        AnalyticsResponse GetComparisonData(Guid orgId, int timelineId, string[] userGroups, string country = "", string city = "", string selectedSeries = "");
        AnalyticsResponse GetInstrumentStatsByGroup(Guid orgId, int timeLineId, string[] userGroups, string instrument = "", string userGroup = "");
        AnalyticsResponse GetNavMapData(Guid orgId, int timelineId, string[] userGroups, string userGroup="", string country = "", string city = "");
    }
}
