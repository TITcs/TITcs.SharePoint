using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.SharePoint.Publishing.Fields;

namespace TITcs.SharePoint.Query
{
    public abstract class QueryBase : IQuery
    {
        internal Dictionary<string, object> _listData = new Dictionary<string, object>();
        internal Dictionary<string, object> _loadData = new Dictionary<string, object>();
        internal Dictionary<string, QueryCommand> _loadCommand = new Dictionary<string, QueryCommand>();
        private bool disposed = false;

        public abstract object Context { get; }

        public bool IsClient { get { return Context is ClientContext; } }

        protected QueryBase()
        {
            LoadedList = new List<ListQuery>();
        }

        protected virtual object ValidateValueType(object value)
        {
            if (value != null)
            {
                var type = value.GetType().ToString();

                switch (type)
                {
                    case "System.String":

                        string stringValue = value.ToString();

                        if (stringValue.ToLower().IndexOf("<img") == 0)
                        {
                            PublishedImage image = stringValue;

                            if (image != null)
                                return image;
                        }

                        return value.ToString();

                    case "System.DateTime":

                        return (DateTime)value;

                    case "System.Single":

                        return (Single)value;

                    case "System.Float":

                        return (float)value;

                    case "System.Boolean":

                        return (bool)value;

                    case "System.Int32":

                        return (Int32)value;

                    case "System.Double":

                        return Double.Parse(value.ToString());

                    case "Microsoft.SharePoint.Client.FieldLookupValue[]":
                    case "Microsoft.SharePoint.Client.FieldUserValue[]":

                        var fieldLookupValues = ((FieldLookupValue[])value);
                        return fieldLookupValues.ToDictionary(item => item.LookupId, item => item.LookupValue);

                    case "Microsoft.SharePoint.Client.FieldUserValue":
                    case "Microsoft.SharePoint.Client.FieldLookupValue":

                        var fieldLookupValue = ((FieldLookupValue)value);
                        var data = new Dictionary<int, string>();
                        data.Add(fieldLookupValue.LookupId, fieldLookupValue.LookupValue);
                        return data;
                    case "Microsoft.SharePoint.Publishing.ToArray.ImageFieldValue":
                        var fieldImageServer = value as ImageFieldValue;
                        return new PublishedImage()
                        {
                            Src = fieldImageServer.ImageUrl,
                            Alt = fieldImageServer.AlternateText,
                            Height = fieldImageServer.Height.ToString(),
                            Width = fieldImageServer.Width.ToString()
                        };
                    case "Microsoft.SharePoint.SPField":
                        var fieldServer = value as SPField;
                        switch (fieldServer.Type)
                        {
                            case SPFieldType.Integer:
                                return Int32.Parse(value.ToString());
                        }
                        return "";
                    case "Microsoft.SharePoint.SPFieldBoolean":
                        var fieldBooleanServer = value as SPFieldBoolean;
                        return bool.Parse(fieldBooleanServer.ToString());

                    case "Microsoft.SharePoint.SPFieldCurrency":
                        var fieldCurrencyServer = value as SPFieldCurrency;
                        return double.Parse(fieldCurrencyServer.ToString());

                    case "Microsoft.SharePoint.SPFieldDecimal":
                        var fieldDecimalServer = value as SPFieldDecimal;
                        return double.Parse(fieldDecimalServer.ToString());
                    case "Microsoft.SharePoint.SPFieldMultiLineText":
                        return value;
                    //{
                    //    var fieldMultiLineText = value as SPFieldMultiLineText;
                    //    return fieldMultiLineText.ToString();
                    //}
                    case "Microsoft.SharePoint.SPFieldText":
                        return value;
                    //{
                    //    var fieldText = value as SPFieldText;
                    //    return fieldText.ToString();
                    //}
                    case "Microsoft.SharePoint.SPFieldLookupValue":

                        var fieldLookupValueServer = value as SPFieldLookupValue;
                        var dataServer = new Dictionary<int, string>();
                        dataServer.Add(fieldLookupValueServer.LookupId, fieldLookupValueServer.LookupValue);
                        return dataServer;
                    case "Microsoft.SharePoint.SPFieldLookupValueCollection":

                        var fieldLookupValueCollectionServer = value as SPFieldLookupValueCollection;

                        return fieldLookupValueCollectionServer.ToDictionary(fldValue => fldValue.LookupId, fldValue => fldValue.LookupValue);

                    case "Microsoft.SharePoint.SPFieldUrlValue":

                        var fieldUrlValue = value as Microsoft.SharePoint.SPFieldUrlValue;
                        return new Url
                        {
                            Description = fieldUrlValue.Description,
                            Uri = new Uri(fieldUrlValue.Url)
                        };
                    case "Microsoft.SharePoint.Client.FieldUrlValue":

                        var fieldUrl = value as Microsoft.SharePoint.Client.FieldUrlValue;
                        return new Url
                        {
                            Description = fieldUrl.Description,
                            Uri = new Uri(fieldUrl.Url)
                        };
                    case "Microsoft.SharePoint.Client.File":

                        if (value != null)
                        {
                            File file = value as Microsoft.SharePoint.Client.File;
                            return file;
                        }

                        return null;

                    case "Microsoft.SharePoint.SPFile":

                        if (value != null)
                        {
                            File file = value as SPFile;
                            return file;
                        }

                        return null;

                    case "Microsoft.SharePoint.Client.FieldComputed":
                    case "Microsoft.SharePoint.Client.FieldCurrency":
                        return ((Field)value).DefaultValue;
                    case "Microsoft.SharePoint.Client.FieldCalculatedErrorValue":
                    case "Microsoft.SharePoint.Client.FieldChoice":
                    case "Microsoft.SharePoint.Client.FieldDateTime":
                    case "Microsoft.SharePoint.Client.FieldGeolocation":
                    case "Microsoft.SharePoint.Client.FieldGeolocationValue":
                    case "Microsoft.SharePoint.Client.FieldGuid":
                    case "Microsoft.SharePoint.Client.FieldLink":
                    case "Microsoft.SharePoint.Client.FieldMultiChoice":
                    case "Microsoft.SharePoint.Client.FieldMultiLineText":
                    case "Microsoft.SharePoint.Client.FieldNumber":
                    case "Microsoft.SharePoint.Client.FieldRatingScale":
                    case "Microsoft.SharePoint.Client.FieldRatingScaleQuestionAnswer":
                    case "Microsoft.SharePoint.Client.FieldStringValues":
                    case "Microsoft.SharePoint.Client.FieldText":
                    case "Microsoft.SharePoint.Client.FieldType":
                    case "Microsoft.SharePoint.Client.FieldUrl":
                    case "Microsoft.SharePoint.Client.FieldUser":
                    case "Microsoft.SharePoint.Client.FieldUserSelectionMode":
                        throw new Exception();
                    default:
                        throw new Exception(string.Format("The {0} type is not defined", type));
                }
            }

            return null;
        }

        protected string ViewFields<T>(Expression<Func<T, object>>[] retrievals)
        {
            return string.Join("", retrievals.Select(i => "<FieldRef Name=\"" + ((PropertyInfo)((MemberExpression)i.Body).Member).Name + "\" />").ToArray());
        }

        protected string[] ToArray<T>(Expression<Func<T, object>>[] expressions)
        {
            return expressions.Select(MemberName).ToArray();
        }

        protected string MemberName<TModel>(Expression<Func<TModel, object>> expression)
        {
            var exp = (expression.Body is MemberExpression)
                ? ((MemberExpression)expression.Body)
                : ((UnaryExpression)expression.Body).Operand as MemberExpression;

            return exp.Member.Name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~QueryBase()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // called via myClass.Dispose(). 
                    // OK to use any private object references
                }

                disposed = true;
            }
        }

        public IList<ListQuery> LoadedList { get; set; }

        public abstract Dictionary<string, string[]> LoadFieldValues(params string[] fieldNames);

        public abstract void Execute();

        public Expression<Func<TModel, bool>> AsPredicate<TModel>(bool evaluate)
        {
            return (evaluate) ? PredicateBuilder.True<TModel>() : PredicateBuilder.False<TModel>();
        }

        public IQueryable<TModel> AsQueryable<TModel>()
        {
            return new LinqToCaml.CamlQueryable<TModel>();
        }

        #region Load
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listName, System.Linq.IQueryable<TModel> queryable);
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listName, System.Linq.IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, System.Linq.IQueryable<TModel> queryable);
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, System.Linq.IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true);
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listTitle, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy);
        public abstract Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, string camlQuery, params Expression<Func<TModel, object>>[] fields);
        #endregion Load

        public abstract int InsertItemByContentType<TContentType>(string listName, Fields<TContentType> fields);
        public abstract int InsertItem<TContentType>(string listName, Fields<TContentType> item);
        public abstract void DeleteItem(string listName, int id);
        public abstract void UpdateItem<TContentType>(string listName, Fields<TContentType> item);
        public abstract UserGroup[] GetGroups();
        public abstract UserGroup GetGroupByName(string name);
        public abstract User[] GetUsers();
        public abstract User[] GetUsersByGroup(string groupName);
        public abstract User GetUser(string login);
        public abstract User GetUserById(string id);
        public abstract User CurrentUser();
        public abstract string ContextUrl { get; }

        public ListQuery GetListQueryByTitle(string title)
        {
            if (LoadedList.Count == 0)
                return null;

            return LoadedList.First(l => l.Title.Equals(title));
        }

        public abstract ItemUploaded UploadDocument<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null,
            int maxLength = 4000000);

        public abstract ContextWraper GetContextWraper();

        public abstract ItemUploaded UploadImage<TContentType>(string listName, string fileName, Stream stream, Fields<TContentType> fields = null, int maxLength = 4000000);

        protected double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }


        /// <summary>
        /// Total list Items
        /// </summary>
        /// <param name="listTitle">Título da lista</param>
        /// <returns></returns>
        public abstract int CountItems(string listTitle);
    }
}
