using CustomEntityFoundation;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class OutputActivity : IWorkflowActivity
    {
        public async Task Run(EntityDbContext dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            Console.WriteLine(activity.GetOptionValue("text"));
        }
    }
}
