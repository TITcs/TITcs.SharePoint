namespace TITcs.SharePoint.Repository
{
    public abstract class SqlServerRepository : SqlServerConnection
    {
        protected SqlServerRepository(string connectionString)
            : base(connectionString)
        {

        }
    }
}