using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Enums
{
    public enum TimeLineEnum
    {
        #region Master Timelines
        [Display(Name = "Previous Year")]
        PreviousYear = 1,
        [Display(Name = "Previous Half Year")]
        PreviousHalfYear = 2,
        [Display(Name = "Previous Quater")]
        PreviousQuater = 3,
        [Display(Name = "Previous Month")]
        PreviousMonth = 4,
        //[Display(Name = "Previous Two Weeks")]
        //PreviousTwoWeeks = 5,
        [Display(Name = "Previous Week")]
        PreviousWeek = 6,
        [Display(Name = "Overall")]
        Overall = 45,
        [Display(Name = "MTD")]
        MTD = 46,
        [Display(Name = "Current Week")]
        CurrentWeek = 47,
        [Display(Name = "Current Quarter")]
        CurrentQuarter = 48,
        [Display(Name = "Current Year")]
        CurrentYear = 49,
        #endregion


        #region Monthly
        [Display(Name = "24th Last Month")]
        LastTwentyFourMonth = 62,
        [Display(Name = "23rd Last Month")]
        LastTwentyThreeMonth = 63,
        [Display(Name = "22nd Last Month")]
        LastTwnetyTwoMonth = 64,
        [Display(Name = "21st Last Month")]
        LastTwentyOneMonth = 65,
        [Display(Name = "20th Last Month")]
        LastTwentyMonth = 66,
        [Display(Name = "19th Last Month")]
        LastNineteenMonth = 67,
        [Display(Name = "18th Last Month")]
        LastEighteenMonth = 68,
        [Display(Name = "17th Last Month")]
        LastSeventeenMonth = 69,
        [Display(Name = "16th Last Month")]
        LastSixteenMonth = 70,
        [Display(Name = "15th Last Month")]
        LastFifteenMonth = 71,
        [Display(Name = "14th Last Month")]
        LastFourteenMonth = 72,
        [Display(Name = "13th Last Month")]
        LastThirteenMonth = 73,
        [Display(Name = "12th Last Month")]
        LastTwelveMonth = 7,
        [Display(Name = "11th Last Month")]
        LastElevenMonth = 8,
        [Display(Name = "10th Last Month")]
        LastTenMonth = 9,
        [Display(Name = "9th Last Month")]
        LastNineMonth = 10,
        [Display(Name = "8th Last Month")]
        LasEighttMonth = 11,
        [Display(Name = "7th Last Month")]
        LastSevenMonth = 12,
        [Display(Name = "6th Last Month")]
        LastSixMonth = 13,
        [Display(Name = "5th Last Month")]
        LastFiveMonth = 14,
        [Display(Name = "4th Last Month")]
        LastFourMonth = 15,
        [Display(Name = "3rd Last Month")]
        LastThreeMonth = 16,
        [Display(Name = "2nd Last Month")]
        LastTwoMonth = 17,
        [Display(Name = "Last Month")]
        LastMonth = 18,
        #endregion

        #region Weekly
        [Display(Name = "24th Last Week")]
        LastTwentyFourWeek = 50,
        [Display(Name = "23rd Last Week")]
        LastTwentyThreeWeek = 51,
        [Display(Name = "22nd Last Week")]
        LastTwnetyTwoWeek = 52,
        [Display(Name = "21st Last Week")]
        LastTwentyOneWeek = 53,
        [Display(Name = "20th Last Week")]
        LastTwentyWeek = 54,
        [Display(Name = "19th Last Week")]
        LastNineteenWeek = 55,
        [Display(Name = "18th Last Week")]
        LastEighteenWeek = 56,
        [Display(Name = "17th Last Week")]
        LastSeventeenWeek = 57,
        [Display(Name = "16th Last Week")]
        LastSixteenWeek = 58,
        [Display(Name = "15th Last Week")]
        LastFifteenWeek = 59,
        [Display(Name = "14th Last Week")]
        LastFourteenWeek = 60,
        [Display(Name = "13th Last Week")]
        LastThirteenWeek = 61,
        [Display(Name = "12th Last Week")]
        LastTwelveWeek = 19,
        [Display(Name = "11th Last Week")]
        LastElevenWeek = 20,
        [Display(Name = "10th Last Week")]
        LastTenWeek = 21,
        [Display(Name = "9th Last Week")]
        LastNineWeek = 22,
        [Display(Name = "8th Last Week")]
        LastEightWeek = 23,
        [Display(Name = "7th Last Week")]
        LastSevenWeek = 24,
        [Display(Name = "6th Last Week")]
        LastSixWeek = 25,
        [Display(Name = "5th Last Week")]
        LastFiveWeek = 26,
        [Display(Name = "4th Last Week")]
        LastFourWeek = 27,
        [Display(Name = "3rd Last Week")]
        LastThreeWeek = 28,
        [Display(Name = "2nd Last Week")]
        LastTwoWeek = 29,
        [Display(Name = "Last Week")]
        LastWeek = 30,
        #endregion

        #region Daily
        [Display(Name = "14th Last Day")]
        LastFourteenDay = 31,
        [Display(Name = "13th Last Day")]
        LastThirteenDay = 32,
        [Display(Name = "12th Last Day")]
        LastTwelveDay = 33,
        [Display(Name = "11th Last Day")]
        LastElevenDay = 34,
        [Display(Name = "10th Last Day")]
        LastTenDay = 35,
        [Display(Name = "9th Last Day")]
        LastNineDay = 36,
        [Display(Name = "8th Last Day")]
        LastEightDay = 37,
        [Display(Name = "7th Last Day")]
        LastSevenDay = 38,
        [Display(Name = "6th Last Day")]
        LastSixDay = 39,
        [Display(Name = "5th Last Day")]
        LastFiveDay = 40,
        [Display(Name = "4th Last Day")]
        LastFourDay = 41,
        [Display(Name = "3rd Last Day")]
        LastThreeDay = 42,
        [Display(Name = "2nd Last Day")]
        LastTwoDay = 43,
        [Display(Name = "Last Day")]
        LastDay = 44, 
        #endregion
    }
}
