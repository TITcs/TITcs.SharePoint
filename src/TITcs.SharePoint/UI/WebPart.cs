using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using TITcs.SharePoint.Log;
using TITcs.SharePoint.UI.WebParts;

namespace TITcs.SharePoint.UI
{
    public delegate void WebPartUserControlEventHandler<in TUserControl>(object sender, TUserControl webPartUserControl) where TUserControl : Control;

    public abstract class WebPart<TUserControl> : WebPart where TUserControl : Control
    {
        public event WebPartUserControlEventHandler<TUserControl> WebPartUserControlLoaded;

        protected WebPart()
        {
            ChromeType = PartChromeType.None;
        }

        public List<IEditorPart> ToolParts { get; set; }

        /// <summary>
        /// Retorna o usuário corrente autenticado no sistema
        /// </summary>
        public IPrincipal CurrentUser
        {
            get
            {
                return this.Context.User;
            }
        }

        public TUserControl WebPartUserControl { get; private set; }


        protected override void OnInit(EventArgs e)
        {
            WebPartUserControl = (TUserControl)Page.LoadControl(typeof(TUserControl), null);

            if (WebPartUserControlLoaded != null)
            {
                WebPartUserControlLoaded.Invoke(null, WebPartUserControl);
            }

            base.OnInit(e);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                FeatureStaplerAttribute.HasFeature(GetType());
                Controls.Add(WebPartUserControl);
            }
            catch (Exception e)
            {
                Logger.Unexpected("WebPart<TUserControl>.CreateChildControls", e.Message);
            }

        }

    }
}
