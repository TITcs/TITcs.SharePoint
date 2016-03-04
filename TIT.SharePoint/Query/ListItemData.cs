using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TITcs.SharePoint.Query
{
    public class ListItemData<TContentType> 
    {
        public Dictionary<string, object> ItemDictionary = new Dictionary<string, object>();

        public void Add(Expression<Func<TContentType, object>> predicate, object value)
        {
            var memberName = MemberName(predicate);
            ItemDictionary.Add(memberName, ValueConverter(value));
        }

        private string MemberName(Expression<Func<TContentType, object>> expression)
        {
            var exp = (expression.Body is MemberExpression) ? ((MemberExpression)expression.Body) : ((UnaryExpression)expression.Body).Operand as MemberExpression;
            return exp.Member.Name;
        }

        private object ValueConverter(object dataObject)
        {
            if (dataObject is KeyValuePair<int,string>)
            {
                return ((KeyValuePair<int, string>) dataObject).Key;
            }
            return dataObject;

        }
    }

    public enum ListItemDataType
    {
        Text,
        Image
    }
}
