using EntityFrameworkCore.BootKit;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class LoopActivity : IWorkflowActivity
    {
        public Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            string startActivityId = activity.GetOptionValue("startActivityId");
            string endActivityId = activity.GetOptionValue("endActivityId");

            // reset input
            activity.Input = activity.OriginInput;

            if (activity.Input.Data.GetType() == typeof(JArray))
            {
                var list = (activity.Input.Data as JArray).ToList();

                // loop item
                if (activity.Flag < list.Count)
                {
                    Console.WriteLine($"loop index: {activity.Flag}");

                    wf.Activities.Find(x => x.Id == endActivityId).NextActivityId = activity.Id;

                    activity.NextActivityId = startActivityId;
                    activity.Output.Data = list[activity.Flag];

                    activity.Flag++;
                }
                // return to main flow
                else
                {
                    Console.WriteLine($"loop ended");

                    activity.Flag = 0;
                    activity.NextActivityId = activity.OriginNextActivityId;
                    activity.Output = activity.Input;
                }
            }

            return Task.CompletedTask;
        }
    }
}
