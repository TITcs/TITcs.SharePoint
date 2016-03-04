using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Mobile.Controls;

namespace TITcs.SharePoint.UI.WebParts
{
    /// <summary>
    /// Classe para customizar as configurações do WebPart no SharePoint.
    /// Exemplo:
    /// 
    /// Na classe do UserControl, herdar do IEditorPart e implementar o método
    /// 
    /// =======================================================================
    /// 
    /// public EditorPartCollection GetEditorPartCollection()
    /// {
    ///     #region Configurações DropDownList
    ///
    ///     var dropDownList = new DropDownListEditorPart("Grupo", Grupos);
    ///
    ///     dropDownList.OnApplyChanges += (sender, args) =>
    ///     {
    ///         Tipo = args.Value;
    ///     };
    ///
    ///     dropDownList.Value = Tipo;
    ///
    ///     #endregion Configurações DropDownList
    ///
    ///     #region Configurações TextBox
    ///
    ///     var textBox = new TextBoxEditorPart("Grupo");
    ///
    ///     textBox.OnApplyChanges += (sender, args) =>
    ///     {
    ///         Tipo = args.Value;
    ///     };
    ///
    ///     textBox.Value = Tipo;
    ///
    ///     #endregion Configurações TextBox
    ///
    ///     var editorArray = new ArrayList { dropDownList, textBox };
    ///
    ///     return new EditorPartCollection(editorArray);
    /// }
    /// 
    /// </summary>
    public class DropDownListEditorPart : EditorPart
    {
        public event EditorPartEventHandler OnApplyChanges;
        private readonly Dictionary<string, string> _dataSource;
        private DropDownList Control { get; set; }

        public DropDownListEditorPart(Dictionary<string, string> dataSource, string title = "Parâmetros")
        {
            Title = title;
            Width = new Unit(100);
            ID = string.Format("{0}_dropDownListEditorPart", ID); 
            _dataSource = dataSource;

            if(dataSource == null)
                throw new Exception("O DataSource não pode ser nulo");
        }

        public string Value { get; set; }

        public override bool ApplyChanges()
        {
            try
            {
                EnsureChildControls();

                if (WebPartManager.Personalization.Scope == PersonalizationScope.Shared)
                {
                    if (OnApplyChanges != null)
                    {
                        OnApplyChanges.Invoke(this, new EditorPartEventArgs
                        {
                            Value = Control.SelectedValue,
                            Text = Control.SelectedItem.Text
                        });

                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }

            //var part = (TWebPart)WebPartToEdit;
            //part.Value = DropDownList.SelectedValue;

            //return true;
        }

        public override void SyncChanges()
        {
            //var part = (TWebPart)WebPartToEdit;
            //string grupo = part.Grupo;

            EnsureChildControls();

            if (WebPartManager.Personalization.Scope == PersonalizationScope.Shared)
            {
                Control.ClearSelection();
                ListItem item = Control.Items.FindByValue(Value);
                if (item != null) item.Selected = true;
            }
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            Control = new DropDownList();

            foreach (var item in _dataSource)
            {
                Control.Items.Add(new ListItem
                {
                    Text = item.Value,
                    Value = item.Key
                });
            }

            Controls.Add(Control);

            if (LinkButton != null)
            {
                Controls.Add(LinkButton);
            }

            base.CreateChildControls();
            ChildControlsCreated = true;
        }

        public Control LinkButton { get; set; }
        public void CreateLinkItem(string text, string url)
        {

            var onClientScript =
                string.Format("var dialogOptions = SP.UI.$create_DialogOptions();dialogOptions.url = '{0}';SP.UI.ModalDialog.showModalDialog(dialogOptions);return false;", url);
            
            var linkButton = new LinkButton();
            linkButton.Text = text;
            linkButton.OnClientClick = onClientScript;
            LinkButton = linkButton;
        }
    }
}
