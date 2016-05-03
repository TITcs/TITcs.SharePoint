using TITcs.SharePoint.Repository;

namespace TITcs.SharePoint.Query
{
    public class ListQuery : IListQuery
    {
        public string Title { get; }
        public string Url { get; }

        public ListQuery(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }
}
