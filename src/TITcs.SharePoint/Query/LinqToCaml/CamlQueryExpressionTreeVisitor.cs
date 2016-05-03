using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TITcs.SharePoint.Query.LinqProvider.Clauses.Expressions;
using TITcs.SharePoint.Query.LinqProvider.Parsing;

namespace TITcs.SharePoint.Query.LinqToCaml
{
    internal class CamlQueryExpressionTreeVisitor : ThrowingExpressionTreeVisitor 
    {
        private readonly Stack<CamlElement> _queueElements = new Stack<CamlElement>();
        private bool _lookupField = false;
        
        public static Stack<CamlElement> GetCamlQueryExpression (Expression linqExpression)
        {
            var camlVisitor = new CamlQueryExpressionTreeVisitor();
            camlVisitor.VisitExpression (linqExpression);
            return camlVisitor.CamlQueryExpression();
        }

        public Stack<CamlElement> CamlQueryExpression ()
        {
            return _queueElements;
        }
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            var camlQueryModelVisitorBase = new CamlQueryModelVisitorBase();
            camlQueryModelVisitorBase.VisitQueryModel(expression.QueryModel);
            var parts = camlQueryModelVisitorBase._queryPartsAggregator;
            
            return expression;
        }

        protected override Expression VisitBinaryExpression (BinaryExpression expression)
        {
            GetLogical(expression);
            VisitExpression(expression.Left);
            VisitExpression(expression.Right);
            
            return expression;
        }

        protected override Expression VisitMemberExpression (MemberExpression expression)
        {
            var camlElement = _queueElements.Peek();
            camlElement.Name = expression.Member.Name;
            var info = expression.Member as PropertyInfo;
            if (info != null)
            {
                if (info.PropertyType.Name == "Dictionary`2")
                {
                    _lookupField = true;
                }
            }
            
           
            var exp = VisitExpression (expression.Expression);
           
            return expression;
        }

        protected override Expression VisitConstantExpression (ConstantExpression expression)
        {
            var camlElement = _queueElements.Peek();
            camlElement.Value = expression.Value.ToString();
            if (expression.Value is DateTime)
            {
                camlElement.Value = ((DateTime) expression.Value).ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            GetCamlType(expression.Value.GetType(),camlElement);

            return expression;
        }

        protected override Expression VisitMemberInitExpression(MemberInitExpression expression)
        {
            foreach (var binding in expression.Bindings)
            {
                var camlElement = new CamlElement();
                camlElement.Name = binding.Member.Name;
                _queueElements.Push(camlElement);
            }
            return expression;
        }

        protected override Expression VisitMethodCallExpression (MethodCallExpression expression)
        {
            GetLogical(expression);
             Expression obj = this.VisitExpression(expression.Object);
            //var objType = expression.Object.Type.GetProperty("").PropertyType;
            IEnumerable<Expression> args = this.VisitExpressionList(expression.Arguments);
            
            return expression;
            
        }
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.VisitExpression(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }
        private Stack<CamlElement> CamlQueryExpression (Expression expression)
        {
            return GetCamlQueryExpression (expression);

        }

        private void GetLogical (Expression expression)
        {
            CamlElement camlElement = null;
            switch (expression.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    //camlElement.Logical = CamlLogical.And;
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    //camlElement.Logical =  CamlLogical.Or;
                    break;
                case ExpressionType.NotEqual:
                    camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Neq;
                    _queueElements.Push(camlElement);
                    break;
                case ExpressionType.Equal:
                     camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Eq;
                    _queueElements.Push(camlElement);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Geq;
                    _queueElements.Push(camlElement);
                    break;
                case ExpressionType.GreaterThan:
                    camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Gt;
                    _queueElements.Push(camlElement);
                    break;
               
                case ExpressionType.LessThan:
                    camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Lt;
                    _queueElements.Push(camlElement);
                    break;
                case ExpressionType.LessThanOrEqual:
                    camlElement = new CamlElement();
                    camlElement.Operator = CamlOperators.Leq;
                    _queueElements.Push(camlElement);
                    break;
                case ExpressionType.Call:
                    var methodExpression = (MethodCallExpression) expression;
                    camlElement = new CamlElement();
                    if(methodExpression.Method.Name.Equals("Equals"))
                        camlElement.Operator = CamlOperators.Eq;
                    if (methodExpression.Method.Name.Equals("StartsWith"))
                        camlElement.Operator = CamlOperators.BeginsWith;
                    if (methodExpression.Method.Name.Equals("Contains"))
                        camlElement.Operator = CamlOperators.Contains;
                    

                    _queueElements.Push(camlElement);
                    break;
                    
                default:
                    throw new NotImplementedException();
            }
        }
        private void GetCamlType(Type type,CamlElement camlElement) 
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    camlElement.Type = "Number";
                    if (_lookupField)
                    {
                        camlElement.Type = "Integer";
                        _lookupField = false;
                    }
                    break;
                case TypeCode.String:
                    camlElement.Type = "Text";
                    if (_lookupField)
                    {
                        camlElement.Type = "Choice";
                        _lookupField = false;
                    }
                    break;
                case TypeCode.DateTime:
                    camlElement.Type = "DateTime";
                    break;
                case TypeCode.Double:
                    camlElement.Type = "Number";
                    break;
                case TypeCode.Boolean:
                    camlElement.Type = "Bool";
                    break;
                default:
                    throw new NotImplementedException();

            }
        }
    }
}
