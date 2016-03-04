using System;

namespace TITcs.SharePoint.Query
{
    public class Url
    {
        public string Description { get; set; }
        public Uri Uri { get; set; }

        public static implicit operator Url(String r)
        {
            return new Url();
        }
    }
}
