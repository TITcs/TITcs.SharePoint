using System.Collections.Generic;
using TITcs.SharePoint.UI.WebParts;

namespace TITcs.SharePoint.UI
{
    public abstract class UISharePointControlEditor<TLogic> : UISharePointControl<TLogic>, IUIWebPartGroup where TLogic : class
    {
        public KeyValuePair<string, string>? Group
        {
            get
            {
                var o = ViewState["1"];
                if (o != null)
                    return (KeyValuePair<string, string>)o;

                return null;
            }
            set
            {
                ViewState["1"] = value;
            }
        }

        public Dictionary<string, string> DataGroups { get; set; }

        public abstract void BindDataGroups();


        public string ListUrl
        {
            get
            {
                var o = ViewState["2"];
                if (o != null)
                    return (string)o;

                return null;
            }
            set
            {
                ViewState["2"] = value;
            }
        }
    }
}
