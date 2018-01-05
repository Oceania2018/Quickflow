using EntityFrameworkCore.BootKit;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Quickflow.UnitTest
{
    public abstract class TestEssential
    {
        protected Database dc { get; set; }

        public TestEssential()
        {
            WorkflowEngine.ContentRootPath = $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..";
            WorkflowEngine.Assembles = new string[] { "Quickflow.Core", "Quickflow.ActivityRepository" };

            dc = new Database();

            dc.BindDbContext<IDbRecord, DbContext4Sqlite>(new DatabaseBind
            {
                MasterConnection = new SqliteConnection($"Data Source={WorkflowEngine.ContentRootPath}\\App_Data\\bootkit.db"),
                CreateDbIfNotExist = true,
                AssemblyNames = WorkflowEngine.Assembles
            });

            dc.DbTran(() => InitTestData());
        }

        private void InitTestData()
        {
            Directory.GetFiles(WorkflowEngine.ContentRootPath + "\\App_Data\\DbInitializer", "*.Workflows.json")
                .ToList()
                .ForEach(path =>
                {
                    string json = File.ReadAllText(path);
                    var dbContent = JsonConvert.DeserializeObject<JObject>(json);

                    if (dbContent["workflows"] != null)
                    {
                        DataInitialization.InitWorkflows(dc, dbContent["workflows"].ToList());
                    }

                });
        }
    }
}
