using System.Collections;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;

namespace TITcs.SharePoint.Utils
{
    public class ContentUtils
    {
        private const string MasterPageUrl = "/_catalogs/masterpage/seattle.master";
        private const string KeyCustomMasterpage = "CUSTOM_MASTER_PAGE";
        private const string KeyCustomWelcomePage = "CUSTOM_WELCOME_PAGE";

        public static void SetMasterPage(SPSite site, string customMasterPageUrl)
        {
            if (site != null)
            {
                var properties = site.RootWeb.Properties;

                if (!properties.ContainsKey(KeyCustomMasterpage))
                {
                    properties[KeyCustomMasterpage] = site.RootWeb.CustomMasterUrl;
                    properties.Update();
                }

                site.RootWeb.MasterUrl = MasterPageUrl;
                site.RootWeb.CustomMasterUrl = customMasterPageUrl;
                site.RootWeb.Update();
            }
        }

        public static void RemoveMasterPage(SPSite site)
        {
            if (site != null)
            {
                site.RootWeb.MasterUrl = MasterPageUrl;
                site.RootWeb.CustomMasterUrl = site.RootWeb.Properties[KeyCustomMasterpage];
                site.RootWeb.Update();
            }
        }

        public static void SetWelcomePage(SPSite site, string pageUrl)
        {
            if (site != null)
            {
                SPFolder folder = site.RootWeb.RootFolder;
                var properties = site.RootWeb.Properties;

                if (properties.ContainsKey(KeyCustomWelcomePage))
                {
                    properties[KeyCustomWelcomePage] = folder.WelcomePage;
                    properties.Update();
                }
                
                folder.WelcomePage = pageUrl;
                folder.Update();
            }
        }

        public static void RemoveWelcomePage(SPSite site)
        {
            if (site != null)
            {
                SPFolder folder = site.RootWeb.RootFolder;
                folder.WelcomePage = site.RootWeb.Properties[KeyCustomWelcomePage];
                folder.Update();
            }

        }

        public static void AllowPageLayouts(SPSite site, bool allow, params string[] pageLayoutsStr)
        {
            var publishingWeb = PublishingWeb.GetPublishingWeb(site.RootWeb);
            if (publishingWeb != null)
            {

                if (allow)
                {

                    var pageLayoutList = new ArrayList();
                    var pSite = publishingWeb.Web.Site;
                    if (pSite != null)
                    {
                        var publishingSite = new PublishingSite(pSite);
                        var pageLayoutsCollection = publishingSite.GetPageLayouts(true);
                        foreach (var pl in pageLayoutsCollection)
                        {
                            if (
                                pageLayoutsStr.ToList()
                                    .Any(p => pl.ServerRelativeUrl.ToLower().Contains(p.ToLower())))
                            {
                                pageLayoutList.Add(pl);
                            }
                        }
                        var newPls = (PageLayout[]) pageLayoutList.ToArray(typeof (PageLayout));
                        publishingWeb.SetAvailablePageLayouts(newPls, false);
                        publishingWeb.Update();
                    }

                }
                else
                {

                    var pageLayouts = publishingWeb.GetAvailablePageLayouts();
                    var pageLayoutList = new ArrayList();
                    pageLayoutList.AddRange(pageLayouts);
                    var pSite = publishingWeb.Web.Site;
                    if (pSite != null)
                    {
                        var publishingSite = new PublishingSite(pSite);
                        var pageLayoutsCollection = publishingSite.GetPageLayouts(true);
                        foreach (PageLayout pl in pageLayoutsCollection)
                        {
                            if (
                                pageLayoutsStr.ToList()
                                    .Any(p => pl.ServerRelativeUrl.ToLower().Contains(p.ToLower())))
                            {
                                if (pageLayoutList.Contains(pl))
                                    pageLayoutList.Remove(pl);
                            }
                        }

                        var newPls = (PageLayout[]) pageLayoutList.ToArray(typeof (PageLayout));
                        publishingWeb.SetAvailablePageLayouts(newPls, false);
                        publishingWeb.Update();
                    }

                }
            }
        }
    }
}
