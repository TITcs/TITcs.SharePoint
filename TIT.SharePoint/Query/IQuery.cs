using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Portal.WebControls.WSRPWebService;
using TITcs.SharePoint.Query.LinqToCaml;

namespace TITcs.SharePoint.Query
{
    public interface IQuery : IDisposable
    {
        //Func<IEnumerable<TModel>> Load<TModel>(string listName) where TModel : class;
        //Func<IEnumerable<TModel>> Load<TModel>(string listName, params Expression<Func<TModel, object>>[] retrievals);
        //Func<IEnumerable<TModel>> LoadWithCamlQuery<TModel>(string listName, string camlQuery, params Expression<Func<TModel, object>>[] retrievals);
        //Func<IEnumerable<TModel>> Load<TModel>(string listName, Expression<Func<TModel, bool>> predicate) where TModel : class;
        //Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, Expression<Func<TModel, bool>> predicate) where TModel : class;
        //Func<IEnumerable<TModel>> Load<TModel>(string listName, Expression<Func<TModel, bool>> predicate, params Expression<Func<TModel, object>>[] retrievals);
        //Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, params Expression<Func<TModel, object>>[] retrievals);
        //Func<IEnumerable<TModel>> LoadWithCamlQuery<TModel>(string listName, string camlQuery, int limit, params Expression<Func<TModel, object>>[] retrievals);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, string camlQuery, params string[] fields);
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
        //ItemUploaded UploadImage(string listName, string fileName, Stream stream);
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


