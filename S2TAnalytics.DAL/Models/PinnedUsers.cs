using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL.Models
{
    public class PinnedUsers : BaseEntity
    {
        public ObjectId UserId { get; set; }
        public List<ObjectId> AccountIds { get; set; }
    }
}
