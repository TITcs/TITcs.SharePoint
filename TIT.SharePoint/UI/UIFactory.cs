using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TITcs.SharePoint.UI
{
    public static class UIFactory
    {
        //private static readonly Dictionary<Type, Type> DataUILogicTypes = null;
        static UIFactory()
        {
            
            //DataUILogicTypes = 
            //    Assembly.LoadWithPartialName("GCS.Vitrine.UI.Logic").ExportedTypes
            //    .Where(i => (i.BaseType != null) && (i.BaseType.Name.Equals("UILogic`1")))
            //    .ToDictionary(i => i.BaseType.GenericTypeArguments.FirstOrDefault(), i => i);
        }
        
        public static TSharePointLogic UIModel<TSharePointLogic>() where TSharePointLogic:class
        {
            var sharePointModelType = typeof (TSharePointLogic);

            var uiLogic = (IInternalUILogic)Activator.CreateInstance(sharePointModelType);

            return (TSharePointLogic) uiLogic;

        }
    }
}