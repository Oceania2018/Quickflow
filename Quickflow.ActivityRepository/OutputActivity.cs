using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class OutputActivity : EssentialActivity, IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            activity.Output.Data = activity.Input.Data;
            Console.WriteLine(activity.GetOptionValue("text"));
            Console.WriteLine(JsonConvert.SerializeObject(activity.Output.Data));
        }
    }
}
