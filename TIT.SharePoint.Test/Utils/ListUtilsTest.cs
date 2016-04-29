using Microsoft.SharePoint;
using NUnit.Framework;
using TITcs.SharePoint.Query;
using TITcs.SharePoint.Utils;

namespace TITcs.SharePoint.Test.Utils
{
    [TestFixture]
    public class ListUtilsTest
    {
        [Test]
        public void ChangeTitle_In_List() { 

           using (var query = ContextFactory.GetContextSite())
           {
               var context = query.Context as SPContext;

                ListUtils.ChangeTitle(context.Web, "Demo", "Demo 1");

               Assert.IsTrue(true);
            }

        }
    }
}
