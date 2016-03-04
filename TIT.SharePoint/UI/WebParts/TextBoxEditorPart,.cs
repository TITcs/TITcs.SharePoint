using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace TITcs.SharePoint.UI.WebParts
{
    public class TextBoxEditorPart : EditorPart
    {
        public event EditorPartEventHandler OnApplyChanges;

        TextBox _textBox;
        private readonly string _name;

        public TextBoxEditorPart(string name)
        {
            Title = "Parâmetros";
            ID = Guid.NewGuid().ToString();
            _name = name;
        }

        public string Value { get; set; }

        public string Name { get { return _name; } }

        public override bool ApplyChanges()
        {
            if (OnApplyChanges != null)
            {
                OnApplyChanges.Invoke(this, new EditorPartEventArgs
                {
                    Value = Control.Text
                });

                return true;
            }

            return false;

            //var part = (TWebPart)WebPartToEdit;
            //part.Value = DropDownList.SelectedValue;

            //return true;
        }

        public override void SyncChanges()
        {
            _textBox.Text = Value;
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            _textBox = new TextBox();

            Controls.Add(_textBox);

        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<b>{0}</b>", Name);
            writer.WriteBreak();
            _textBox.RenderControl(writer);
            writer.WriteBreak();
        }

        // Access the drop-down control through a property. 
        private TextBox Control
        {
            get
            {
                EnsureChildControls();
                return _textBox;
            }
        }
    }
}
