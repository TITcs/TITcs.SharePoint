using TITcs.SharePoint.Services;

namespace ProviderHosted.Services
{
    public class CacheService : ServiceBase
    {
        [Route]
        public object Get()
        {
            string id = Model.id;

            return CacheResult(() => Ok(new
            {
                id = "c34c140f-c798-4be2-af77-ae0ea4617419",
                description = "Cache"
            }), GetType(), "Get({0})", id);

        }

        [Route]
        public object Invalidate()
        {
            InvalidateCache(GetType());

            return Call(() => Ok(null));

        }
    }
}
