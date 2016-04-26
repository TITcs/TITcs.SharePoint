using System.Web;
using System.Web.Configuration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Net;
using TITcs.SharePoint.Log;

namespace TITcs.SharePoint.Query
{

    /// <summary>
    /// Nome:ContextFactory
    /// Responsabilidade: Identificar se o ambiente de execução é SharePoint ou não. Se for um ambiente com Sharepoint
    /// a factory deve retornar um objeto SSOM caso contrário CSOM
    /// </summary>
    public static class ContextFactory
    {
        private static bool _isClient;

        private const String SharePointProductsRegistryPath = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions\15.0\";
        private const String SharepointProductKey = @"SharePoint";

        /// <summary>
        /// identifica o ambiente de SharePoint através de registro
        /// </summary>
        static ContextFactory()
        {
#if(DEBUG)
            _isClient = true;
#else
            //Open the registry key in read-only mode.
            var sharepointRegistry = Registry.LocalMachine.OpenSubKey(SharePointProductsRegistryPath, false);
            if (sharepointRegistry == null)
            {
                _isClient = true;
            }
            else
            {
                _isClient = (sharepointRegistry.GetValue(SharepointProductKey) == null);    
                
            }
#endif

        }

        public static IQuery GetContext()
        {
            string rootUrl;

            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                rootUrl = string.Format("{0}://{1}", request.Url.Scheme, request.Url.Authority);

#if DEBUG
                rootUrl = WebConfigurationManager.AppSettings["SharePoint:Root"];
#endif
            }
            else
            {
                rootUrl = WebConfigurationManager.AppSettings["SharePoint:Root"];
            }

            Logger.Information("ContextFactory.GetContext", string.Format("Url: {0}", rootUrl));

            return GetContext(rootUrl);
        }
 
        public static IQuery GetContextSite()
        {
#if DEBUG

            return GetContext();

#else
            var rootUrl = WebConfigurationManager.AppSettings["SharePoint:Root"];
            var urlSite = (_isClient)
                ? rootUrl
                    : (SPContext.Current != null) 
                        ? SPContext.Current.Site.RootWeb.Url : rootUrl;

            return GetContext(urlSite);
#endif
        }

        public static IQuery GetContextWeb()
        {
 #if DEBUG
            return GetContext();

#else
            var rootUrl = WebConfigurationManager.AppSettings["SharePoint:Root"];//WebConfigurationManager.AppSettings["SharePoint:Root"];
            var urlSite = (_isClient) ? rootUrl : (SPContext.Current!=null)?SPContext.Current.Web.Url:
            rootUrl;
            return GetContext(urlSite);
#endif
        }

        private static IQuery GetContext(string url)
        {
            try
            {
                Logger.Information("ContextFactory.GetContext", string.Format("Context = Client, Url = {0}", url));

                if (_isClient)
                {
                    var context = new ClientContext(url)
                    {
                        Credentials = CredentialCache.DefaultCredentials
                    };

#if DEBUG
                    var userName = WebConfigurationManager.AppSettings["SharePoint:UserName"];
                    var passWord = WebConfigurationManager.AppSettings["SharePoint:PassWord"];
                    var netDomain = WebConfigurationManager.AppSettings["SharePoint:NetDomain"];

                    context.Credentials = new NetworkCredential(userName, passWord, netDomain);
#endif

                    return new ClientQuery(context);
                }

                var spSite = new SPSite(url);

                object obj = spSite.OpenWeb();
                return new ServerQuery(obj);


            }
            catch (Exception e)
            {
                Logger.Unexpected("ContextFactory.GetContext", e.Message);
                throw e;
            }
        }
         
    }
}
