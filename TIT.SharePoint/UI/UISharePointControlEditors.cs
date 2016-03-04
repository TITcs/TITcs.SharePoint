using System;
using System.Collections.Generic;
using TITcs.SharePoint.UI.WebParts;

namespace TITcs.SharePoint.UI
{
    public abstract class UISharePointControlEditors<TLogic> : UISharePointControl<TLogic>, IUIWebPartGroups where TLogic : class
    {
        protected UISharePointControlEditors()
        {
            Groups = new System.Collections.Generic.List<KeyValuePair<string, string>?>();
            DataGroups = new System.Collections.Generic.List<Dictionary<string, string>>();
            ListUrls = new System.Collections.Generic.List<string>();
        }


        public IList<KeyValuePair<string, string>?> Groups
        {
            get
            {
                var o = ViewState["1"];
                if (o != null)
                    return (IList<KeyValuePair<string, string>?>)o;

                return null;
            }
            set
            {
                ViewState["1"] = value;
            }
        }

        public IList<Dictionary<string, string>> DataGroups { get; set; }

        public abstract void BindDataGroups();


        public IList<string> ListUrls
        {
            get
            {
                var o = ViewState["2"];
                if (o != null)
                    return (IList<string>)o;

                return null;
            }
            set
            {
                ViewState["2"] = value;
            }
        }
    }
}
