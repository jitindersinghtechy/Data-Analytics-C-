using MongoDB.Bson;
using S2TAnalytics.Common.Helper;
using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class DatasourceModel
    {
        public DatasourceModel()
        {
            this.RateOfReturns = new List<RateOfReturns>();
        }
        public string Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string DatasourceId { get; set; } //Oanda , MT4
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
        #endregion
        public bool IsConnected { get; set; }
        public bool IsConfigured { get; set; }
        public List<RateOfReturns> RateOfReturns { get; set; }

        public List<DatasourceModel> ToDatasourceModel(List<Datasource> datasources)
        {
            return datasources.Select(d => new DatasourceModel()
            {
                Id = d.Id.ToString(),
                OrganizationId = d.OrganizationId,
                AccessToken = d.AccessToken,
                DatasourceId = d.DatasourceId.Encrypt(),
                IsDemo = d.IsDemo,
                LoginId = d.LoginId,
                Password = d.Password,
                Server = d.Server,
                User = d.User,
                IsConnected = d.IsConnected,
                IsConfigured = d.IsConfigured,
                Name = d.Name,
                RateOfReturns = d.RateOfReturns
            }).ToList();
        }
    }
}
