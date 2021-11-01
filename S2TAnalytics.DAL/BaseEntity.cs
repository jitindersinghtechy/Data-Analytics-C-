using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.DAL
{
    public abstract class BaseEntity
    {
        public ObjectId Id { get; set; }
    }
}
