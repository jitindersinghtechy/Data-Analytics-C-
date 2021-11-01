using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.ExistingDatasourcesELT.Helpers
{
    public static class Dates
    {
        public static DateTime GetStartDateByTimeLineID(int timelineId, DateTime startDate)
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
        public static DateTime GetEndDateByTimeLineID(int timelineId, DateTime startDate)
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
        public static DateTime GetStartingDateofQuater(DateTime dtNow)
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
        public static DateTime GetStartingDateofHalfYear(DateTime dtNow)
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
        public static DateTime SecondsToDate(Int64 seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds).ToUniversalTime();
        }
    }
}
