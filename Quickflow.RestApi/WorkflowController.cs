using EntityFrameworkCore.BootKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Quickflow.RestApi
{
    /// <summary>
    /// Workflow controller
    /// </summary>
#if !DEBUG
    [Authorize]
#endif
    [Produces("application/json")]
    [Route("qf/[controller]")]
    public class WorkflowController : ControllerBase
    {
        /// <summary>
        /// Run workflow
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("run/{workflowId}")]
        public async Task<string> Run([FromRoute] string workflowId, [FromBody] JObject data)
        {
            var dc = new DefaultDataContextLoader().GetDefaultDc();

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
