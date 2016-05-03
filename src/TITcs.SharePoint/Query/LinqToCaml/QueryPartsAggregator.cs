using System.Collections.Generic;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class QueryPartsAggregator
    {
        public readonly Queue<CamlElement> CamlElements = new Queue<CamlElement>(); 
        public readonly Queue<CamlElement> SelectElements = new Queue<CamlElement>(); 
       
        internal void AddWhereClause (Stack<CamlElement> p)
        {
            while (p.Count != 0)
            {
                CamlElements.Enqueue(p.Pop());
            }
        }
        internal void AddSelectClause(Stack<CamlElement> p)
        {
            while (p.Count != 0)
            {
                SelectElements.Enqueue(p.Pop());
            }
        }
    }
}