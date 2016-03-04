using System;
using System.Web;
using System.Web.UI;
using TITcs.SharePoint.Query;
using Microsoft.SharePoint.Library;
namespace TITcs.SharePoint.UI
{
    public abstract class UISharePointControl<TSharePointLogic> : UserControl where TSharePointLogic : class
    {
        private event UISharePointEventHandler<TSharePointLogic> LoadUserControl;
        public TSharePointLogic Logic { get; private set; }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Load += UISharePointControl_Load;
            LoadUserControl += UserControlInit;
            LoadUserControl += UserControlLogicCall;
            LoadUserControl += UserControlData;
            LoadUserControl += UserControlBusiness;
            
        }

        void UserControlLogicCall(object sender, UISharePointContext<TSharePointLogic> sharePointContext)
        {
            ((IInternalUILogic)Logic).InternalUILogic();
        }

        /// <summary>
        /// Inicializa a Logic e executa
        /// </summary>
        public void InitLogic()
        {
            Logic = UIFactory.UIModel<TSharePointLogic>();
            ((IInternalUILogic)Logic).InternalUILogic();
        }

        public abstract void UserControlData(object sender, UISharePointContext<TSharePointLogic> sharePointContext);
        public abstract void UserControlBusiness(object sender, UISharePointContext<TSharePointLogic> sharePointContext);
        public abstract void UserControlInit(object sender, UISharePointContext<TSharePointLogic> sharePointContext);
        
        /// <summary>
        /// Método responsável pelo ciclo de construção do controle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UISharePointControl_Load(object sender, EventArgs e)
        {
            try
            {
                Logic = UIFactory.UIModel<TSharePointLogic>();
                
                if (LoadUserControl != null)
                {
                    var uiSharePointContext = new UISharePointContext<TSharePointLogic>(HttpContext.Current)
                    {
                        Logic = Logic,
                        SPContextWraper = ContextFactory.GetContext().GetSPContextWraper()
                    };
                    LoadUserControl(sender, uiSharePointContext);
                }
            }
            catch (Exception ex)
            {
                var ux = new UIException(ex);
                ux.LogicClass = typeof(TSharePointLogic).FullName;
                ux.ControlClass = GetType().FullName;
                ux.ModelClass = Logic.GetType().GetProperty("Model").GetValue(Logic).GetType().FullName;
                throw ux;
            }

        }
       

    }
}
