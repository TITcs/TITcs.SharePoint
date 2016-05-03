using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace TITcs.SharePoint.Query
{
    public interface IQuery : IDisposable
    {
        #region Load
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, string camlQuery, params Expression<Func<TModel, object>>[] fields);
        #endregion Load

        Dictionary<string, string[]> LoadFieldValues(params string[] fieldName);
        Expression<Func<TModel, bool>> AsPredicate<TModel>(bool evaluate);
        IQueryable<TModel> AsQueryable<TModel>();
        void Execute();
        UserGroup[] GetGroups();
        UserGroup GetGroupByName(string name);
        User[] GetUsers();
        User[] GetUsersByGroup(string groupName);
        User GetUser(string login);
        User GetUserById(string id);
        User CurrentUser();
        void DeleteItem(string listName, int id);
        void UpdateItem<TContentType>(string listName, Fields<TContentType> fields);
        int InsertItem<TContentType>(string listName, Fields<TContentType> fields);
        int InsertItemByContentType<TContentType>(string listName, Fields<TContentType> fields);
        string ContextUrl { get; }
        IList<ListQuery> LoadedList { get; set; }
        ListQuery GetListQueryByTitle(string title);
        ItemUploaded UploadImage<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null, int maxLength = 4000000);
        ItemUploaded UploadDocument<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null, int maxLength = 4000000);
        ContextWraper GetContextWraper();
        object Context { get; }
        bool IsClient { get; }
        int CountItems(string listTitle);
    }

    public class ContextWraper
    {
        public string ListItemId { get; set; }
        public Dictionary<string, object> Item { get; set; }
        public string List { get; set; }
        public string ListId { get; set; }
    }
}


