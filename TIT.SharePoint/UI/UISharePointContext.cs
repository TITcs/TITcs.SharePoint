
using System.Web;
using Microsoft.SharePoint;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.UI
{
    public class UISharePointContext<TUILogic>
    {
        public TUILogic Logic { get; set; }
        private HttpContext Context
        {
            get;
            set;
        }
        public HttpRequest Request
        {
            get
            {
                return Context.Request;
            }
        }

        public SPContext SPContext
        {
            get { return SPContext.Current; }
        }

        public UISharePointContext(HttpContext httpContext)
        {
            Context = httpContext;
        }

        public SPContextWraper SPContextWraper { get; set; }
        
    }
}
