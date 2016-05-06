using System.Web.ModelBinding;
using TITcs.SharePoint.Services;

namespace ProviderHosted.Services
{
    public class AvatarService : ServiceBase
    {
        [Route]
        public object Get()
        {
            return Call(() => Ok(new
                {
                    id = "c34c140f-c798-4be2-af77-ae0ea4617419",
                    url = "/img/avatar.gif"
                }));

        }

        [Route]
        public object Error()
        {
            return Error("Generic error");
        }

        [Route]
        public object BusinessRule()
        {
            return BusinessRule(0, "Business rule", null);
        }
    }
}
