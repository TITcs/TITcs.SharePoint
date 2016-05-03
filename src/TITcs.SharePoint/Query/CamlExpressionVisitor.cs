using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TITcs.SharePoint.Query
{
#pragma warning disable 414, 3021,108
    public class CamlExpressionVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public string CamlQuery
        {
            get { return _builder.ToString(); }
        }

        public void Caml(Expression expression)
        {
            this.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _builder.Append(CamlConverter(b.NodeType));
            Visit(b.Left);
            Visit(b.Right);
            _builder.Append(CamlConverter(b.NodeType, true));
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            _builder.Append(CamlValueConverter(c.Type.Name, c.Value.ToString()));
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            _builder.Append(CamlFieldConverter(m.Member.Name, ((PropertyInfo)m.Member).PropertyType.Name));
            Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression)
            {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        private string CamlValueConverter(string type, string value)
        {
            var valueConst = "<Value Type='{0}'>{1}</Value></Field>";
            switch (type)
            {
                case "String":
                    return string.Format(valueConst, "Text", value);
                case "Int32":
                    return string.Format(valueConst, "Number", value);
                default:
                    return string.Empty;

            }
        }

        private string CamlFieldConverter(string fieldName, string fieldType)
        {
            var fieldConst = "<Field Name='{0}' Value='{1}'>";
            switch (fieldType)
            {
                case "String":
                    return string.Format(fieldConst, fieldName, "Text");
                case "Int32":
                    return string.Format(fieldConst, fieldName, "Number");
                default:
                    return string.Empty;
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            _builder.Append(CamlMethodConverter(m.Method.Name));
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return Expression.Call(obj, m.Method, args);
            }
            _builder.Append(CamlMethodConverter(m.Method.Name, true));
            return m;
        }

        private string CamlMethodConverter(string methodName, bool end = false)
        {
            switch (methodName)
            {
                case "Equals":
                    return (!end) ? "<Eq>" : "</Eq>";
                case "ContainsKey":
                    return (!end) ? "<Eq>" : "</Eq>";
                default:
                    return "";
            }
        }

        private string CamlConverter(ExpressionType expressionType, bool end = false)
        {
            switch (expressionType)
            {
                case ExpressionType.And:
                    return (!end) ? "<And>" : "</And>";
                case ExpressionType.AndAlso:
                    return (!end) ? "<And>" : "</And>";
                case ExpressionType.Or:
                    return (!end) ? "<Or>" : "</Or>";
                case ExpressionType.OrElse:
                    return (!end) ? "<Or>" : "</Or>";
                case ExpressionType.Equal:
                    return (!end) ? "<Eq>" : "</Eq>";
                default:
                    return string.Empty;
            }
        }
    }
}