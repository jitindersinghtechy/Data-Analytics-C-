using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class Notification
    {
        public Guid id { get; set; }
        public ObjectId? From { get; set; }

        public string FromName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }

    }
}
