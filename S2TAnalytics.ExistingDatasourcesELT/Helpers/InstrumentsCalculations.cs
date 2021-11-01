using P23.MetaTrader4.Manager.Contracts;
using S2TAnalytics.Common.Enums;
using S2TAnalytics.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.ExistingDatasourcesELT.Helpers
{
    public static class InstrumentsCalculations
    {        
        public static double GetVolumeOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);

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
        public static double GetProfitOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
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
        public static double GetLossOfInstrumentByTimelineId(int timelineId, List<TradeRecord> trades, string instrumentName)
        {
            if (trades.Count > 0)
            {
                var startDate = new DateTime();
                var endDate = new DateTime();
                if (timelineId == (int)TimeLineEnum.Overall) //timelineid= 45
                    startDate = Dates.SecondsToDate(trades[0].OpenTime).Date;
                else
                    startDate = Dates.GetStartDateByTimeLineID(timelineId, DateTime.Today.AddDays(-1).Date);

                if (Timelines.timelinesTillToday.Contains(timelineId))
                    endDate = DateTime.Today.AddDays(-1).Date;
                else
                    endDate = Dates.GetEndDateByTimeLineID(timelineId, startDate);
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
    }
}
