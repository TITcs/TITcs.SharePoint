using System;
using System.Collections.Generic;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Data.ContentTypes
{
    public abstract class ContentType
    {
        public int _Level { get; set; }
        public Dictionary<int, string> Author { get; set; }
        public Dictionary<int, string> ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public File File { get; set; }
        public string FileRef { get; set; }
        public int ID { get; set; }
        public Dictionary<int, string> LikedBy { get; set; }
        public double LikesCount { get; set; }
        public DateTime Modified { get; set; }
        public string Title { get; set; }
    }
}
