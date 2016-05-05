using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TITcs.SharePoint.Core;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Log;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Repository
{
    public abstract class RepositoryBase : CacheService, IDisposable
    {
        private int _limit = 50;

        public IQuery Query { get; set; }
        public IListQuery ListQuery { get; set; }
        public User User { get; set; }

        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        protected RepositoryBase()
        {
        }

        protected RepositoryBase(IListQuery listQuery)
        {
            ListQuery = listQuery;
        }

        public Func<IEnumerable<TModel>> Load<TModel>(IQueryable<TModel> queryable) where TModel : ContentType
        {
            using (Query)
            {
                var result = Query.Load(ListQuery.Title, queryable);

                Query.Execute();

                return result;
            }
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

        public ICollection<Item> GetAll()
        {
            Logger.Information("RepositoryBase.GetAll");

            return Call(() =>
            {
                using (Query)
                {
                    var result = Query.Load(ListQuery.Title, Limit, getQuery());

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public Item GetById(int id)
        {
            Logger.Information("RepositoryBase.GetById", "ID: {0}", id);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.ID == id);

                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    var item = result().SingleOrDefault();

                    if (item == null)
                        throw new Exception(string.Format("Invalid item ID \"{0}\" for list \"{1}\"", id,
                            ListQuery.Title));

                    return item;
                }
            });
        }

        public ICollection<Item> GetAllByAuthor(string author)
        {
            Logger.Information("RepositoryBase.GetAllByAuthor", "Author: {0}", author);

            return Call(() =>
            {
                using (Query)
                {
                    var camlQuery = "<Eq><FieldRef Name='Author' /><Value Type='User'>" + author + "</Value></Eq>";

                    var result = Query.Load<Item>(ListQuery.Title, Limit, camlQuery, getFields());

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByAuthor(int authorId)
        {
            Logger.Information("RepositoryBase.GetAllByAuthor", "AuthorId: {0}", authorId);

            return Call(() =>
            {
                using (Query)
                {
                    var camlQuery = "<Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + authorId +
                                    "</Value></Eq>";

                    var result = Query.Load<Item>(ListQuery.Title, Limit, camlQuery, getFields());

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByUser(int userId)
        {
            Logger.Information("RepositoryBase.GetAllByUser", "UserId: {0}", userId);

            return Call(() =>
            {
                using (Query)
                {
                    var camlQuery = "<Eq><FieldRef Name='User_x0020_ID' LookupId='True' /><Value Type='User'>" + userId +
                                    "</Value></Eq>";

                    var result = Query.Load<Item>(ListQuery.Title, Limit, camlQuery, getFields());

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByCreated(DateTime created)
        {
            Logger.Information("RepositoryBase.GetAllByCreated", "Created: {0}", created);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.Created == created);
                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByGreaterThanCreated(DateTime created)
        {
            Logger.Information("RepositoryBase.GetAllByGreaterThanCreated", "Created: {0}", created);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.Created > created);
                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByGreaterThanOrEqualCreated(DateTime created)
        {
            Logger.Information("RepositoryBase.GetAllByGreaterThanOrEqualCreated", "Created: {0}", created);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.Created >= created);
                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByLessThanCreated(DateTime created)
        {
            Logger.Information("RepositoryBase.GetAllByLessThanCreated", "Created: {0}", created);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.Created < created);
                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public ICollection<Item> GetAllByLessThanOrEqualCreated(DateTime created)
        {
            Logger.Information("RepositoryBase.GetAllByLessThanOrEqualCreated", "Created: {0}", created);

            return Call(() =>
            {
                using (Query)
                {
                    var query = getQuery().Where(i => i.Created <= created);
                    var result = Query.Load(ListQuery.Title, Limit, query);

                    Query.Execute();

                    return result().ToList();
                }
            });
        }

        public virtual Item Insert(Item item)
        {
            return Call(() =>
            {
                using (Query)
                {
                    var fields = new Fields<Item>();

                    fields.Add(i => i.Title, item.Title);

                    var id = Query.InsertItem(ListQuery.Title, fields);

                    item.ID = id;

                    return item;
                }
            });
        }

        public virtual Item Update(Item item)
        {
            using (Query)
            {
                var fields = new Fields<Item>();

                fields.Add(i => i.ID, item.ID);
                fields.Add(i => i.Title, item.Title);

                Query.UpdateItem(ListQuery.Title, fields);

                return item;
            }
        }

        public virtual void Delete(int id)
        {
            Exec(() =>
            {
                using (Query)
                {
                    var item = GetById(id);

                    if (item == null)
                        throw new Exception(string.Format("Invalid item ID \"{0}\" for list \"{1}\"", id, ListQuery.Title));

                    Query.DeleteItem(ListQuery.Title, id);
                }
            });
        }

        private IQueryable<Item> getQuery()
        {
            return Query.AsQueryable<Item>().Select(i => new Item()
            {
                ID = i.ID,
                Title = i.Title,
                Created = i.Created,
                Modified = i.Modified,
                Author = i.Author,
                FileRef = i.FileRef,
                File = i.File,
                LikedBy = i.LikedBy,
                LikesCount = i.LikesCount,
                _Level = i._Level,
                ModifiedBy = i.ModifiedBy
            });
        }

        private Expression<Func<Item, object>>[] getFields()
        {
            var fields = new List<Expression<Func<Item, object>>>
            {
                i => i.ID,
                i => i.Title,
                i => i.Created,
                i => i.Modified,
                i => i.Author,
                i => i.FileRef,
                i => i.File,
                i => i.LikedBy,
                i => i.LikesCount,
                i => i._Level,
                i => i.ModifiedBy
            };

            return fields.ToArray();
        }
    }
}
