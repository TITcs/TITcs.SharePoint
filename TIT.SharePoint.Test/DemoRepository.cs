using System.Collections.Generic;
using System.Linq;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Query;
using TITcs.SharePoint.Repository;

namespace TITcs.SharePoint.Test
{
    public class DemoRepository : RepositorySite
    {
        public DemoRepository()
            : base(new ListQuery("Demo", @"/Lists/Demo/AllItems.aspx"))
        {

        }
        public ICollection<Item> GetAll()
        {
            using (Query)
            {
                var result = Query.Load(ListQuery.Title, GetQuery());

                Query.Execute();

                return result().ToList();
            }
        }

        private IQueryable<Item> GetQuery()
        {
            var query = Query.AsQueryable<Item>();

            return query.Select(i => new Item()
            {
                ID = i.ID,
                Title = i.Title
            });
        }
    }
}
