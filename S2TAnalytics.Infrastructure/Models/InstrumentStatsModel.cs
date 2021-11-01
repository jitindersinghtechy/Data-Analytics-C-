using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class InstrumentStatsModel
    {
        public Object AccountDailyStatsId { get; set; }
        public int InstrumentId { get; set; }
        public string InstrumentName { get; set; }
        public int TimeLineId { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string UserGroup { get; set; }
        public double NAV { get; set; }
        public double ROI { get; set; }
        public double WINRate { get; set; }
        public double BuyRate { get; set; }
        public double Volume { get; set; }
        public bool Status { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }


        public List<InstrumentStatsModel> AccountDetailToInstrumentStatsModel(List<AccountDetail> accountDetails)
        {
            var instrumentStatsmodel = new List<InstrumentStatsModel>();
            if (accountDetails.Count <= 0)
                return instrumentStatsmodel;
            var instrumentStats = accountDetails.SelectMany(x => x.InstrumentStats);
            return accountDetails.Select(z => new InstrumentStatsModel
            {
                Name = z.Name,
                City = z.City,
                Country = z.Country,
                ROI = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].ROI : 0,
                WINRate = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].WINRate : 0,
                AccountDailyStatsId = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].AccountStatsId : 0,
                BuyRate = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].BuyRate : 0,
                InstrumentName = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].InstrumentName : string.Empty,
                Profit = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].Profit : 0,
                Loss = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].Loss : 0,
                //InstrumentName = z.InstrumentStats.Count() > 0 ? ((InstrumentMasterEnum)Enum.ToObject(typeof(InstrumentMasterEnum), z.InstrumentStats[0].InstrumentId)).GetEnumDisplayName() : string.Empty,
                //InstrumentName = z.InstrumentStats.Where(x => x.TimeLineId == 2).Count() > 0 ? ((InstrumentMasterEnum)Enum.ToObject(typeof(InstrumentMasterEnum), z.InstrumentStats.Where(x => x.TimeLineId == 2).FirstOrDefault().InstrumentId)).GetEnumDisplayName() : "",
                //z.InstrumentStats.Where(x => x.TimeLineId == 2).FirstOrDefault().InstrumentId : "",
                InstrumentId = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].InstrumentId : 0,
                //InstrumentId = z.InstrumentStats.Where(x => x.TimeLineId == 2).Count() > 0 ? z.InstrumentStats.Where(x => x.TimeLineId == 2).FirstOrDefault().InstrumentId: 0,
                NAV = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].NAV : 0,
                Status = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].Status : false,
                TimeLineId = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].TimeLineId : 0,
                //TimeLineId= z.InstrumentStats.Where(x => x.TimeLineId == 2).Count() > 0 ? z.InstrumentStats.Where(x=>x.TimeLineId==2).FirstOrDefault().TimeLineId:0,
                UserGroup = z.UserGroup,
                Volume = z.InstrumentStats.Count() > 0 ? z.InstrumentStats[0].Volume : 0,
            }).ToList();
            //var instrument = accountDetails.SelectMany(z => z.InstrumentStats).ToList();
            //instrumentStatsmodel = new InstrumentStatsModel().ToInstrumentStatsModel(instrument);
            //instrumentStatsmodel.ForEach(m => m.Country = accountDetails.Country);
            //instrumentStatsmodel.ForEach(m => m.City= accountDetails.City);
            //instrumentStatsmodel.ForEach(m => m. UserGroup= accountDetails.UserGroup);
            //instrumentStatsmodel.ForEach(m=>m.CountriesModel=);
        }

        public List<InstrumentStatsModel> ToInstrumentStatsModel(List<InstrumentStats> model)
        {
            if (model.Count <= 0)
                return new List<InstrumentStatsModel>();

            return model.Select(m => new InstrumentStatsModel
            {
                AccountDailyStatsId = m.AccountStatsId,
                InstrumentName = m.InstrumentName,
                //InstrumentName = ((InstrumentMasterEnum)Enum.ToObject(typeof(InstrumentMasterEnum), m.InstrumentId)).GetEnumDisplayName(),
                InstrumentId = m.InstrumentId,

                BuyRate = m.BuyRate,
                WINRate = m.WINRate,
                Volume = m.Volume,
                TimeLineId = m.TimeLineId,
                NAV = m.NAV,
                ROI = m.ROI,
                Status = m.Status,
                Profit = m.Profit,
                Loss = m.Loss
            }).ToList();
        }
        public List<InstrumentStats> ToInstrumentStats(List<InstrumentStatsModel> model)
        {
            if (model.Count <= 0)
                return new List<InstrumentStats>();

            return model.Select(m => new InstrumentStats
            {
                AccountStatsId = m.AccountDailyStatsId,
                InstrumentId = m.InstrumentId,
                BuyRate = m.BuyRate,
                WINRate = m.WINRate,
                Volume = m.Volume,
                TimeLineId = m.TimeLineId,
                NAV = m.NAV,
                ROI = m.ROI,
                Status = m.Status,
                Profit = m.Profit,
                Loss = m.Loss
            }).ToList();
        }
    }
}
