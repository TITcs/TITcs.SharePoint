using System.Threading.Tasks;

namespace TITcs.SharePoint.Query.Extensions
{
    public static class IQueryExtensions
    {public static Task ExecuteAsync(this IQuery query)
        {
            return Task.Factory.StartNew(query.Execute);
        }
    }
}
