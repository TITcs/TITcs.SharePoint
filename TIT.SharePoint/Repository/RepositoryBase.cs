using System;
using System.Collections.Generic;
using System.Linq;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Log;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Repository
{
    public abstract class RepositoryBase : IDisposable
    {
        public IQuery Query { get; set; }
        public IListQuery ListQuery { get; set; }
        public User User { get; set; }

        protected RepositoryBase()
        {
        }

        protected RepositoryBase(IListQuery listQuery)
        {
            ListQuery = listQuery;
        }

        public Func<IEnumerable<TModel>> Load<TModel>(IQueryable<TModel> queryable) where TModel : ContentType
        {
            return Query.Load(ListQuery.Title, queryable);
        }

        public void Dispose()
        {
            if (Query != null)
            {
                Query.Dispose();
            }
        }

        #region Cache
        protected T CacheResult<T>(Func<T> method, string cacheKey, string cacheSubKey, params object[] args)
        {
            string key = cacheKey;
            string subKey = string.Format(cacheSubKey, args);

            if (!Cache.Contains(key, subKey))
            {
                T result = Call(method);

                if (result == null)
                    return default(T);

                Cache.Insert(key, subKey, result);

                return result;
            }

            return Cache.Get<T>(key, subKey);
        }

        protected void InvalidateCache(string cacheKey)
        {
            InvalidateCache(cacheKey, null);
        }

        protected void InvalidateCache(string cacheKey, string cacheSubKey, params object[] args)
        {
            string key = cacheKey;

            if (cacheSubKey == null)
                Cache.Remove(key);
            else
                Cache.Remove(key, string.Format(cacheSubKey, args));
        }
        #endregion Cache

        protected TResult Call<TResult>(Func<TResult> method)
        {
            try
            {
                return method();
            }
            catch (Exception exception)
            {
                Logger.Unexpected("RepositoryBase.Call", exception.Message);
                throw;
            }
        }

        protected void Exec(Action method)
        {
            try
            {
                method();
            }
            catch (Exception exception)
            {
                Logger.Unexpected("RepositoryBase.Exec", exception.Message);
                throw;
            }
        }
    }
}
