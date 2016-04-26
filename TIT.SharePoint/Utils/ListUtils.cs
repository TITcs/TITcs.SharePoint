using Microsoft.SharePoint;

namespace TITcs.SharePoint.Utils
{
    public class ListUtils
    {
        /// <summary>
        /// Enable Anonymous Access list
        /// </summary>
        /// <param name="web">Context</param>
        /// <param name="listTitle">List title</param>
        public static void EnableAccessAnonymous(SPWeb web, string listTitle)
        {
            EnableAccessAnonymous(web, listTitle);
        }

        public static void EnableAccessAnonymous(SPWeb web, string listTitle, SPBasePermissions basePermissions = SPBasePermissions.ViewPages |
                    SPBasePermissions.OpenItems | SPBasePermissions.ViewVersions |
                    SPBasePermissions.Open | SPBasePermissions.UseClientIntegration |
                    SPBasePermissions.ViewFormPages | SPBasePermissions.ViewListItems)
        {
            var list = web.Lists.TryGetList(listTitle);

            if (list != null)
            {
                var allowSafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;

                list.BreakRoleInheritance(true, false);
                list.AllowEveryoneViewItems = true;
                list.AnonymousPermMask64 = basePermissions;

                list.Update();

                web.AllowUnsafeUpdates = allowSafeUpdates;
            }
        }

        /// <summary>
        /// Disable Anonymous Access list
        /// </summary>
        /// <param name="web">Context</param>
        /// <param name="listTitle">List title</param>
        public static void DisableAccessAnonymous(SPWeb web, string listTitle)
        {
            var list = web.Lists.TryGetList(listTitle);

            if (list != null)
            {
                var allowSafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;

                list.ResetRoleInheritance();
                list.Update();

                web.AllowUnsafeUpdates = allowSafeUpdates;
            }
        }
    }
}
