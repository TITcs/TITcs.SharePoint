using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class CamlCommandData
    {
        public QueryPartsAggregator QueryParts { get; set; }
        public CamlCommandData (QueryPartsAggregator queryParts)
        {
            QueryParts = queryParts;
        }

        public string Caml ()
        {
            return GenerateQuery (QueryParts.CamlElements);
        }

        public string[] Fields()
        {
            return QueryParts.SelectElements.Select(i => i.Name).ToArray();
        }

        public string GenerateQuery(Queue<CamlElement> lstOfElement)
        {
            var tempElements = new Queue<CamlElement>();
            var arranjedElements = lstOfElement.Select(i => new {item = i, Path = i.Name + i.Operator + i.Type});
            var groupedIn = arranjedElements.GroupBy(i => i.Path);
            foreach (var group in groupedIn)
            {
                if (group.Count() != 1)
                {
                    var inElement = new CamlElement();
                    inElement.Name = group.First().item.Name;
                    inElement.Logical = CamlLogical.And;
                    inElement.Operator = CamlOperators.In;
                    inElement.Values = group.Select(i => i.item.Value).ToArray();
                    inElement.Type = group.First().item.Type;
                    tempElements.Enqueue(inElement);
                }
                else
                {
                    var camlElement = new CamlElement();
                    camlElement.Name = group.First().item.Name;
                    camlElement.Logical = group.First().item.Logical;
                    camlElement.Operator = group.First().item.Operator;
                    camlElement.Value = group.First().item.Value;
                    camlElement.Type = group.First().item.Type;
                    tempElements.Enqueue(camlElement);
                }
            }
            lstOfElement = tempElements;
            var queryJoin = new StringBuilder();
            const string query = @"<{0}><FieldRef Name='{1}' {6}/><Value {2} Type='{3}'>{4}</Value></{5}>";
            
            if (lstOfElement.Count > 0)
            {
                var itemCount = 0;
                while (lstOfElement.Count != 0)
                {
                    var element = lstOfElement.Dequeue();
                    itemCount++;
                    var date = string.Empty;
                    var lookupId = string.Empty;
                    // Display only Date
                    if (String.Compare(element.Type, "DateTime", StringComparison.OrdinalIgnoreCase) == 0)
                        date = "IncludeTimeValue='false'";
                    if (String.Compare(element.Type, "Integer", StringComparison.OrdinalIgnoreCase) == 0)
                        lookupId = "LookupId='TRUE'";
                   
                  
                    if (element.Operator == CamlOperators.In)
                    {
                        queryJoin.AppendFormat("<In><FieldRef Name='{0}' /><Values>",element.Name);
                        foreach (var val in element.Values)
                        {
                            queryJoin.AppendFormat("<Value Type='{0}'>{1}</Value>", element.Type,val);
                        }
                        queryJoin.AppendFormat("</Values></In>");
                            
                    }
                    else
                    {
                        queryJoin.AppendFormat
                            (query, element.Operator, element.Name,
                                    date, element.Type, element.Value, element.Operator,lookupId);
                    }
                    

                    if (itemCount < 2)
                        continue;
                    queryJoin.Insert(0, string.Format("<{0}>", element.Logical));
                    queryJoin.Append(string.Format("</{0}>", element.Logical));
                }
                queryJoin.Insert(0, "");
                queryJoin.Append("");
            }
            return queryJoin.ToString();
        }

    }
}