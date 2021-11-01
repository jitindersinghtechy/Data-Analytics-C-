using System;
using System.Linq;
using MongoDB.Driver;
using System.Linq.Expressions;
using MongoDB.Driver.Builders;
using S2TAnalytics.DAL.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using S2TAnalytics.Common.Helper;
using MongoDB.Bson;

namespace S2TAnalytics.DAL.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private IMongoDatabase _database;
        private IMongoCollection<T> _collection;

        // constructor to initialise database and table/collection  
        public BaseRepository(IMongoDatabase db)
        {
            _database = db;
            _collection = _database.GetCollection<T>(typeof(T).Name);
        }

        /// <summary>
        /// Get all records 
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            return _collection.AsQueryable();
        }

        /// <summary>
        /// Get collection 
        /// </summary>
        /// <returns></returns>
        public IMongoCollection<T> GetCollection()
        {
            return _collection;
        }
        /// <summary>
        ///  Generic single result by Id
        /// </summary>
        /// <param name="id"></param>
        public T GetById(string id)
        {
            var filter = new FilterDefinitionBuilder<T>().Eq("Id", ObjectId.Parse(id));
            return _collection.Find(filter).SingleOrDefault();
        }

        public void Delete(Expression<Func<T, bool>> filter)
        {
            _collection.DeleteMany<T>(filter);
        }
        public List<T> GetByFilters(FilterDefinition<T> filterDefinition)
        {
            return _collection.Find(filterDefinition).ToList();
        }

        /// <summary>
        /// Get Records for a page 
        /// </summary>
        /// <returns></returns>
        public List<T> GetPagedRecords(PageRecordModel pageRecordModel, SortDefinition<T> sortDefinition = null, FilterDefinition<T> filterDefinition = null, ProjectionDefinition<T> projection = null)
        {
            if (sortDefinition == null && filterDefinition == null)
                return _collection.Find(x => true).Skip(pageRecordModel.PageSize * (pageRecordModel.PageNumber - 1)).Limit(pageRecordModel.PageSize).ToList();
            else if (sortDefinition != null && filterDefinition != null)
                return _collection.Find(filterDefinition).Skip(pageRecordModel.PageSize * (pageRecordModel.PageNumber - 1)).Limit(pageRecordModel.PageSize).Sort(sortDefinition).ToList();
            else if (sortDefinition == null && filterDefinition != null)
                return _collection.Find(filterDefinition).Skip(pageRecordModel.PageSize * (pageRecordModel.PageNumber - 1)).Limit(pageRecordModel.PageSize).ToList();
            else if (sortDefinition != null && filterDefinition == null)
                return _collection.Find(x => true).Skip(pageRecordModel.PageSize * (pageRecordModel.PageNumber - 1)).Limit(pageRecordModel.PageSize).Sort(sortDefinition).ToList();
            else
                return _collection.Find(filterDefinition).Skip(pageRecordModel.PageSize * (pageRecordModel.PageNumber - 1)).Limit(pageRecordModel.PageSize).Sort(sortDefinition).ToList();
        }

        public List<T> GetPagedRecordsLinq(PageRecordModel pageRecordModel, Expression<Func<T, bool>> whereCondition, Expression<Func<T, string>> orderBy, string sortDirection = "asc")
        {
            if (sortDirection == "asc")
                return (_collection.AsQueryable().Where(whereCondition).OrderBy(orderBy).Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize)).ToList();
            else
                return (_collection.AsQueryable().Where(whereCondition).OrderByDescending(orderBy).Skip((pageRecordModel.PageNumber - 1) * pageRecordModel.PageSize).Take(pageRecordModel.PageSize)).ToList();
        }
        public int GetTotalRecordsCount(FilterDefinition<T> filterDefinition = null)
        {
            if (filterDefinition == null)
                return Convert.ToInt32(_collection.Find(x => true).Count());
            else
                return Convert.ToInt32(_collection.Find(filterDefinition).Count());
        }
        /// <summary>
        /// Generic add method to insert enities to collection 
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void AddMultiple(List<T> entity)
        {
            _collection.InsertMany(entity);
        }

    /// <summary>
    ///  Generic update method to update record on the basis of id
    /// </summary>
    /// <param name="queryExpression"></param>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    public async Task<UpdateResult> UpdateOne(string id, UpdateDefinition<T> update)
        {
            var filter = new FilterDefinitionBuilder<T>().Eq("Id", id);
            return await _collection.UpdateOneAsync(filter, update);
        }
        /// <summary>
        ///  Generic update method to update record on the basis of Entity model
        /// </summary>
        /// <param name="queryExpression"></param>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        public void Update(T item)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(e => e.Id, item.Id);
            _collection.ReplaceOne(filter, item);
        }

    }
}
