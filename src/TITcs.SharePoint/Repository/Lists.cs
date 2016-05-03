using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Repository
{
    public partial class Lists
    {
        public static readonly IListQuery Imagens = new ListQuery("Imagens", @"/PublishingImages/Forms/Thumbnails.aspx");

        public static readonly IListQuery ImagensConjuntoSites = new ListQuery("Imagens do Conjunto de Sites", @"/SiteCollectionImages/Forms/Thumbnails.aspx");
    }
}
