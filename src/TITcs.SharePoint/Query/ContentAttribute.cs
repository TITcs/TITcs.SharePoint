using System;

namespace TITcs.SharePoint.Query
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContentAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
