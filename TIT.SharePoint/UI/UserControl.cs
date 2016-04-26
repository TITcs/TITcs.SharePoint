using System;
using System.Text;
using System.Web.UI.WebControls;
using TITcs.SharePoint.Log;

namespace TITcs.SharePoint.UI
{
    public abstract class UserControl : System.Web.UI.UserControl
    {
        private event EventHandler<EventArgs> LoadUserControl;

        public abstract void Control_Load(object sender, EventArgs e);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += (sender, args) =>
            {
                try
                {
                    if (LoadUserControl != null)
                    {
                        LoadUserControl(sender, e);
                    }
                }
                catch (Exception exception)
                {
                    Logger.Unexpected("UserControl.OnInit", exception.Message);

                    var html = new Literal();

                    html.Text = GetHtml(exception);

                    Controls.Clear();
                    Controls.Add(html);

                }
            };

            LoadUserControl += Control_Load;
        }

        private string GetHtml(Exception e)
        {
            var html = new StringBuilder();

            html.Append("<div style=\"text-align:left;border:solid 1px #666;width:auto;padding:8px;margin:8px;\">");
            html.Append("    <h2 style=\"margin-bottom:4px;color:#535353\">Detalhe do Erro</h2>");
            html.Append("    <h4 style=\"color:#666\">Exception:</h4>");
            html.Append("    <p style=\"margin-bottom:8px;color:#8e8b8b;font-size:12px\">");
            html.Append(e.Message);
            html.Append("    </p>");

            if (e.InnerException != null)
            {
                html.Append("    <h4 style=\"color:#666\">Inner Exception:</h4>");
                html.Append("    <p style=\"margin-bottom:8px;color:#8e8b8b;font-size:12px\">");
                html.Append(e.InnerException);
                html.Append("    </p>");
            }

            html.Append("    <h4 style=\"color:#666\">StackTrace:</h4>");
            html.Append("    <p style=\"margin-bottom:8px;color:#8e8b8b;font-size:12px\">");
            html.Append(e.StackTrace);
            html.Append("    </p>");
            html.Append("</div>");

            return html.ToString();
        }

    }
}
