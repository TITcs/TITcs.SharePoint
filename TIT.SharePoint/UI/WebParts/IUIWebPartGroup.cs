using System.Collections.Generic;

namespace TITcs.SharePoint.UI.WebParts
{
    public interface IUIWebPartGroup
    {
        void InitLogic();
        KeyValuePair<string, string>? Group { get; set; }
        Dictionary<string, string> DataGroups { get; set; }
        void BindDataGroups();
        string ListUrl { get; set; }
    }
}
