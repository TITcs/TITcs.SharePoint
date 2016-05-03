using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TITcs.SharePoint.Query.LinqProvider;
using TITcs.SharePoint.Query.LinqProvider.Clauses;
using TITcs.SharePoint.Query.LinqProvider.Clauses.ResultOperators;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class CamlQueryModelVisitorBase : QueryModelVisitorBase
    {
        //private CamlCommandData _camlCommandData = null;
        internal readonly QueryPartsAggregator _queryPartsAggregator = new QueryPartsAggregator();
        public static CamlCommandData GenerateCaml (QueryModel queryModel)
        {
            var camlQueryModelVisitorBase = new CamlQueryModelVisitorBase();
            camlQueryModelVisitorBase.VisitQueryModel(queryModel);
            return camlQueryModelVisitorBase.GetCamlCommand();
        }

        internal CamlCommandData GetCamlCommand ()
        {
            return new CamlCommandData (_queryPartsAggregator);
        }
        public override void VisitWhereClause (WhereClause whereClause, QueryModel queryModel, int index)
        {
            _queryPartsAggregator.AddWhereClause(CamlQueryExpressionTreeVisitor.GetCamlQueryExpression(whereClause.Predicate));
            
        }

        public override void VisitSelectClause (SelectClause selectClause, QueryModel queryModel)
        {
            _queryPartsAggregator.AddSelectClause(CamlQueryExpressionTreeVisitor.GetCamlQueryExpression(selectClause.Selector));
        }

        //public override void VisitQueryModel(QueryModel queryModel)
        //{
        //    base.VisitQueryModel(queryModel);
        //}

        //protected override void VisitBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel queryModel)
        //{
        //    //base.VisitBodyClauses(bodyClauses, queryModel);
        //    foreach (var bodyClause in bodyClauses)
        //    {
        //        if (bodyClause is WhereClause)
        //        {
        //            var whereClause = (WhereClause) bodyClause;
        //            _queryPartsAggregator.AddWhereClause(CamlQueryExpressionTreeVisitor.GetCamlQueryExpression(whereClause.Predicate));
        //        }
        //    }
            
        //}

        //public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        //{
        //    if (resultOperator is ContainsResultOperator)
        //    {
        //        //_queryPartsAggregator.AddWhereClause(CamlQueryExpressionTreeVisitor.GetCamlQueryExpression(whereClause.Predicate));
        //    }
        //}
    }
}