using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TITcs.SharePoint.Data.ContentTypes;
using TITcs.SharePoint.Repository;

namespace TITcs.SharePoint.Test
{
    public class DemoSqlRepository : SqlServerRepository
    {
        public DemoSqlRepository()
            : base("Demo")
        {
        }

        public ICollection<Item> GetAll()
        {
            var dataSet = Execute("dbo.SP_Demo_GetAll");

            return (from DataRow row in dataSet.Tables[0].Rows
                select new Item
                {
                    ID = GetInt32(row["ID"]),
                    Title = GetString(row["Title"])
                }).ToList();
        }

        public Item GetById(int id)
        {
            var dataSet = Execute("dbo.SP_Demo_GetById", 
                CreateParameter("@ID", id));


            var row = dataSet.Tables[0].Rows.Cast<DataRow>().SingleOrDefault(i => GetInt32(i["ID"]) == id);

            if (row == null)
                throw new Exception(string.Format("Invalid id \"{0}\"", id));

            return new Item
            {
                ID = GetInt32(row["ID"]),
                Title = GetString(row["Title"])
            };
        }
    }
}
