using System.Collections;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.SharePoint.Publishing.Fields;
using TITcs.SharePoint.Log;
using TITcs.SharePoint.Query.LinqToCaml;

namespace TITcs.SharePoint.Query
{
    public class ClientQuery : QueryBase
    {
        private readonly ClientContext _clientContext;

        public ClientQuery(object clientContext)
        {
            _clientContext = (ClientContext) clientContext;
            _clientContext.Load(_clientContext.Web);
        }

        public override object Context
        {
            get { return _clientContext; }
        }

        #region Execute

        #endregion Execute

        private string MemberName<TModel>(Expression<Func<TModel, object>> expression)
        {
            var exp = (expression.Body is MemberExpression)
                ? ((MemberExpression) expression.Body)
                : ((UnaryExpression) expression.Body).Operand as MemberExpression;
            return exp.Member.Name;
        }

        public override Dictionary<string, string[]> LoadFieldValues(params string[] fieldName)
        {
            var dic = new Dictionary<string, string[]>();

            if (!fieldName.Any())
                throw new UIException(new ArgumentException("fieldName"));

            var _fields =
                fieldName.Select(f => _clientContext.Web.AvailableFields.GetByInternalNameOrTitle(f)).ToArray();
            var choiceFields = new List<FieldChoice>();

            foreach (var field in _fields)
            {
                var choice = _clientContext.CastTo<FieldChoice>(field);
                choiceFields.Add(choice);
                _clientContext.Load(choice, c => c.InternalName, c => c.Choices);
            }
            _clientContext.ExecuteQuery();

            foreach (var field in choiceFields)
            {
                if (field == null) continue;
                var choices = field.Choices;
                dic.Add(field.InternalName, choices);
            }

            return dic;
        }

        public override void Execute()
        {
            try
            {
                using (_clientContext)
                {
                    for (int i = 0; i < _loadData.Keys.Count; i++)
                    {
                        var loadDataKey = _loadData.Keys.ElementAt(i);
                        var loadDataCommand = _loadCommand[loadDataKey];
                        List list = null;
                        CamlQuery query = CamlQuery.CreateAllItemsQuery();
                        String queryBuilder = string.Empty;
                        var modelName = loadDataCommand.Model.GetCustomAttribute<ContentAttribute>(true).Name;

                        if (!string.IsNullOrEmpty(loadDataCommand.ListName))
                        {
                            list = _clientContext.Web.Lists.GetByTitle(loadDataCommand.ListName);
                        }

                        if (loadDataCommand.Retrievals != null)
                        {
                            queryBuilder += "<ViewFields>" +
                                            string.Join("",
                                                loadDataCommand.Retrievals.Select(j => "<FieldRef Name=\"" + j + "\" />")
                                                    .ToArray()) + "</ViewFields>";
                        }
                        if (loadDataCommand.Limit > 0)
                        {
                            queryBuilder += string.Format("<RowLimit>{0}</RowLimit>", loadDataCommand.Limit);
                        }
                        if (queryBuilder.Length > 0)
                        {
                            queryBuilder += "</View>";

                            if (!string.IsNullOrEmpty(loadDataCommand.CamlQuery))
                            {
                                var tmpQueryXml = query.ViewXml.Replace("<View>", "<View Scope='RecursiveAll'>")
                                    .Replace("<Query>\r\n    </Query>",
                                        "<Query><Where>" + loadDataCommand.CamlQuery + "</Where></OrderBy></Query>");

                                query.ViewXml = tmpQueryXml.Replace("</View>", queryBuilder);
                            }
                            else
                            {
                                var tmpQueryXml = query.ViewXml.Replace("<View>", "<View Scope='RecursiveAll'>")
                                    .Replace("<Query>\r\n    </Query>",
                                        "<Query><Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>" +
                                        modelName + "</Value></Eq></Where></OrderBy></Query>");

                                query.ViewXml = tmpQueryXml.Replace("</View>", queryBuilder);
                            }

                            if (loadDataCommand.OrderBy == null)
                                loadDataCommand.OrderBy = "";

                            query.ViewXml = query.ViewXml.Replace("</OrderBy>", loadDataCommand.OrderBy);
                            

                        }
                        _clientContext.Load(list, l => l.DefaultViewUrl);
                        _listData[loadDataKey] = list;

                        Logger.Information("ClientQuery.Execute", string.Format("List = {0}, ContentType = {1}, Query = {2}", loadDataCommand.ListName, modelName, query.ViewXml));

                        var items = list.GetItems(query);

                        if (loadDataCommand.Retrievals != null && loadDataCommand.Retrievals.Any(f => f.Equals("File")))
                            _clientContext.Load(items,
                                item => item.IncludeWithDefaultProperties(i1 => i1, i3 => i3.File));
                        else
                            _clientContext.Load(items);

                        _loadData[loadDataKey] = items;
                    }

                    _clientContext.ExecuteQuery();

                    for (int i = 0; i < _loadData.Keys.Count; i++)
                    {
                        var loadDataKey = _loadData.Keys.ElementAt(i);
                        var loadDataCommand = _loadCommand[loadDataKey];
                        var items = (ListItemCollection) _loadData[loadDataKey];
                        var listResult = (IList) Activator.CreateInstance(loadDataCommand.Result);

                        LoadedList.Add(new ListQuery(loadDataCommand.ListName,
                            ((List) _listData[loadDataKey]).DefaultViewUrl));

                        foreach (var item in items)
                        {
                            object newModel = Activator.CreateInstance(loadDataCommand.Model);

                            foreach (var property in loadDataCommand.Retrievals)
                            {
                                var propertyInfo = loadDataCommand.Model.GetProperty(property);

                                object value = null;
                                string valueType = null;

                                try
                                {

                                    if (!propertyInfo.Name.Equals("File"))
                                    {
                                        if (item.FieldValues.ContainsKey(propertyInfo.Name))
                                        {
                                            value = ValidateValueType(item.FieldValues[propertyInfo.Name]);
                                            valueType = item.FieldValues[propertyInfo.Name] == null
                                                ? null
                                                : item.FieldValues[propertyInfo.Name].GetType().ToString();
                                        }
                                        else
                                        {
                                            value = item[propertyInfo.Name];
                                            valueType = value == null ? null : value.GetType().ToString();
                                        }
                                    }
                                    else
                                    {
                                        value = ValidateValueType(item.File);
                                        valueType = value == null ? null : value.GetType().ToString();
                                    }

                                    Logger.Information("ClientQuery.Execute", string.Format("Property = {0}, ValueType = {1}, Value = {2}", propertyInfo.Name, valueType, value));

                                    propertyInfo.SetValue(newModel, value);
                                }
                                catch (Exception e)
                                {
                                    Logger.Unexpected("ClientQuery.Execute", string.Format("Property = {0}, ValueType = {1}, Value = {2}, Exception = {3}", propertyInfo.Name, valueType, value, e.Message));

                                }
                            }

                            listResult.Add(newModel);

                        }
                        _loadData[loadDataKey] = listResult;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Unexpected("ClientQuery.Execute", exception.Message);

                throw new UIException(exception);
            }

        }

        public override UserGroup[] GetGroups()
        {
            var userGroups = _clientContext.Web.SiteGroups;
            _clientContext.Load(userGroups, g => g.Include(i => i.Id, i => i.Title));
            _clientContext.ExecuteQuery();
            return userGroups.ToList().Select(i => new UserGroup()
            {
                Id = i.Id.ToString(),
                Name = i.Title

            }).ToArray();

        }

        public override UserGroup GetGroupByName(string name)
        {
            var userGroups = _clientContext.Web.SiteGroups;
            var groupName = userGroups.GetByName(name);

            _clientContext.Load(groupName);
            _clientContext.ExecuteQuery();
            return groupName != null ? new UserGroup() {Id = groupName.Id.ToString(), Name = groupName.Title} : null;
        }

        public override User[] GetUsers()
        {
            var webUsers = _clientContext.Web.SiteUsers;
            _clientContext.Load(webUsers, g => g.Include(i => i.Id, i => i.Title, i => i.LoginName, i => i.UserId));
            _clientContext.ExecuteQuery();
            return webUsers.ToList().Select(i => new User()
            {
                Id = i.Id.ToString(),
                Name = i.Title,
                Login = i.LoginName,
                Claims = i.LoginName

            }).ToArray();
        }

        public override User[] GetUsersByGroup(string name)
        {
            var userGroups = _clientContext.Web.SiteGroups;
            var groupName = userGroups.GetByName(name);
            _clientContext.Load(groupName.Users);
            _clientContext.ExecuteQuery();
            return groupName.Users.ToList().Select(i => new User()
            {
                Id = i.Id.ToString(),
                Name = i.Title,
                Login = i.LoginName,
                Claims = i.LoginName

            }).ToArray();
        }

        public override User GetUser(string login)
        {
            var currentUser = _clientContext.Web.SiteUsers.GetByLoginName(login);
            _clientContext.Load(currentUser, g => g.Id, g => g.Title, g => g.LoginName);
            _clientContext.ExecuteQuery();

            return bindCurrentUser(currentUser);
        }

        public override User GetUserById(string id)
        {
            var currentUser = _clientContext.Web.SiteUsers.GetById(Int32.Parse(id));
            _clientContext.Load(currentUser, g => g.Id, g => g.Title, g => g.LoginName, g => g.Groups);
            _clientContext.ExecuteQuery();
            
            return bindCurrentUser(currentUser);
        }

        private User bindCurrentUser(Microsoft.SharePoint.Client.User currentUser)
        {
            if (currentUser == null)
                return null;

            var user = new User()
            {
                Id = currentUser.Id.ToString(),
                Name = currentUser.Title,
                Login = currentUser.LoginName
            };

            var list = new List<UserGroup>();

            foreach (var group in currentUser.Groups)
            {
                list.Add(new UserGroup
                {
                    Id = group.Id.ToString(),
                    Name = group.Title
                });
            }

            user.Groups = list;

            return user;
        }

        public override User CurrentUser()
        {
            var currentUser = _clientContext.Web.CurrentUser;
            _clientContext.Load(currentUser, g => g.Id, g => g.Title, g => g.LoginName, g => g.Groups);
            _clientContext.ExecuteQuery();

            return bindCurrentUser(currentUser);
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

        public override Func<IEnumerable<TModel>> Load<TModel>(string listName, int limit, IQueryable<TModel> queryable, Expression<Func<TModel, object>> orderBy, bool orderByAscending = true)
        {
            Guid tempKey = Guid.Empty;
            var iCommand = new QueryCommand
            {
                ListName = listName
            };

            if (limit == 0)
                limit = 100;

            iCommand.Limit = limit;
            tempKey = Guid.NewGuid();
            iCommand.Model = typeof (TModel);
            iCommand.Result = typeof (List<TModel>);
            var camlQueryable = (CamlQueryable<TModel>) queryable;
            
            camlQueryable.ToList();

            if (limit > 0)
                iCommand.Limit = limit;

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

            Logger.Information("ClientQuery.Load<TModel>", string.Format("List = {0}, Query = {1}, Fields = {2}", listName, iCommand.CamlQuery, string.Join(",", iCommand.Retrievals.Select(i => i).ToArray())));

            return () => (IList<TModel>) _loadData[tempKey.ToString()];
        }

        public override int InsertItemByContentType<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            Logger.Information("ClientQuery.InsertItemByContentType<TContentType>", string.Format("List = {0}, Items = {1}, Fields = {2}", listName, string.Join(",", itemData.ItemDictionary.Select(i => string.Format("{0} = {1}", i.Key, i.Value)).ToArray())));
            
            using (_clientContext)
            {

                var list = _clientContext.Web.Lists.GetByTitle(listName);

                var listCreationInformation = new ListItemCreationInformation();
                var listItem = list.AddItem(listCreationInformation);

                ContentType oContentType = GetContentType(list,
                    typeof (TContentType).GetCustomAttribute<ContentAttribute>(true).Name);

                listItem["ContentTypeId"] = oContentType.Id.ToString();

                foreach (var item in itemData.ItemDictionary)
                {
                    listItem[item.Key] = item.Value;
                }

                listItem.Update();

                _clientContext.ExecuteQuery();

                return listItem.Id;
            }
        }

        private ContentType GetContentType(List docs, string contentType)
        {
            ContentTypeCollection listContentTypes = docs.ContentTypes;

            _clientContext.Load(listContentTypes, types => types.Include
                (type => type.Id, type => type.Name,
                    type => type.Parent));

            var result = _clientContext.LoadQuery(listContentTypes.Where(c => c.Name == contentType));

            _clientContext.ExecuteQuery();

            ContentType targetDocumentSetContentType = result.FirstOrDefault();

            return targetDocumentSetContentType;
        }

        public override int InsertItem<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            Logger.Information("ClientQuery.InsertItem<TContentType>", string.Format("List = {0}, Fields = {1}", listName, string.Join(",", itemData.ItemDictionary.Select(i => string.Format("{0} = {1}", i.Key, i.Value)).ToArray())));

            using (_clientContext)
            {
                var list = _clientContext.Web.Lists.GetByTitle(listName);

                var listCreationInformation = new ListItemCreationInformation();
                var listItem = list.AddItem(listCreationInformation);

                foreach (var item in itemData.ItemDictionary)
                    listItem[item.Key] = item.Value;

                listItem.Update();

                _clientContext.ExecuteQuery();

                return listItem.Id;
            }
        }

        public override void DeleteItem(string listName, int id)
        {
            Logger.Information("ClientQuery.DeleteItem", string.Format("List = {0}, ID = {1}", listName, id));

            using (_clientContext)
            {
                var list = _clientContext.Web.Lists.GetByTitle(listName);

                var item = list.GetItemById(id);
                item.DeleteObject();

                list.Update();

                _clientContext.ExecuteQuery();
            }
        }

        public override void UpdateItem<TContentType>(string listName, ListItemData<TContentType> itemData)
        {
            Logger.Information("ClientQuery.UpdateItem<TContentType>", string.Format("List = {0}, Fields = {1}", listName, string.Join(",", itemData.ItemDictionary.Select(i => string.Format("{0} = {1}", i.Key, i.Value)).ToArray())));

            using (_clientContext)
            {
                if (!itemData.ItemDictionary.ContainsKey("ID"))
                    throw new ArgumentException("Não é possível atualizar o item sem o respectivo ID");

                var itemId = itemData.ItemDictionary["ID"].ToString();
                Int32 id = 0;

                if (!Int32.TryParse(itemId, out id))
                    throw new ArgumentException("ID precisa ser maior do que zero");

                var list = _clientContext.Web.Lists.GetByTitle(listName);

                var item = list.GetItemById(id);

                foreach (var value in itemData.ItemDictionary)
                {
                    if (!value.Key.Equals("ID", StringComparison.InvariantCultureIgnoreCase))
                        item[value.Key] = value.Value.ToString();
                }

                item.Update();

                _clientContext.ExecuteQuery();
            }
        }

        public override string ContextUrl
        {
            get { return _clientContext.Web.Url; }
        }

        public override SPContextWraper GetSPContextWraper()
        {
            try
            {
                var pathContext = HttpContext.Current.Request.Url.PathAndQuery;
                var listName = pathContext.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                var currentList = _clientContext.Web.Lists;
                _clientContext.Load(currentList, l => l, l => l.Include(i => i.RootFolder));
                _clientContext.ExecuteQuery();
                var camlQueryUrl =
                    string.Format(
                        "<View><Query><Where><Eq><FieldRef Name=\"FileRef\"/><Value Type=\"URL\">{0}</Value></Eq></Where></Query></View>",
                        pathContext);
                var camlQuery = new CamlQuery();
                camlQuery.ViewXml = camlQueryUrl;
                var selectedList = currentList.FirstOrDefault(i => i.RootFolder.Name.Equals(listName));
                var queryRs = selectedList.GetItems(camlQuery);
                //clientContext.Load(currentList);
                _clientContext.Load(queryRs);
                _clientContext.ExecuteQuery();

                var spContextWrapper = new SPContextWraper();
                spContextWrapper.Item = queryRs.FirstOrDefault().FieldValues;
                spContextWrapper.List = selectedList.Title;
                spContextWrapper.ListId = selectedList.Id.ToString("D");
                spContextWrapper.ListItemId = queryRs.FirstOrDefault().Id.ToString();
                return spContextWrapper;
            }
            catch (Exception exception)
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
                throw new Exception("O arquivo não está no formato correto. Os formatos permitidos são: gif, jpg, png, bmp, tif e jpeg.");


            var list = _clientContext.Web.Lists.GetByTitle(listName);

            _clientContext.Load(list.RootFolder);
            _clientContext.ExecuteQuery();

            var fileRef = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fileName);
            Microsoft.SharePoint.Client.File.SaveBinaryDirect(_clientContext, fileRef, stream, true);

            Microsoft.SharePoint.Client.File newFile = _clientContext.Web.GetFileByServerRelativeUrl(fileRef);
            
            if (itemData != null)
            {
                _clientContext.Load(newFile);
                _clientContext.ExecuteQuery();

                foreach (var item in itemData.ItemDictionary)
                {
                    newFile.ListItemAllFields[item.Key] = item.Value;
                }

                newFile.ListItemAllFields.Update();
                _clientContext.ExecuteQuery();
            }

            return new ImageUploaded
            {
                FileRef = fileRef,
                Name = fileName
            };

        }

        /// <summary>
        /// Retorna o total de itens da lista
        /// </summary>
        /// <param name="listName">Título da Lista</param>
        /// <returns>Inteiro</returns>
        public override int CountListItems(string listName)
        {
            var list = _clientContext.Web.Lists.GetByTitle(listName);

            _clientContext.Load(list, i => i.ItemCount);
            _clientContext.ExecuteQuery();

            return list.ItemCount;
        }
    }
}



