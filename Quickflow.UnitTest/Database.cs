using CustomEntityFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quickflow.UnitTest
{
    public class Database
    {
        public static EntityDbContext GetDatabase()
        {
            EntityDbContext.Assembles = new String[] { "Quickflow.Core", "Quickflow.ActivityRepository" };
            var options = new DatabaseOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..",
            };

            // Sqlite
            options.Database = "Sqlite";
            options.ConnectionString = "Data Source=|DataDirectory|\\Voicecoin.db";
            EntityDbContext.Options = options;

            var dc = new EntityDbContext();
            dc.InitDb();

            return dc;
        }
    }
}
