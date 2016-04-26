using System;
using System.Collections.Generic;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Data.ContentTypes
{
    public abstract class ContentType
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int _Level { get; set; }
        public string FileRef { get; set; }
        public double LikesCount { get; set; }
        public Dictionary<int, string> LikedBy { get; set; }
        public Dictionary<int, string> Author { get; set; }
        public File File { get; set; }
        public string Title { get; set; }
    }
}
