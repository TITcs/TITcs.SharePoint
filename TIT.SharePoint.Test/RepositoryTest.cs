using System;
using System.Collections.Generic;
using NUnit.Framework;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Repository;

namespace TITcs.SharePoint.Test
{
    [TestFixture]
    public class RepositoryTest
    {
        [Test]
        public void GetAllItems()
        {
            var repository = new DemoRepository();

            var items = repository.GetAll();

            Assert.IsTrue(items.Count > 0);
        }

        /// <summary>
        /// </summary>
        [Test]
        public void GetAllItemsByAuthor_DisplayName()
        {
            var repository = new DemoRepository();

            var items = repository.GetAllByAuthor("Raul Fuentes");

            Assert.IsTrue(items.Count > 0);
        }

        [Test]
        public void GetAllItemsByAuthor_Id()
        {
            var repository = new DemoRepository();

            var items = repository.GetAllByAuthor(17);

            Assert.IsTrue(items.Count > 0);
        }

        /// <summary>
        /// </summary>
        [Test]
        public void GetAllItemsByUser()
        {
            var repository = new DemoRepository();

            var items = repository.GetAllByUser(17);

            Assert.IsTrue(items.Count > 0);
        }

        [Test]
        public void GetAllItemsByCreated()
        {
            var repository = new DemoRepository();

            var items = repository.GetAllByCreated(DateTime.Today);

            Assert.IsTrue(items.Count > 0);
        }

        [Test]
        public void InsertItem()
        {
            var repository = new DemoRepository();

            var item = new Item();

            item.Title = "Item " + DateTime.Now.Ticks;

            item = repository.Insert(item);

            Assert.IsTrue(item.ID > 0);
        }

        [Test]
        public void UpdateItem()
        {
            var repository = new DemoRepository();

            var item = new Item();

            item.ID = 3;

            item.Title = "Item Updated " + DateTime.Now.Ticks;

            item = repository.Update(item);

            Assert.IsTrue(item.Title.IndexOf("Updated") > -1);
        }

        [Test]
        public ICollection<Item> GetAll_By_Load()
        {
            return null;

        }
    }
}
