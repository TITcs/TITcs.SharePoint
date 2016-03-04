using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.UI
{
    public interface IUILogic<out TSharePointModel>
    {
        IQuery Query { get; set; }
        TSharePointModel UIModel();
        
        
    }
}