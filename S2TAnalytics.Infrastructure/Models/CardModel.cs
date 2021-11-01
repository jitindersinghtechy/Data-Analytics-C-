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
    public class CardModel
    {
        public Guid Id { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CVV { get; set; }
        public string PaymentType { get; set; }
        public string PaymentTypeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DisplayCardName { get; set; }
        public List<CardModel> ToCardModel(List<UserCard> userCards)
        {
            if (userCards == null)
                return null;

            return userCards.Select(d => new CardModel()
            {
                Id = d.Id,
                CardName = d.CardName,
                CardNumber = d.CardNumber,
                ExpirationMonth = d.ExpirationMonth,
                ExpirationYear = d.ExpirationYear,
                CreatedDate = d.CreatedDate,               
                IsActive = d.IsActive,
                PaymentTypeName= ((PaymentTypeEnum)Enum.ToObject(typeof(PaymentTypeEnum), Convert.ToInt32(d.PaymentType))).GetEnumDisplayName(),
                DisplayCardName ="XXXXXXXXXXXX" + d.CardNumber.Substring(d.CardNumber.Length - 4)
        }).ToList();
        }
    }
}
