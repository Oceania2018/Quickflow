using EntityFrameworkCore.BootKit;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.Core.Activities
{
    public class EmitCommandActivity : EssentialActivity, IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            await SignalHub.All.SendCoreAsync("ReceiveMessage", new object[] { preActivity.Output.Data });
        }
    }
}
