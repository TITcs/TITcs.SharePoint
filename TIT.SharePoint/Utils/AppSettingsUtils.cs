using System;

namespace TITcs.SharePoint.Utils
{
    public static class AppSettingsUtils
    {
        public static string UserName
        {
            get { return ReadAppSettings("SharePoint:UserName"); }
        }

        public static string Password
        {
            get { return ReadAppSettings("SharePoint:Password"); }
        }

        public static string NetDomain
        {
            get { return ReadAppSettings("SharePoint:NetDomain"); }
        }

        public static string Root
        {
            get { return ReadAppSettings("SharePoint:Root"); }
        }

        public static int CacheDurationInMinutes
        {
            get
            {
                try
                {
                    var minutes = ReadAppSettings("CacheDurationInMinutes");

                    return Convert.ToInt32(minutes);
                }
                catch
                {
                    return 5;
                }
            }
        }

        

        public static string Read(string key)
        {
            return ReadAppSettings(key);
        }

        private static string ReadAppSettings(string key)
        {
            object appSetting = System.Configuration.ConfigurationManager.AppSettings[key];

            if (appSetting == null)
                throw new Exception(string.Format("The key \"{0}\" in system.web/appSettings not found in web.config", key));

            return appSetting.ToString();
        }
    }
}
