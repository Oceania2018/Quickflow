using CustomEntityFoundation;
using CustomEntityFoundation.Entities;
using CustomEntityFoundation.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickflow.RestApi
{
    [Produces("application/json")]
    [Route("qf/[controller]")]
    public abstract class WorkflowController : ControllerBase
    {
        [HttpPost("run/{workflowId}")]
        public async Task<string> Run([FromRoute] string workflowId, [FromBody] JObject data)
        {
            DateTime startWf = DateTime.Now;

            var dc = new EntityDbContext();
            dc.InitDb();

            var wf = new WorkflowEngine { WorkflowId = workflowId };

            dc.Transaction<IDbRecord>(async delegate
            {
                await wf.Run(dc, data);
            });

            Console.WriteLine("");
            Console.WriteLine($"------ {workflowId.ToUpper()} spent {(DateTime.Now - startWf).TotalSeconds}s ------");

            return dc.TransactionId;
        }

        
    }

}
