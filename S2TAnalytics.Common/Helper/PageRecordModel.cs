using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Helper
{
    public class PageRecordModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, string> Filters { get; set; }
        public string SortOrder { get; set; }
        public string SortBy { get; set; }
        public Guid OrganizationID { get; set; }
        public ObjectId UserID { get; set; }
        public List<ObjectId> DatasourceIDs { get; set; }
        public List<string> ListFilter { get; set; }
        public Dictionary<string,List<string>> MultipleListFilter { get; set; }
        //public string TimeLineId { get; set; }

    }


}
