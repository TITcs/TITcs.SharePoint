using System;
using TITcs.SharePoint.Log;

namespace TITcs.SharePoint.Core
{
    public abstract class CacheService
    {
        protected TResult Call<TResult>(Func<TResult> method)
        {
            try
            {
                return method();
            }
            catch (Exception exception)
            {
                Logger.Unexpected("ServiceCache.Call", exception.Message);
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
                Logger.Unexpected("ServiceCache.Exec", exception.Message);
                throw;
            }
        }


        #region Cache

        protected T CacheResult<T>(Func<T> method, Type cacheKey, string cacheSubKey, params object[] args)
        {
            string key = cacheKey.ToString();
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

        protected void InvalidateCache(Type cacheKey)
        {
            InvalidateCache(cacheKey, null);
        }

        protected void InvalidateCache(Type cacheKey, string cacheSubKey, params object[] args)
        {
            string key = cacheKey.ToString();

            if (cacheSubKey == null)
                Cache.Remove(key);
            else
                Cache.Remove(key, string.Format(cacheSubKey, args));
        }

        #endregion Cache
    }
}
