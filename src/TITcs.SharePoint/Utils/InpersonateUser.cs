using System;
using Microsoft.SharePoint;

namespace TITcs.SharePoint.Utils
{
    /// <summary>
    /// Excute code with inpsersonate user
    /// </summary>
    public class InpersonateUser
    {
        public static void RunWithAccountSystem(SPSite currentSite, Action<SPWeb> action)
        {
            SPUserToken systoken = currentSite.SystemAccount.UserToken;
            using (SPSite site = new SPSite(currentSite.Url, systoken))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    action(web);
                }
            }
        }

        public static void RunWithApplicationPool(Action action)
        {
            var impersonationContext = System.Security.Principal.WindowsIdentity.GetCurrent().Impersonate();

            action();

            impersonationContext.Undo();
            
        }
    }
}
