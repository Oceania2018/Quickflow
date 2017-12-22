using CustomEntityFoundation;
using CustomEntityFoundation.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickflow.Core
{
    public class WorkflowEngine
    {
        public String WorkflowId { get; set; }

        public async Task Run<TInput>(EntityDbContext dc, TInput input)
        {
            var workflow = dc.Table<Workflow>()
                            .Include(x => x.Activities).ThenInclude(x => x.Options)
                            .FirstOrDefault(x => x.Id == WorkflowId);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"------ {workflow.Name.ToUpper()}, TRACEID: {dc.TransactionId} ------");
            Console.WriteLine($"{workflow.Description}");
            Console.WriteLine("");

            ConstructActivityLinkedlist(workflow);

            var types = TypeHelper.GetClassesWithInterface<IWorkflowActivity>(EntityDbContext.Assembles);

            ActivityInWorkflow preActivity = null;
            ActivityInWorkflow activity = workflow.Activities.First();
            activity.Input = input;
            

            int step = 1;
            while (activity != null)
            {
                Console.WriteLine("");
                Console.WriteLine($"--- STEP {step++}: {activity.ActivityName} ---");

                Console.WriteLine($"{String.Join("", activity?.Options)}");
                DateTime start = DateTime.Now;
                var type = types.FirstOrDefault(x => x.Name.Equals(activity.ActivityName, StringComparison.CurrentCultureIgnoreCase));

                if(type == null)
                {
                    Console.WriteLine($"Can't find activity: {activity.ActivityName}");
                    return;
                }

                var instance = (IWorkflowActivity)Activator.CreateInstance(type);

                activity.Output = new ActivityResult();

                try
                {
                    await instance.Run(dc, workflow, activity, preActivity);
                }
                catch (Exception ex)
                {
                    if (String.IsNullOrEmpty(activity.Output.ErrorMessage))
                    {
                        activity.Output.ErrorMessage = ex.Message;
                    }

                    throw ex;
                }

                Console.WriteLine("");
                Console.WriteLine($"{activity.ActivityName} spent {(DateTime.Now - start).TotalMilliseconds}ms");
                Console.WriteLine($"{activity.Output.ErrorMessage}");
                Console.WriteLine($"{JsonConvert.SerializeObject(activity.Output.Data)}");

                preActivity = activity;
                activity = workflow.Activities.FirstOrDefault(x => x.Id == activity.NextActivityId);

                if(activity != null)
                {
                    activity.Input = preActivity.Output;
                }
            }
        }

        private void ConstructActivityLinkedlist(Workflow workflow)
        {
            var activities = new List<ActivityInWorkflow>();
            string nextActivityId = workflow.RootActivityId;

            while (workflow.Activities.Count > 0)
            {
                var currentActivity = workflow.Activities.FirstOrDefault(x => x.Id == nextActivityId);

                if (currentActivity == null)
                {
                    currentActivity = workflow.Activities.FirstOrDefault();
                }

                activities.Add(currentActivity);
                workflow.Activities.Remove(currentActivity);

                nextActivityId = currentActivity.NextActivityId;
            }

            workflow.Activities = activities;
        }
    }
}
