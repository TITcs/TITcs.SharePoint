using System;

namespace TITcs.SharePoint.Query
{
    internal class QueryCommand
    {
        public string ListName { get; set; }
        public string[] Retrievals { get; set; }
        public int Limit { get; set; }
        public Type Model { get; set; }
        public Type Result { get; set; }
        public string CamlQuery { get; set; }

        public string OrderBy { get; set; }
        public bool OrderByAscending { get; set; }
    }
}