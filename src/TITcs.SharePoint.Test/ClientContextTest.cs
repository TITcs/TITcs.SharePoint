using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Query;

namespace TITcs.SharePoint.Test
{
    [TestFixture]
    public class ClientContextTest
    {
        [Test]
        public void ListAll()
        {
            var login = @"tw.q|tit\stiven.camara";
            var parts = login.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            login = parts[parts.Length - 1];


            //using (var query = ContextFactory.GetContextSite())
            //{
            //    var camlQuery = "";//"<Eq><FieldRef Name='ID' /><Value Type='Counter'>1</Value></Eq>";

            //    var noticiaQuery = query.Load<Pagina>("Páginas", 1000, camlQuery, i => i.Title);

            //    query.Execute();

            //    var paginas = noticiaQuery().ToList();

            //    Assert.IsTrue(true);
            //}
        }
    }

}
