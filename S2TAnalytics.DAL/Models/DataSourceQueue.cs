using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class DataSourceQueue : BaseEntity
    {
        public ObjectId DatasourceId { get; set; }
        public int Status { get; set; }
        public Guid OrganizationId { get; set; }
        public int LoginId { get; set; }//995
        public string Password { get; set; }//Welcome2021@",
        public string Server { get; set; } //"us03-demo.mt4tradeserver.com:443"
    }
}
