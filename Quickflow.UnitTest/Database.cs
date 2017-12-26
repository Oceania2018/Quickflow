using CustomEntityFoundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quickflow.UnitTest
{
    public abstract class Database
    {
        protected EntityDbContext dc { get; set; }

        public Database()
        {
            EntityDbContext.Assembles = new String[] { "Quickflow.Core", "Quickflow.ActivityRepository" };
            var options = new DatabaseOptions
            {
                ContentRootPath = Directory.GetCurrentDirectory() + "\\..\\..\\..",
            };

            // Sqlite
            options.Database = "Sqlite";
            options.ConnectionString = "Data Source=|DataDirectory|\\quickflow.db";
            EntityDbContext.Options = options;

            dc = new EntityDbContext();
            dc.InitDb();

            dc.DbTran(() => InitTestData());
        }

        private void InitTestData()
        {
            new Quickflow.Core.HookDbInitializer().Load(null, dc);
        }
    }
}
