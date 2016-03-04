using System.Globalization;
using System.IO;
using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using TITcs.SharePoint.Log;

namespace TITcs.SharePoint.Query
{
    public class ServerQuery : QueryBase
    {
        private readonly SPWeb _serverContext;
        private SPField _currentField = null;
        public ServerQuery(object serverQuery)
        {
            _serverContext = (SPWeb)serverQuery;
        }

        public override object Context { get { return _serverContext; } }

        protected string MemberName<TModel>(Expression<Func<TModel, object>> expression)
        {
            var exp = (expression.Body is MemberExpression) ? ((MemberExpression)expression.Body) : ((UnaryExpression)expression.Body).Operand as MemberExpression;
            return exp.Member.Name;
        }

        public override Dictionary<string,string[]> LoadFieldValues(params string[] fieldName)
        {
            var dic = new Dictionary<string, string[]>();
            var items = fieldName.Select(f => _serverContext.AvailableFields.GetFieldByInternalName(f)).ToArray();
            foreach (var spField in items)
            {
                var choiceField = spField as SPFieldChoice;
                if(choiceField!=null)
                    dic.Add(spField.InternalName,choiceField.Choices.Cast<string>().ToArray());
            }
            return dic;
        }

        public override void Execute()
        {
            try
            {
                using (_serverContext)
                {
                    for (int i = 0; i < _loadData.Keys.Count; i++)
                    {
                        var loadDataKey = _loadData.Keys.ElementAt(i);
                        var loadDataCommand = _loadCommand[loadDataKey];
                        SPList list = null;
                        var query = new SPQuery
                        {
                            ViewAttributes = "Scope=\"RecursiveAll\""
                        };


                        if (!string.IsNullOrEmpty(loadDataCommand.ListName))
                        {
                            list = _serverContext.Lists.TryGetList(loadDataCommand.ListName);
                        }

                        if (loadDataCommand.Retrievals != null)
                        {
                            query.ViewFields = string.Join("", loadDataCommand.Retrievals.Select(j => "<FieldRef Name=\"" + j + "\" />").ToArray());
                        }
                        if (loadDataCommand.Limit > 0)
                        {
                            query.RowLimit = (uint)loadDataCommand.Limit;
                        }

                        var modelName = loadDataCommand.Model.GetCustomAttribute<ContentAttribute>(true).Name;

                        if (!string.IsNullOrEmpty(loadDataCommand.CamlQuery))
                        {
                            query.Query = "<Where>" + loadDataCommand.CamlQuery + "</Where>";
                            }
                        else
                        {
                            query.Query = "<Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>" +
                                              modelName + "</Value></Eq></Where>";
                        }

                        if (!string.IsNullOrEmpty(loadDataCommand.OrderBy))
                            query.Query += loadDataCommand.OrderBy;

                        var items = list.GetItems(query);
                        LoadedList.Add(new ListQuery(loadDataCommand.ListName, list.DefaultViewUrl));
                        _loadData[loadDataKey] = items;
                    }


                    for (int i = 0; i < _loadData.Keys.Count; i++)
                    {
                        var loadDataKey = _loadData.Keys.ElementAt(i);
                        var loadDataCommand = _loadCommand[loadDataKey];
                        var items = (SPListItemCollection)_loadData[loadDataKey];
                        var listResult = (IList)Activator.CreateInstance(loadDataCommand.Result);

                        foreach (SPListItem item in items)
                        {
                            object newModel = Activator.CreateInstance(loadDataCommand.Model);

                            foreach (var property in loadDataCommand.Retrievals)
                            {
                                var propertyInfo = loadDataCommand.Model.GetProperty(property);
                                
                                
                                    if (item.Fields.ContainsField(propertyInfo.Name))
                                    {
                                        _currentField = item.Fields.GetFieldByInternalName(propertyInfo.Name);
                                        if (item[propertyInfo.Name] != null)
                                        {
                                            var fieldValue = item[propertyInfo.Name];

                                            propertyInfo.SetValue(newModel, ValidateValueType(fieldValue));
                                        }
                                        

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (propertyInfo.Name.Equals("File"))
                                            {
                                                File titFile = item.File;
                                                propertyInfo.SetValue(newModel, titFile);
                                            }
                                            else
                                            {
                                                propertyInfo.SetValue(newModel, item[propertyInfo.Name].ToString());
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new UIException(ex);
                                        }
                                    }
                                
                            }

                            listResult.Add(newModel);

                        }
                        _loadData[loadDataKey] = listResult;
                    }
                }
            }
            catch (Exception e)
            {
                throw new UIException(e);
            }
        }

        public override UserGroup[] GetGroups()
        {
            return
                _serverContext.SiteGroups.Cast<SPGroup>()
                    .Select(g => new UserGroup() { Id = g.ID.ToString(), Name = g.Name })
                    .ToArray();
        }

        public override UserGroup GetGroupByName(string name)
        {

            var group = _serverContext.SiteGroups.GetByName(name);
            return new UserGroup() { Id = group.ID.ToString(), Name = group.Name };


        }

        public override User[] GetUsers()
        {
            return
                _serverContext.SiteUsers.Cast<SPUser>()
                    .Select(g => new User() { Id = g.ID.ToString(), Name = g.Name, Login = g.LoginName })
                    .ToArray();
        }

        public override User[] GetUsersByGroup(string groupName)
        {
            var group = _serverContext.SiteGroups.GetByName(groupName);
            return group.Users.Cast<SPUser>().Select(g => new User() { Id = g.ID.ToString(), Name = g.Name, Login = g.LoginName })
                    .ToArray();

        }

        public override User GetUser(string login)
        {
            var currentUser = _serverContext.SiteUsers.Cast<SPUser>().FirstOrDefault(i => i.LoginName.Equals(login));
            return bindCurrentUser(currentUser);
        }

        public override User GetUserById(string id)
        {
            var currentUser = _serverContext.SiteUsers.GetByID(Int32.Parse(id));
            return bindCurrentUser(currentUser);
        }

        public override User CurrentUser()
        {
            var currentUser = _serverContext.CurrentUser;
            return bindCurrentUser(currentUser);
        }

        private User bindCurrentUser(SPUser currentUser)
        {
            if (currentUser != null)
                return new User()
                {
                    Claims = currentUser.LoginName,
                    Id = currentUser.ID.ToString(),
                    Login = currentUser.LoginName,
                    Name = currentUser.Name,
                    Groups = currentUser.Groups.Cast<SPGroup>().Select(g => new UserGroup
                    {
                        Id = g.ID.ToString(),
                        Name = g.Name
                    }).ToList()
                };
            return null;
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable)
        {
            return Load(listName, 0, queryable, null);
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true)
        {
            return Load(listName, 0, queryable, orderBy, orderByAscending);
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy)
        {
            return Load(listName, 0, queryable, orderBy);
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable)
        {
            return Load(listName, limit, queryable, null);
        }

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, System.Linq.IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true)
        {
            Guid tempKey = Guid.NewGuid();
            var iCommand = new QueryCommand();
            var camlQueryable = (LinqToCaml.CamlQueryable<TModel>)queryable;

            if (limit == 0)
                limit = 100;

            iCommand.Limit = limit;
            iCommand.ListName = listName;
            iCommand.Model = typeof(TModel);
            iCommand.Result = typeof(List<TModel>);

            camlQueryable.ToList();

            if (orderBy != null)
            {
                var query = new StringBuilder();

                query.Append(orderByAscending ? "<OrderBy Override=\"TRUE\">" : "<OrderBy>");

                var strOrderBy = orderBy.ToString();
                var len = strOrderBy.Length;
                var index = strOrderBy.IndexOf(".", StringComparison.InvariantCulture);
                var property = strOrderBy.Substring(index + 1, (len - 1) - index);

                query.Append("<FieldRef Name=\"" + property.Replace(")", "") + "\" Ascending=\"" + orderByAscending.ToString().ToUpper() + "\" />");
                query.Append("</OrderBy>");

                iCommand.OrderBy = query.ToString();
                iCommand.OrderByAscending = orderByAscending;
            }

            iCommand.CamlQuery = camlQueryable.Caml();
            iCommand.Retrievals = camlQueryable.Fields();
            _loadCommand.Add(tempKey.ToString(), iCommand);
            _loadData.Add(tempKey.ToString(), null);
            return () => (IList<TModel>)_loadData[tempKey.ToString()];
        }


        public override int InsertItem<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            Logger.Information("ServerQuery.InsertItem<TContentType>", string.Format("List = {0}, Fields = {1}", listName, string.Join(",", itemData.ItemDictionary.Select(i => string.Format("{0} = {1}", i.Key, i.Value)).ToArray())));

            using (_serverContext)
            {
                var list = _serverContext.Lists[listName];

                SPListItem newitem = list.AddItem();

                SPContentType contentType = _serverContext.AvailableContentTypes[typeof(TContentType).GetCustomAttribute<ContentAttribute>(true).Name]; // GetContentType(list, typeof(TModel).GetCustomAttribute<ModelAttribute>(true).Name);

                bool allowUnsafeUpdates = _serverContext.AllowUnsafeUpdates;
                _serverContext.AllowUnsafeUpdates = true;

                newitem["ContentTypeId"] = contentType.Id.ToString();

                if (list.ContentTypes[contentType.Name] != null)
                {
                    foreach (var item in itemData.ItemDictionary)
                    {
                        newitem[item.Key] = item.Value;
                    }
                }
                newitem.Update();

                _serverContext.AllowUnsafeUpdates = allowUnsafeUpdates;

                return newitem.ID;
            }
        }

        public override int InsertItemByContentType<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            using (_serverContext)
            {
                var list = _serverContext.Lists[listName];

                SPListItem newitem = list.AddItem();

                SPContentType contentType = _serverContext.AvailableContentTypes[typeof(TContentType).GetCustomAttribute<ContentAttribute>(true).Name]; // GetContentType(list, typeof(TModel).GetCustomAttribute<ModelAttribute>(true).Name);

                bool allowUnsafeUpdates = _serverContext.AllowUnsafeUpdates;
                _serverContext.AllowUnsafeUpdates = true;

                newitem["ContentTypeId"] = contentType.Id.ToString();

                if (list.ContentTypes[contentType.Name] != null)
                {
                    foreach (var item in itemData.ItemDictionary)
                    {
                        newitem[item.Key] = item.Value;
                    }
                }
                newitem.Update();
                _serverContext.AllowUnsafeUpdates = allowUnsafeUpdates;

                return newitem.ID;
            }
        }


        protected override object ValidateValueType(object value)
        {
            var lookup = new Dictionary<int, string>();
            
            switch (_currentField.Type)
            {
                case SPFieldType.Invalid:
                    var imageField = value as Microsoft.SharePoint.Publishing.Fields.ImageFieldValue;
                    if (imageField != null)
                    {
                        return new PublishedImage()
                        {
                            Src = imageField.ImageUrl,
                            Alt = imageField.AlternateText,
                            Height = imageField.Height.ToString(CultureInfo.InvariantCulture),
                            Width = imageField.Width.ToString(CultureInfo.InvariantCulture)
                        };
                    }
                    break;
                case SPFieldType.Integer:
                    {
                        return Int32.Parse(value.ToString());

                    }
                case SPFieldType.Text:
                    {
                        return value.ToString();

                    }
                case SPFieldType.Note:
                    {
                        return value.ToString();

                    }
                case SPFieldType.DateTime:
                    {
                        return (DateTime)value;

                    }
                case SPFieldType.Counter:
                    {
                        return (Int32)value;

                    }
                case SPFieldType.Choice:
                    {
                        return value.ToString();

                    }
                case SPFieldType.Lookup:
                    {
                        var fieldLookupValue = value as SPFieldLookupValue;
                        if (fieldLookupValue != null)
                        {
                            var lookupValue = fieldLookupValue;
                            lookup.Add(lookupValue.LookupId, lookupValue.LookupValue);
                            return lookup;
                        }
                        var collection = value as SPFieldLookupValueCollection;
                        if (collection != null)
                        {
                            var lookupValueCollection = collection;
                            lookup = lookupValueCollection.ToDictionary(i => i.LookupId, j => j.LookupValue);
                            return lookup;
                        }
                        var stringLookup = value as string;
                        if (stringLookup != null)
                        {
                            if (stringLookup.IndexOf(";#") > 0)
                            {
                                var lkpValue = new SPFieldLookupValue(stringLookup);
                                lookup.Add(lkpValue.LookupId, lkpValue.LookupValue);
                                return lookup;
                            }
                            return stringLookup;
                        }

                        break;
                    }
                case SPFieldType.Boolean:
                    {
                        return (bool)value;

                    }
                case SPFieldType.Number:
                    {
                        return double.Parse(value.ToString());

                    }
                case SPFieldType.Currency:
                    {
                        return double.Parse(value.ToString());

                    }
                case SPFieldType.URL:
                    {
                        try
                        {
                            var urlValue = value as string;
                            if (urlValue.IndexOf(',') > 0)
                            {
                                var parts = urlValue.Split(',');

                                return new Url
                                {
                                    Uri = new Uri(parts[0]),
                                    Description = string.Join(",", parts.Skip(1).Select(i => i).ToArray())
                                };
                            }
                        }
                        catch (Exception e)
                        {
                            new UIException(e);
                        }

                        return null;
                    }
                case SPFieldType.Computed:
                    break;
                case SPFieldType.Threading:
                    break;
                case SPFieldType.Guid:
                    break;
                case SPFieldType.MultiChoice:
                    break;
                case SPFieldType.GridChoice:
                    break;
                case SPFieldType.Calculated:
                    break;
                case SPFieldType.File:

                    if (value == null)
                        return null;

                    File file = value as SPFile;
                    return file;

                case SPFieldType.Attachments:
                    break;
                case SPFieldType.User:
                    {
                        //Usado somente quando o campo permite somente selecionar um usuário ou grupo
                        if (value is string)
                        {
                            var stringLookup = value as string;
                            if (stringLookup != null)
                            {
                                if (stringLookup.IndexOf(";#") > 0)
                                {
                                    var lkpValue = new SPFieldLookupValue(stringLookup);
                                    lookup.Add(lkpValue.LookupId, lkpValue.LookupValue);
                                    return lookup;
                                }
                                return stringLookup;
                            }
                        }

                        //Usado somente quando o campo permite selecionar vários usuários ou grupos
                        if (value is SPFieldUserValueCollection)
                        {
                            var userValues = value as SPFieldUserValueCollection;

                            if (userValues != null)
                            {
                                foreach (var userValue in userValues)
                                {
                                    lookup.Add(userValue.LookupId, userValue.LookupValue);
                                }

                                return lookup;
                            }
                        }

                        return null;
                    }
                case SPFieldType.Recurrence:
                    break;
                case SPFieldType.CrossProjectLink:
                    break;
                case SPFieldType.ModStat:
                    break;
                case SPFieldType.Error:
                    break;
                case SPFieldType.ContentTypeId:
                    break;
                case SPFieldType.PageSeparator:
                    break;
                case SPFieldType.ThreadIndex:
                    break;
                case SPFieldType.WorkflowStatus:
                    break;
                case SPFieldType.AllDayEvent:
                    break;
                case SPFieldType.WorkflowEventType:
                    break;
                case SPFieldType.Geolocation:
                    break;
                case SPFieldType.OutcomeChoice:
                    break;
                case SPFieldType.MaxItems:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return base.ValidateValueType(value);
        }

        public override void DeleteItem(string listName, int id)
        {
            Logger.Information("ServerQuery.DeleteItem", string.Format("List = {0}, ID = {1}", listName, id));

            using (_serverContext)
            {
                var list = _serverContext.Lists[listName];

                bool allowUnsafeUpdates = _serverContext.AllowUnsafeUpdates;
                _serverContext.AllowUnsafeUpdates = true;

                var item = list.GetItemById(id);

                item.Delete();
                _serverContext.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public override void UpdateItem<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            Logger.Information("ServerQuery.UpdateItem<TContentType>", string.Format("List = {0}, Fields = {1}", listName, string.Join(",", itemData.ItemDictionary.Select(i => string.Format("{0} = {1}", i.Key, i.Value)).ToArray())));

            if (!itemData.ItemDictionary.ContainsKey("ID"))
                throw new ArgumentException("Não é possível atualizar o item sem o respectivo ID");

            var itemId = itemData.ItemDictionary["ID"].ToString();

            Int32 id = 0;

            if (!Int32.TryParse(itemId, out id))
                throw new ArgumentException("ID precisa ser maior do que zero");

            using (_serverContext)
            {
                var list = _serverContext.Lists[listName];

                bool allowUnsafeUpdates = _serverContext.AllowUnsafeUpdates;
                _serverContext.AllowUnsafeUpdates = true;

                var item = list.GetItemById(id);

                foreach (var value in itemData.ItemDictionary)
                {
                    if (!value.Key.Equals("ID", StringComparison.InvariantCultureIgnoreCase))
                        item[value.Key] = value.Value;
                }

                item.Update();
                _serverContext.AllowUnsafeUpdates = allowUnsafeUpdates;
            }
        }

        public override string ContextUrl
        {
            get { return SPContext.Current.Web.Url; }
        }

        public override SPContextWraper GetSPContextWraper()
        {
            try
            {
                
                var listItem = SPContext.Current.ListItem;
                
                var spContextWraper = new SPContextWraper();
                if (listItem != null)
                {
                    spContextWraper.Item = listItem.Fields.Cast<SPField>()
                        .ToDictionary(i => i.InternalName, i => ValidateValueType(listItem[i.InternalName]));
                    spContextWraper.ListItemId = listItem.ID.ToString();
                }
                spContextWraper.List = SPContext.Current.List.Title;
                spContextWraper.ListId = SPContext.Current.ListId.ToString("D");
                return spContextWraper;
                
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Envia uma imagem para uma lista do tipo Biblioteca de Imagens
        /// </summary>
        /// <param name="listName">Título da lista</param>
        /// <param name="fileName">Nome da imagem</param>
        /// <param name="stream">Stream da imagem</param>
        /// <param name="itemData">Informações adicionais a serem armazenados na imagem</param>
        /// <param name="maxLength">Tamanho máximo permitido para a imagem</param>
        /// <returns></returns>
        public override ImageUploaded UploadImage<TContentType>(string listName, string fileName, Stream stream, ListItemData<TContentType> itemData = null, int maxLength = 4000000)
        {
            if (stream.Length > maxLength)
                throw new Exception(string.Format("O tamanho máximo do arquivo é de {0}Mb", ConvertBytesToMegabytes(maxLength)));

            string ext = Path.GetExtension(fileName).ToLower();

            if (ext == null || (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".tif" && ext != ".gif"))
                throw new Exception("A imagem não está no formato correto. Os formatos permitidos são: gif, jpg, png, bmp, tif e jpeg.");

            fileName = fileName.Replace(" ", "-");

            var list = _serverContext.Lists[listName];
            
            var fileRef = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fileName);

            bool allowUnsafeUpdates = _serverContext.AllowUnsafeUpdates;
            _serverContext.AllowUnsafeUpdates = true;
            
            var file = list.RootFolder.Files.Add(fileRef, stream, true);

            if (itemData != null)
            {
                foreach (var data in itemData.ItemDictionary)
                {
                    file.Item[data.Key] = data.Value;
                }
                file.Item.Update();

            }

            _serverContext.AllowUnsafeUpdates = allowUnsafeUpdates;

            return new ImageUploaded
            {
                FileRef = fileRef,
                Name = fileName
            };
        }

        public override int CountListItems(string listName)
        {
            var list = _serverContext.Lists[listName];
            return list.ItemCount;
        }
    }
}
