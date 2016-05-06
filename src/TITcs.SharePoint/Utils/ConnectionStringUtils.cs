using System;

namespace TITcs.SharePoint.Utils
{
    public static class ConnectionStringUtils
    {
        public static string Read(string key)
        {
            return ReadConnectionStrings(key);
        }

        private static string ReadConnectionStrings(string key)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[key];

            if (connectionString == null)
                throw new Exception(string.Format("The key \"{0}\" in connectionStrings not found in web.config", key));

            return connectionString.ConnectionString;
        }
    }
}
