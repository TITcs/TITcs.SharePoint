using System.Collections.Generic;

namespace TITcs.SharePoint.Query
{
    public class User
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Login { get; set; }
        public string Claims { get; set; }
        public ICollection<UserGroup> Groups { get; set; }
    }

    public class UserProfileInfo
    {
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string DisplayName { get; set; }
        public string PictureUrl { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public IDictionary<string, string> Properties { get; set; }

        public string ReadProperty(string key)
        {
            if (Properties.ContainsKey(key))
            {
                return Properties[key];
            }

            return "";
        }
    }
}
