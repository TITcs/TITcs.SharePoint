using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.UI
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TUIModel"></typeparam>
    /// <typeparam name="TQuery"></typeparam>
    public abstract class UILogic<TUIModel> : IUILogic<TUIModel>, IInternalUILogic where TUIModel : class
    {
        //private event UILogicEventHandler<TUIModel> LogicInit;
        //private event UILogicEventHandler<TUIModel> LogicData;
        //private event UILogicEventHandler<TUIModel> LogicBusiness;
        public TUIModel Model { get; set; }
        public IQuery Query { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Data();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Bussiness();

        protected UILogic()
        {
            //LogicInit += UILogic_LogicInit;
            //LogicData += UILogic_LogicData;
            //LogicBusiness += UILogic_LogicBusiness;
        }

        /// <summary>
        /// Cria uma instância da Model
        /// </summary>
        public void CreateModelInstance()
        {
            if (Model == null)
                Model = (TUIModel)Activator.CreateInstance(typeof(TUIModel));
        }

        public TUIModel UIModel()
        {
            if (Model == null)
                Model = (TUIModel)Activator.CreateInstance(typeof(TUIModel));

            try
            {

                Init();
                Data();
                Bussiness();
            }
            catch (Exception initException)
            {
                var ux = new UIException(initException);
                ux.LogicClass = GetType().FullName;
                ux.ModelClass = typeof(TUIModel).FullName;
                throw ux;
            }


            return Model;
        }

        void UILogic_LogicBusiness(object sender, UILogicContext<TUIModel> logicContext)
        {
            //throw new NotImplementedException();
        }

        void UILogic_LogicData(object sender, UILogicContext<TUIModel> logicContext)
        {
            //throw new NotImplementedException();
        }

        void UILogic_LogicInit(object sender, UILogicContext<TUIModel> logicContext)
        {

        }

        public object InternalUILogic()
        {
            return UIModel();
        }

        public IEnumerable<TContentType> GetChildItemModel<TContentType>(List<TContentType> items, string fileRefParent) where TContentType : TITcs.SharePoint.Query.CtpBase
        {
            if (string.IsNullOrEmpty(fileRefParent))
            {
                var availableItems = items.Where(i => GetLevel(i.FileRef) == 1).ToList();
                items.RemoveAll((i) => GetLevel(i.FileRef) == 1);
                return availableItems;
            }
            else
            {
                var paths = ConcatFileRef(fileRefParent).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                var availableItems =
                    items.Where(i =>
                        ConcatFileRef(i.FileRef).Contains(ConcatFileRef(fileRefParent)) &&
                        ConcatFileRef(i.FileRef).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length ==
                        (paths.Length + 1)).Select(i => i).ToList();

                items.RemoveAll(i =>
                        ConcatFileRef(i.FileRef).Contains(ConcatFileRef(fileRefParent)) &&
                        ConcatFileRef(i.FileRef).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length ==
                        (paths.Length + 1));

                return availableItems;
            }
        }

        private string ConcatFileRef(string fileRef)
        {
            if (!fileRef.EndsWith("/"))
                fileRef += "/";

            return fileRef;
        }

        private int GetLevel(string fileReaf)
        {
            return fileReaf.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length - 2;
        }
    }
}
