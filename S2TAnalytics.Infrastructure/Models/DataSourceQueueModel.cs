using S2TAnalytics.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
    public class DataSourceQueueModel
    {
        public string Id { get; set; }
        public string DatasourceId { get; set; }
        public int Status { get; set; }
        public DataSourceQueueModel ToDataSourceQueueModel(DataSourceQueue dataSourceQueue)
        {
            return new DataSourceQueueModel
            {
                Id = dataSourceQueue.Id.ToString(),               
                DatasourceId = dataSourceQueue.DatasourceId.ToString(),
                Status = dataSourceQueue.Status
            };
        }
    }
}
