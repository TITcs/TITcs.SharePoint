using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.SharePoint;

namespace TITcs.SharePoint.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureStaplerAttribute : Attribute
    {
        public string FeatureId { get; set; }

        public static void HasFeature(Type type)
        {
            var featureStapler = type.GetCustomAttribute<FeatureStaplerAttribute>();
            
            if (featureStapler == null)
                return;
            
            if(string.IsNullOrEmpty(featureStapler.FeatureId))
                return;

            var featureId = new Guid(featureStapler.FeatureId);

            using (var web = new SPSite(HttpContext.Current.Request.Url.AbsoluteUri).OpenWeb())
            {
                var hasFeature = web.Features.FirstOrDefault(i => i.DefinitionId.Equals(featureId));

                if (hasFeature == null)
                    web.Features.Add(featureId, true);
            }
       }
    }
}
