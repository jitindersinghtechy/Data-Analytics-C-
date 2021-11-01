using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    [BsonIgnoreExtraElements]
    public class Datasource : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public int DatasourceId { get; set; } //Oanda , MT4
        #region OANDA Properties
        public string AccessToken { get; set; }//AccessToken = "c5ad79f200e1176f93eb79d189308f42-d5b241034d259888f586184af29b47dc";
        public Boolean IsDemo { get; set; }//false
        public string User { get; set; } //""
        #endregion
        #region MT4 Properties        
        public int LoginId { get; set; }//995
        public string Name { get; set; }
        public string Password { get; set; }//Welcome2021@",
        public string Server { get; set; } //"us03-demo.mt4tradeserver.com:443"
        public bool IsConnected { get; set; }
        public bool IsConfigured { get; set; }
        public List<RateOfReturns> RateOfReturns { get; set; }
        #endregion

    }
}
