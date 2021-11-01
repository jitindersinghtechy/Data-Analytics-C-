using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using S2TAnalytics.DAL.Models;
using S2TAnalytics.Common.Helper;

namespace S2TAnalytics.DAL.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        IMongoCollection<T> GetCollection();
        T GetById(string id);
        List<T> GetByFilters(FilterDefinition<T> filterDefinition);
        List<T> GetPagedRecords(PageRecordModel pageRecordModel, SortDefinition<T> sortDefinition = null,  FilterDefinition<T> filterDefinition = null, ProjectionDefinition<T> projection = null);
        int GetTotalRecordsCount(FilterDefinition<T> filterDefinition = null);
        //List<T> GetPagedRecords(int pageSize, int pageNumber, FilterDefinition<T> filterDefinition = null);
        void Add(T entity);
        void AddMultiple(List<T> entity);
        Task<UpdateResult> UpdateOne(string id, UpdateDefinition<T> update);
        void Update(T item);
        List<T> GetPagedRecordsLinq(PageRecordModel pageRecordModel, Expression<Func<T, bool>> whereCondition, Expression<Func<T, string>> orderBy, string sortDirection = "asc");
        void Delete(Expression<Func<T, bool>> filter);
    }
}
