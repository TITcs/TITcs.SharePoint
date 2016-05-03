using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.SharePoint.Client;
using TITcs.SharePoint.Query.LinqProvider;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class CamlQueryableExecutor : IQueryExecutor
    {
        private string _camlString = string.Empty;
        private string[] _fields = null;
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            return ExecuteCollection<T>(queryModel).SingleOrDefault();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            return ExecuteCollection<T>(queryModel).SingleOrDefault();
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            var modelName = typeof(T).GetCustomAttribute<ContentAttribute>(true).Name;
            var commandCaml = CamlQueryModelVisitorBase.GenerateCaml(queryModel);
            if (commandCaml.QueryParts.CamlElements.FirstOrDefault(i => i.Name.Equals("ContentType")) == null)
                commandCaml.QueryParts.CamlElements.Enqueue(new CamlElement()
                {
                    Logical = CamlLogical.And,
                    Name = "ContentType",
                    Operator = CamlOperators.Eq,
                    Type = "Text",
                    Value = modelName
                });
            _camlString = commandCaml.Caml();
            _fields = commandCaml.Fields();
            return new List<T>();
        }

        public string Caml()
        {
            return _camlString;
        }

        public string[] Fields()
        {
            return _fields;
        }
    }
}