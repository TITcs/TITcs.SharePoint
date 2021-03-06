﻿using System.Reflection;
using Microsoft.SharePoint;
using TITcs.SharePoint.Log;

namespace TITcs.SharePoint.Utils
{
    public static class EventReceiverUtils
    {
        public static void Remove(SPWeb web, string listTitle)
        {
            Remove(web, listTitle, Assembly.GetExecutingAssembly().FullName);
        }

        public static void Remove(SPWeb web, string listTitle, string assembly)
        {
            Logger.Information("EventReceiverUtils.Remove", "List: {0}, Assembly: {1}", listTitle, assembly);

            SPList list = web.Lists.TryGetList(listTitle);

            if (list != null)
            {
                for (int i = list.EventReceivers.Count - 1; i >= 0; --i)
                {
                    if (list.EventReceivers[i].Assembly == assembly)
                    {
                        list.EventReceivers[i].Delete();

                        Logger.Information("EventReceiverUtils.Remove", "EventReceiver: {0}", list.EventReceivers[i].Name);
                    }
                }
            }
        }
    }
}
