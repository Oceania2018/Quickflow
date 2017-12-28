using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Quickflow.RestApi
{
#if !DEBUG
    [Authorize]
#endif
    [Produces("application/json")]
    [Route("qf/[controller]")]
    public abstract class WorkflowController : ControllerBase
    {
        protected Database dc { get; set; }
        
        public WorkflowController()
        {
            var dc = new Database();

            string db = WorkflowEngine.Configuration.GetSection("Database:Default").Value;
            string connectionString = WorkflowEngine.Configuration.GetSection("Database:ConnectionStrings")[db];

            if (db.Equals("SqlServer"))
            {
                dc.BindDbContext<IDbRecord, DbContext4SqlServer>(new DatabaseBind
                {
                    MasterConnection = new SqlConnection(connectionString),
                    CreateDbIfNotExist = true,
                    AssemblyNames = WorkflowEngine.Assembles
                });
            }
            else if (db.Equals("Sqlite"))
            {
                connectionString = connectionString.Replace("|DataDirectory|\\", WorkflowEngine.ContentRootPath + "\\App_Data\\");
                dc.BindDbContext<IDbRecord, DbContext4Sqlite>(new DatabaseBind
                {
                    MasterConnection = new SqliteConnection(connectionString),
                    CreateDbIfNotExist = true,
                    AssemblyNames = WorkflowEngine.Assembles
                });
            }
            else if (db.Equals("MySql"))
            {
                dc.BindDbContext<IDbRecord, DbContext4MySql>(new DatabaseBind
                {
                    MasterConnection = new MySqlConnection(connectionString),
                    CreateDbIfNotExist = true,
                    AssemblyNames = WorkflowEngine.Assembles
                });
            }
            else if (db.Equals("InMemory"))
            {
                dc.BindDbContext<IDbRecord, DbContext4Memory>(new DatabaseBind
                {
                    AssemblyNames = WorkflowEngine.Assembles
                });
            }
        }

        [HttpPost("run/{workflowId}")]
        public async Task<string> Run([FromRoute] string workflowId, [FromBody] JObject data)
        {
            var wf = new WorkflowEngine
            {
                WorkflowId = workflowId,
                TransactionId = Guid.NewGuid().ToString(),
            };

            dc.Transaction<IDbRecord>(async delegate
            {
                await wf.Run(dc, data);
            });

            return wf.TransactionId;
        }
    }
}
