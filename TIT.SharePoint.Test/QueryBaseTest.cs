using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Test
{
    [TestFixture]
    public class QueryBaseTest
    {
        [Test]
        public void Fields()
        {
            var fields = new DemoQuery().Load<Item>("Demo", 0, "", i => i.ID, i => i.Title);
        }
    }

    public class DemoQuery : QueryBase
    {
        public override ItemUploaded UploadImage<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null,
            int maxLength = 4000000)
        {
            throw new NotImplementedException();
        }

        public override int CountItems(string listTitle)
        {
            throw new NotImplementedException();
        }

        public override ItemUploaded UploadDocument<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null,
            int maxLength = 4000000)
        {
            throw new NotImplementedException();
        }

        public override ContextWraper GetContextWraper()
        {
            throw new NotImplementedException();
        }

        public override object Context { get; }
        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, string camlQuery, params Expression<Func<TModel, object>>[] fields)
        {
            var siteColumns = ToArray(fields);

            return null;
        }

        public override int InsertItemByContentType<TContentType>(string listName, Fields<TContentType> fields)
        {
            throw new NotImplementedException();
        }

        public override string ContextUrl { get; }

        public override void UpdateItem<TContentType>(string listName, Fields<TContentType> item)
        {
            throw new NotImplementedException();
        }

        public override UserGroup[] GetGroups()
        {
            throw new NotImplementedException();
        }

        public override UserGroup GetGroupByName(string name)
        {
            throw new NotImplementedException();
        }

        public override User[] GetUsers()
        {
            throw new NotImplementedException();
        }

        public override User[] GetUsersByGroup(string groupName)
        {
            throw new NotImplementedException();
        }

        public override User GetUser(string login)
        {
            throw new NotImplementedException();
        }

        public override User GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public override User CurrentUser()
        {
            throw new NotImplementedException();
        }

        public override int InsertItem<TContentType>(string listName, Fields<TContentType> item)
        {
            throw new NotImplementedException();
        }

        public override void DeleteItem(string listName, int id)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string[]> LoadFieldValues(params string[] fieldNames)
        {
            throw new NotImplementedException();
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable)
        {
            throw new NotImplementedException();
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listTitle, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy)
        {
            throw new NotImplementedException();
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true)
        {
            throw new NotImplementedException();
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable)
        {
            throw new NotImplementedException();
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true)
        {
            throw new NotImplementedException();
        }
    }
}
