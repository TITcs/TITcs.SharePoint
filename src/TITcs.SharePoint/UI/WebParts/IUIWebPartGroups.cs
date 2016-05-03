using System.Collections.Generic;

namespace TITcs.SharePoint.UI.WebParts
{
    public interface IUIWebPartGroups
    {
        void InitLogic();
        IList<KeyValuePair<string, string>?> Groups { get; set; }
        IList<Dictionary<string, string>> DataGroups { get; set; }
        void BindDataGroups();
        IList<string> ListUrls { get; set; }
    }
}
