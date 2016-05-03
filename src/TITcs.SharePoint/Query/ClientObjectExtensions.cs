using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.Linq.Expressions;

namespace TITcs.SharePoint.Query
{
    public static class ClientObjectExtensions
    {
        //private static Expression<Func<ListItemCollection, object>>[] IncludeFields(string[] fieldNames)
        //{
        //    var list = new List<Expression<Func<ListItemCollection, object>>>();

        //    foreach (var fieldName in fieldNames)
        //    {
        //        list.Add(items => items.Include(
        //            i => i[fieldName]
        //            ));
        //    }

        //    return list.ToArray();
        //}

        private static string ValidateValueType(object value)
        {
            if (value != null)
            {
                var type = value.GetType().ToString();

                switch (type)
                {
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.DateTime":
                    case "System.String":
                        return value.ToString();
                    case "Microsoft.SharePoint.Client.FieldUrlValue":
                        return (value as Microsoft.SharePoint.Client.FieldUrlValue).Url;
                    default:
                        throw new Exception(string.Format("The test {0} is not defined", type));
                }
            }

            return null;
        }

        public static IList<T> LoadList<T>(this ClientContext clientContext, string listName, string[] includeFields)
        {
            var list = clientContext.Web.Lists.GetByTitle(listName);

            var type = typeof(T);
            var camlQuery = new CamlQuery()
            {
                ViewXml = string.Format("<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>{0}</Value></Eq></Where></Query><ViewFields>{1}</ViewFields><QueryOptions><ViewAttributes Scope=\"RecursiveAll\" /></QueryOptions></View>", type.Name.Substring(0, type.Name.Length - 5), string.Join("", includeFields.Select(i => "<FieldRef Name=\"" + i + "\" />").ToArray()))
            };

            var items = list.GetItems(camlQuery);

            clientContext.Load(items);
            clientContext.ExecuteQuery();

            var result = new List<T>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    T newModel = (T)Activator.CreateInstance(type);
                    foreach (var property in newModel.GetType().GetProperties())
                    {
                        if (item.FieldValues.ContainsKey(property.Name))
                            property.SetValue(newModel, ValidateValueType(item.FieldValues[property.Name]));
                        else
                        {
                            try
                            {
                                property.SetValue(newModel, item[property.Name].ToString());   
                            }
                            catch { }
                        }
                    }
                    result.Add(newModel);
                }
            }
            return result;
        }
    }
}
