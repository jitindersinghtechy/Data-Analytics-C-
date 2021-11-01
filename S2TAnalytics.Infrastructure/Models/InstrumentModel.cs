using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Infrastructure.Models
{
  public  class InstrumentModel
    {
        public ObjectId InstrumentIdObjectId { get; set; }
        public string InstrumentId { get; set; }
        public string PerformerName { get; set; }
        public string AccountNumber { get; set; }
        public string Volume { get; set; }
        public string Location { get; set; }
        public string UserGroup { get; set; }


    }
}
