using System;
using System.Web.SessionState;
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
            runCodeInListInstance(web, listTitle, (list) =>
            {
                list.BreakRoleInheritance(true, false);
                list.AllowEveryoneViewItems = true;
                list.AnonymousPermMask64 = basePermissions;

                list.Update();
            });
            
        }

        /// <summary>
        /// Disable Anonymous Access list
        /// </summary>
        /// <param name="web">Context</param>
        /// <param name="listTitle">List title</param>
        public static void DisableAccessAnonymous(SPWeb web, string listTitle)
        {
            runCodeInListInstance(web, listTitle, (list) =>
            {
                list.ResetRoleInheritance();
                list.Update();
            });

        }

        public static void AllowDuplicateValues(SPWeb web, string listTitle, string fieldName)
        {
            runCodeInListInstance(web, listTitle, (list) =>
            {
                if (list.Fields.ContainsField(fieldName))
                {
                    SPField field = list.Fields[fieldName];

                    field.Indexed = true;
                    //field.AllowDuplicateValues = false;
                    field.EnforceUniqueValues = true;

                    field.Update();
                    list.Update();
                }
            });
        }

        private static void runCodeInListInstance(SPWeb web, string listTitle, Action<SPList> action)
        {
            var list = web.Lists.TryGetList(listTitle);

            if (list != null)
            {
                var allowSafeUpdates = web.AllowUnsafeUpdates;
                web.AllowUnsafeUpdates = true;

                action(list);

                web.AllowUnsafeUpdates = allowSafeUpdates;
            }
        }
    }
}
