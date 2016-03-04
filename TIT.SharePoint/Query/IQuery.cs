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
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limite, IQueryable<TModel> queryable);
        Func<IEnumerable<TModel>> Load<TModel>(string listName, int limite, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
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
        void UpdateItem<TContentType>(string listName, ListItemData<TContentType> itemData);
        int InsertItem<TContentType>(string listName, ListItemData<TContentType> itemData);
        int InsertItemByContentType<TContentType>(string listName, ListItemData<TContentType> itemData);
        string ContextUrl { get; }
        IList<ListQuery> LoadedList { get; set; }
        ListQuery GetListQueryByTitle(string title);
        ImageUploaded UploadImage<TContentType>(string listName, string fileName, Stream stream, ListItemData<TContentType> itemData = null, int maxLength = 4000000);
        SPContextWraper GetSPContextWraper();
        object Context { get; }
        //ImageUploaded UploadImage(string listName, string fileName, Stream stream);
        bool IsClient { get; }
        int CountListItems(string listName);
    }

    public class SPContextWraper
    {
        public string ListItemId { get; set; }
        public Dictionary<string, object> Item { get; set; }
        public string List { get; set; }
        public string ListId { get; set; }
    }
}


