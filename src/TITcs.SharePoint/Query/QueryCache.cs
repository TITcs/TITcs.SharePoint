using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TITcs.SharePoint.Query
{
    public class QueryCache:IQueryCache
    {
        private ObjectCache _objectCache = MemoryCache.Default;

        
        public void Add(object data)
        {
            var cachePolicy = new CacheItemPolicy();
        }

        public void Add(object data, TimeSpan expire)
        {
            throw new NotImplementedException();
        }

        public TModel Get<TModel>(int code)
        {
            throw new NotImplementedException();
        }
    }

    public interface IQueryCache
    {
        void Add(object data);
        void Add(object data, TimeSpan expire);
        TModel Get<TModel>(int code);
    }

    public class QueryMonitor : ChangeMonitor
    {
        private readonly Timer _timer = null;
        private readonly double _timeSpan = 30*(60*1000);
        private int _queryId = 0;

        public QueryMonitor(int queryId)
        {
            _queryId = queryId;
            _timer = new Timer(_timeSpan);
            _timer.Elapsed += QueryTimerElapsed;
            _timer.Start();
        }

        public QueryMonitor(int queryId,int minutes)
        {
            _queryId = queryId;
            var timeSpan = minutes*(60*1000);
            _timer = new Timer(timeSpan);
            _timer.Elapsed += QueryTimerElapsed;
            _timer.Start();
        }

        void QueryTimerElapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }
        protected override void Dispose(bool disposing)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public override string UniqueId
        {
            get { return _queryId.ToString(); }
        }
    }
}
