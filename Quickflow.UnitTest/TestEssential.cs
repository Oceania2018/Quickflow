using EntityFrameworkCore.BootKit;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            AppDomain.CurrentDomain.SetData("ContentRootPath", $"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..");
            AppDomain.CurrentDomain.SetData("Assemblies", new string[] { "Quickflow.Core", "Quickflow.ActivityRepository", "Quickflow.UnitTest" });

            dc = new Database();

            dc.BindDbContext<IDbRecord, DbContext4SqlServer>(new DatabaseBind
            {
                MasterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Quickflow;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"),
                CreateDbIfNotExist = true
            });

            dc.DbTran(() => InitTestData());
        }

        private void InitTestData()
        {
            Directory.GetFiles(AppDomain.CurrentDomain.GetData("ContentRootPath").ToString() + "\\App_Data\\DbInitializer", "*.Workflows.json")
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
