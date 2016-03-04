using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace TITcs.SharePoint.UI.WebParts
{
    [Serializable]
    public class UIWebPartGroups<TUserControl> : UIWebPart<TUserControl>, IWebEditable where TUserControl : Control, IUIWebPartGroups
    {
        public UIWebPartGroups()
        {
            WebPartUserControlLoaded += OnWebPartUserControlLoaded;
        }


        private void OnWebPartUserControlLoaded(object sender, TUserControl webPartUserControl)
        {
            for (var i = 0; i < WebPartUserControl.Groups.Count; i++)
            {
                if (Groups[i] != null)
                {
                    var parts = Groups[i].Split('$');

                    webPartUserControl.Groups[i] = new KeyValuePair<string, string>(parts[0], parts[1]);
                }
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

        [Personalizable(true)]
        public string[] Groups { get; set; }


        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            var editors = new List<EditorPart>();

            //Força a inicialização da Logic para garantir que os grupos da lista sejam populados
            WebPartUserControl.InitLogic();
            //Preenche a propriedade Data com os grupos da lista
            WebPartUserControl.BindDataGroups();


            int totalGroups = WebPartUserControl.DataGroups.Count;

            Groups = new string[totalGroups];


            for (var i = 0; i < totalGroups; i++)
            {
                WebPartUserControl.Groups.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
                
                editors.Add(CreateEditorPart(i));
            }

            return new EditorPartCollection(editors);
        }

        private DropDownListEditorPart CreateEditorPart(int index)
        {
            var editorPart = new DropDownListEditorPart(WebPartUserControl.DataGroups[index]);

            editorPart.CreateLinkItem("Ir para a Lista", WebPartUserControl.ListUrls[index]);

            editorPart.OnApplyChanges += (sender, args) =>
            {
                Groups[index] = string.Format("{0}${1}", args.Value, args.Text);

                OnInit(EventArgs.Empty);

                WebPartUserControl.Groups[index] = new KeyValuePair<string, string>(args.Value, args.Text);
            };

            if (!string.IsNullOrEmpty(Groups[index]))
            {
                editorPart.Value = Groups[index].Split('$')[0];
            }

            return editorPart;
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }
    }
}
