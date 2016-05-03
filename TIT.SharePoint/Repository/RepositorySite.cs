using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Repository
{
    public abstract class RepositorySite : RepositoryBase
    {
        protected RepositorySite()
        {
            Query = ContextFactory.GetContextSite();
        }
        protected RepositorySite(IListQuery listQuery) :
            base(listQuery)
        {
            Query = ContextFactory.GetContextSite();

            User = Query.CurrentUser();
        }
    }
}
