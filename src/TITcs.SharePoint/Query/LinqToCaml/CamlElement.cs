using System;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class CamlElement
    {
        public CamlLogical Logical { get; set; }
        public CamlOperators Operator { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string[] Values { get; set; }
    }
}