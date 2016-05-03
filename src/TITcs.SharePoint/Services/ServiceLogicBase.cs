using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.JSGrid;
using TITcs.SharePoint.Query;
using TITcs.SharePoint.UI;

namespace TITcs.SharePoint.Services
{
    //public abstract class ServiceLogicBase<TSharePointLogic> : ServiceBase
    //    where TSharePointLogic : class
    //{
    //    public TSharePointLogic Logic { get; set; }
    //    public object Execute(ServiceContext context)
    //    {
    //        try
    //        {
    //            Logic = UIFactory.UIModel<TSharePointLogic>();
    //            FillLogicMetadata(Logic, context.Model);
    //            var modelObject = (ModelObject) context.Model;
    //            if (modelObject.GetDynamicMemberNames().Contains("Method"))
    //            {
    //                return ExecuteAction(Logic, context.Model);
    //            }

    //            ((IInternalUILogic) Logic).InternalUILogic();

    //            var result = Logic.GetType().GetProperty("Model").GetValue(Logic, null);

    //            return result;
    //        }
    //        catch (Exception exception)
    //        {
    //            throw new UIException(exception);
    //        }
            
    //    }

    //    private void FillLogicMetadata(object logic, ModelObject model)
    //    {
    //        var propertyDictionary = logic.GetType().GetProperties().ToDictionary(i => i.Name.ToLower(), i => i);
    //        foreach (var dynamicMemberName in model.GetDynamicMemberNames())
    //        {
    //            var propName = dynamicMemberName.ToLower();
    //            if (propertyDictionary.ContainsKey(propName))
    //            {
    //                propertyDictionary[propName].SetValue(logic, model.ReadValue(propName));
    //            }
    //        }
    //    }

    //    private object ExecuteAction(object logic, ModelObject model)
    //    {
    //        if (!model.GetDynamicMemberNames().Contains("Method"))
    //            return null;
    //        var methodName = model.ReadValue("Method").ToString();

    //        var propertyDictionary = logic.GetType().GetMethods().ToDictionary(i => i.Name.ToLower(), i => i);
    //        if (!propertyDictionary.ContainsKey(methodName))
    //            return null;
    //        var methodInfo = propertyDictionary[methodName];
    //        var parameters = methodInfo.GetParameters().Select(i => model.ReadValue(i.Name)).ToArray();
    //        return methodInfo.Invoke(logic, parameters);

    //    }

    //}
}
