using System;
using System.Linq;
using System.Linq.Expressions;
using TITcs.SharePoint.Query.LinqProvider;
using TITcs.SharePoint.Query.LinqProvider.Parsing.Structure;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    public class CamlQueryable<TModel> : QueryableBase<TModel>
    {
        public CamlQueryable ():base(QueryParser.CreateDefault(),new CamlQueryableExecutor())
        {
            
        } 
        public CamlQueryable (IQueryParser queryParser, IQueryExecutor executor)
                : base(queryParser, executor)
        {
        }

        public CamlQueryable (IQueryProvider provider)
                : base(provider)
        {
        }

        public CamlQueryable (IQueryProvider provider, Expression expression)
                : base(provider, expression)
        {
        }

        public string Caml()
        {
            return ((CamlQueryableExecutor) ((DefaultQueryProvider) Provider).Executor).Caml();
        }

        public string[] Fields()
        {
            return ((CamlQueryableExecutor) ((DefaultQueryProvider) Provider).Executor).Fields();
        }
    }

   
    //[TestFixture]
    //public class CamlQueryableTest
    //{
    //    [Test]
    //    public void TestCaml ()
    //    {
            
    //        var queryableItem = new CamlQueryable<Item>();
    //        var query = queryableItem.Where (i => i.Id == 0 && i.Title.Equals ("Eric"));
    //        query = query.Where (i => i.StartDate >= DateTime.Now && i.EndDate <= DateTime.Now);
    //        var projection = query.Select (
    //                i => new Item()
    //                     {
    //                         Id = i.Id,
    //                         Title = i.Title
    //                     });
    //        var rs = projection.ToList();
    //    }
    //}
    


}