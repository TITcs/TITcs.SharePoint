using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace TITcs.SharePoint.UI.WebParts
{
    [Serializable]
    public class UIWebPartGroup<TUserControl> : UIWebPart<TUserControl>, IWebEditable where TUserControl : Control, IUIWebPartGroup
    {
        public UIWebPartGroup()
        {
            WebPartUserControlLoaded += OnWebPartUserControlLoaded;
        }


        private void OnWebPartUserControlLoaded(object sender, TUserControl webPartUserControl)
        {
            if (Group != null)
            {

                var parts = Group.Split('$');

                webPartUserControl.Group = new KeyValuePair<string, string>(parts[0], parts[1]);
            }
        }

        //protected override void CreateChildControls()
        //{
        //    base.CreateChildControls();

        //    var lbl = new Label();

        //    if (Grupo != null)
        //        lbl.Text = "Grupo selecionado: " + Grupo;

        //    base.Controls.Add(lbl);
        //}
        
        [Personalizable(PersonalizationScope.Shared)]
        public string Group { get; set; }


        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            var editors = new List<EditorPart>();

            //Força a inicialização da Logic para garantir que os grupos da lista sejam populados
            WebPartUserControl.InitLogic();
            //Preenche a propriedade Data com os grupos da lista
            WebPartUserControl.BindDataGroups();

            var editorPart = new DropDownListEditorPart(WebPartUserControl.DataGroups);

            editorPart.CreateLinkItem("Ir para a Lista", WebPartUserControl.ListUrl);

            editorPart.OnApplyChanges += (sender, args) =>
            {
                Group = string.Format("{0}${1}", args.Value, args.Text);

                OnInit(EventArgs.Empty);

                WebPartUserControl.Group = new KeyValuePair<string, string>(args.Value, args.Text);
            };

            if (!string.IsNullOrEmpty(Group))
            {
                editorPart.Value = Group.Split('$')[0];
            }

            editors.Add(editorPart);

            return new EditorPartCollection(editors);
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }
    }
}
