using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Text;
using CustomEntityFoundation;
using Quickflow.Core.Entities;
using System.Threading.Tasks;
using CustomEntityFoundation.Bundles;
using Newtonsoft.Json.Linq;
using System.Linq;
using Quickflow.Core;
using Microsoft.EntityFrameworkCore;

namespace Quickflow.ActivityRepository.CustomEntityFoundation
{
    public class InsertBundleEntityActivity : IWorkflowActivity
    {
        public Task Run(EntityDbContext dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            var bundle = dc.Bundle.Include(x => x.Fields).FirstOrDefault(x => x.Id == activity.GetOptionValue("bundleId"));
            activity.Output.Data = bundle.AddRecord(dc, JObject.FromObject(activity.Input));

            return Task.CompletedTask;
        }
    }
}
