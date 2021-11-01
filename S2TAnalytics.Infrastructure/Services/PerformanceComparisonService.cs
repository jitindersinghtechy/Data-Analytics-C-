using MongoDB.Bson;
using S2TAnalytics.DAL.Interfaces;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.DAL.UnitOfWork;
using S2TAnalytics.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Services
{
    public class PerformanceComparisonService
    {
        public readonly IUnitOfWork _unitOfWork;
        public PerformanceComparisonService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<User> GetAllUsers()
        {
            var result = _unitOfWork.UserRepository.GetAll().ToList();
            return result;
        }

        public List<ObjectId> GetDataSourceIdsByOrganisationId(Guid organizationID)
        {
            var result = _unitOfWork.DatasourceRepository.GetAll().Where(x => x.OrganizationId == organizationID).ToList().Select(x => x.Id).ToList();
            return result;
        }

        public Dictionary<string, List<PerformersModel>> GetTopFivePerformersByDataSourceIds(List<ObjectId> dataSourceIds, Dictionary<string, int> timeline, string sortBy)
        {
            var result = new Dictionary<string, List<PerformersModel>>();
            var startTimelineId = timeline.Where(x => x.Key == "StartTimelineId").Select(x => x.Value).FirstOrDefault();
            var endTimelineId = timeline.Where(x => x.Key == "EndTimelineId").Select(x => x.Value).FirstOrDefault();

            var performerModel = new List<PerformersModel>();
            IEnumerable<PerformersModel> EnumerablePerformersModel;
            EnumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => dataSourceIds.Contains(x.DataSourceId)).ToList()
                       .Select(p => new PerformersModel()
                       {
                           AccountDetailId = p.Id.ToString(),
                           AccountNumber = p.AccountNumber,
                           ROI = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().ROI : 0,
                           DD = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().DD : 0,
                           BestPL = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().BestPL : "",
                           Leverage = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().Leverage : 0,
                           NAV = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().NAV : 0,
                           PerformerName = p.Name,
                           WINRate = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().WINRate : 0,
                           SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().SharpRatio : 0,
                           UserGroup = p.UserGroup
                       });
            if (sortBy == "MaxDD")
                performerModel = EnumerablePerformersModel.OrderByDescending(o => o.DD).Take(5).ToList();
            else if (sortBy == "WinRate")
                performerModel = EnumerablePerformersModel.OrderByDescending(o => o.WINRate).Take(5).ToList();
            else if (sortBy == "SharpRatio")
                performerModel = EnumerablePerformersModel.OrderByDescending(o => o.SharpRatio).Take(5).ToList();
            else
                performerModel = EnumerablePerformersModel.OrderByDescending(o => o.ROI).Take(5).ToList();

            var topAccountIds = performerModel.Select(x => ObjectId.Parse(x.AccountDetailId)).ToList();
            var comparePerformerModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => topAccountIds.Contains(x.Id)).ToList()
                       .Select(p => new PerformersModel()
                       {
                           AccountDetailId = p.Id.ToString(),
                           AccountNumber = p.AccountNumber,
                           ROI = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().ROI : 0,
                           DD = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().DD : 0,
                           BestPL = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().BestPL : "",
                           Leverage = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().Leverage : 0,
                           NAV = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().NAV : 0,
                           PerformerName = p.Name,
                           WINRate = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().WINRate : 0,
                           SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().SharpRatio : 0,
                           UserGroup = p.UserGroup
                       }).ToList();//.OrderByDescending(o => o.ROI).Take(5).ToList();;

            result.Add("PerformerModels", performerModel);
            result.Add("ComparePerformerModels", comparePerformerModel);
            return result;
        }

        public Dictionary<string, List<PerformersModel>> GetTopPinnedPerformersByUserId(ObjectId userId, Dictionary<string, int> timeline, string sortBy)
        {
            var result = new Dictionary<string, List<PerformersModel>>();
            var startTimelineId = timeline.Where(x => x.Key == "StartTimelineId").Select(x => x.Value).FirstOrDefault();
            var endTimelineId = timeline.Where(x => x.Key == "EndTimelineId").Select(x => x.Value).FirstOrDefault();
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == userId).Select(x => x.AccountIds).FirstOrDefault();
            if (pinnedUsers != null)
            {
                var performerModel = new List<PerformersModel>();
                IEnumerable<PerformersModel> EnumerablePerformersModel;
                EnumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.Contains(x.Id)).ToList()
                           .Select(p => new PerformersModel()
                           {
                               AccountDetailId = p.Id.ToString(),
                               AccountNumber = p.AccountNumber,
                               ROI = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().ROI : 0,
                               DD = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().DD : 0,
                               BestPL = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().BestPL : "",
                               Leverage = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().Leverage : 0,
                               NAV = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().NAV : 0,
                               PerformerName = p.Name,
                               WINRate = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().WINRate : 0,
                               SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().SharpRatio : 0,
                               UserGroup = p.UserGroup
                           });
                if (sortBy == "MaxDD")
                    performerModel = EnumerablePerformersModel.OrderByDescending(o => o.DD).Take(5).ToList();
                else if (sortBy == "WinRate")
                    performerModel = EnumerablePerformersModel.OrderByDescending(o => o.WINRate).Take(5).ToList();
                else if (sortBy == "SharpRatio")
                    performerModel = EnumerablePerformersModel.OrderByDescending(o => o.SharpRatio).Take(5).ToList();
                else
                    performerModel = EnumerablePerformersModel.OrderByDescending(o => o.ROI).Take(5).ToList();

                var accountIds = performerModel.Select(x => ObjectId.Parse(x.AccountDetailId)).ToList();
                var comparePerformerModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => accountIds.Contains(x.Id)).ToList()
                           .Select(p => new PerformersModel()
                           {
                               AccountDetailId = p.Id.ToString(),
                               AccountNumber = p.AccountNumber,
                               ROI = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().ROI : 0,
                               DD = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().DD : 0,
                               BestPL = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().BestPL : "",
                               Leverage = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().Leverage : 0,
                               NAV = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().NAV : 0,
                               PerformerName = p.Name,
                               WINRate = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().WINRate : 0,
                               SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().SharpRatio : 0,
                               UserGroup = p.UserGroup
                           }).ToList();//.OrderByDescending(o => o.ROI).Take(5).ToList();;

                result.Add("PerformerModels", performerModel);
                result.Add("ComparePerformerModels", comparePerformerModel);
                return result;
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, List<PerformersModel>> GetLowPinnedPerformersByUserId(ObjectId userId, Dictionary<string, int> timeline, string sortBy)
        {
            var result = new Dictionary<string, List<PerformersModel>>();
            var startTimelineId = timeline.Where(x => x.Key == "StartTimelineId").Select(x => x.Value).FirstOrDefault();
            var endTimelineId = timeline.Where(x => x.Key == "EndTimelineId").Select(x => x.Value).FirstOrDefault();
            var pinnedUsers = _unitOfWork.PinnedUsersRepository.GetAll().Where(x => x.UserId == userId).Select(x => x.AccountIds).FirstOrDefault();
            if (pinnedUsers != null)
            {
                var performerModel = new List<PerformersModel>();
                IEnumerable<PerformersModel> EnumerablePerformersModel;
                EnumerablePerformersModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => pinnedUsers.Contains(x.Id)).ToList()
                           .Select(p => new PerformersModel()
                           {
                               AccountDetailId = p.Id.ToString(),
                               AccountNumber = p.AccountNumber,
                               ROI = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().ROI : 0,
                               DD = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().DD : 0,
                               BestPL = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().BestPL : "",
                               Leverage = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().Leverage : 0,
                               NAV = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().NAV : 0,
                               PerformerName = p.Name,
                               WINRate = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().WINRate : 0,
                               SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == startTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == startTimelineId).First().SharpRatio : 0,
                               UserGroup = p.UserGroup
                           });
                if (sortBy == "MaxDD")
                    performerModel = EnumerablePerformersModel.OrderBy(o => o.DD).Take(5).ToList();
                else if (sortBy == "WinRate")
                    performerModel = EnumerablePerformersModel.OrderBy(o => o.WINRate).Take(5).ToList();
                else if (sortBy == "SharpRatio")
                    performerModel = EnumerablePerformersModel.OrderBy(o => o.SharpRatio).Take(5).ToList();
                else
                    performerModel = EnumerablePerformersModel.OrderBy(o => o.ROI).Take(5).ToList();

                var accountIds = performerModel.Select(x => ObjectId.Parse(x.AccountDetailId)).ToList();
                var comparePerformerModel = _unitOfWork.AccountDetailRepository.GetAll().Where(x => accountIds.Contains(x.Id)).ToList()
                           .Select(p => new PerformersModel()
                           {
                               AccountDetailId = p.Id.ToString(),
                               AccountNumber = p.AccountNumber,
                               ROI = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().ROI : 0,
                               DD = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().DD : 0,
                               BestPL = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().BestPL : "",
                               Leverage = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().Leverage : 0,
                               NAV = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().NAV : 0,
                               PerformerName = p.Name,
                               WINRate = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().WINRate : 0,
                               SharpRatio = p.AccountStats.Count(ast => ast.TimeLineId == endTimelineId) > 0 ? p.AccountStats.Where(ast => ast.TimeLineId == endTimelineId).First().SharpRatio : 0,
                               UserGroup = p.UserGroup
                           }).ToList();//.OrderByDescending(o => o.ROI).Take(5).ToList();;

                result.Add("PerformerModels", performerModel);
                result.Add("ComparePerformerModels", comparePerformerModel);
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
