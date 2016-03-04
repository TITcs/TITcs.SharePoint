using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using TITcs.SharePoint.Query;
using TITcs.SharePoint.UI.WebParts;

namespace TITcs.SharePoint.UI
{
    public delegate void WebPartUserControlEventHandler<in TWebPartUserControl>(object sender, TWebPartUserControl webPartUserControl) where TWebPartUserControl : Control;

    public abstract class UIWebPart<TWebPartUserControl> : WebPart where TWebPartUserControl : Control
    {
        public event WebPartUserControlEventHandler<TWebPartUserControl> WebPartUserControlLoaded;

        public UIWebPart()
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

        public TWebPartUserControl WebPartUserControl { get; private set; }
        private bool _hasException = false;
        private UIException _uiException = null;

        protected override void OnInit(EventArgs e)
        {
            WebPartUserControl = (TWebPartUserControl)Page.LoadControl(typeof(TWebPartUserControl), null);

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
            catch (UIException uiException)
            {
                _hasException = true;
                _uiException = uiException;
            }
            catch (Exception exception)
            {
                _hasException = true;
                _uiException = new UIException(exception);
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (_hasException)
            {
                _uiException.Render(writer);
            }
            else
            {
                base.Render(writer);
            }
        }
    }
}
