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

    }
}
