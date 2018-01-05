using EntityFrameworkCore.BootKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickflow.Core
{
    public class WorkflowEngine
    {
        public String WorkflowId { get; set; }
        public String TransactionId { get; set; }
        public static String[] Assembles { get; set; }
        public static IConfiguration Configuration { get; set; }
        public static string ContentRootPath { get; set; }

        public async Task<ActivityResult> Run<TInput>(Database dc, TInput input)
        {
            DateTime dtStart = DateTime.UtcNow;

            var workflow = dc.Table<Workflow>()
                            .Include(x => x.Activities).ThenInclude(x => x.Options)
                            .FirstOrDefault(x => x.Id == WorkflowId);

            if(workflow == null)
            {
                Console.WriteLine($"Can't find workflow {WorkflowId}");
                return null;
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"------ {workflow.Name.ToUpper()}, TRACEID: {TransactionId} ------");
            Console.WriteLine($"{workflow.Description}");
            Console.WriteLine("");

            ConstructActivityLinkedlist(workflow);

            var types = TypeHelper.GetClassesWithInterface<IWorkflowActivity>(Assembles);

            ActivityInWorkflow preActivity = null;
            ActivityInWorkflow activity = workflow.Activities.First();
            activity.Input = new ActivityResult { Data = input };
            
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
                }

                var instance = (IWorkflowActivity)Activator.CreateInstance(type);

                activity.Output = new ActivityResult();

                // Keep original NextActivityId
                if (String.IsNullOrEmpty(activity.OriginNextActivityId))
                {
                    activity.OriginNextActivityId = activity.NextActivityId;
                }

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
                }

                // Halt workflow
                if (!String.IsNullOrEmpty(activity.Output.ErrorMessage))
                {
                    activity.NextActivityId = "";
                    Console.WriteLine($"{activity.Output.ErrorMessage}");
                    throw new Exception($"{activity.Output.ErrorMessage}");
                }

                Console.WriteLine("");
                Console.WriteLine($"{activity.ActivityName} spent {(DateTime.Now - start).TotalMilliseconds}ms");
                Console.WriteLine($"{JsonConvert.SerializeObject(activity.Output.Data)}");

                preActivity = activity;
                activity = workflow.Activities.FirstOrDefault(x => x.Id == activity.NextActivityId);

                if(activity != null)
                {
                    activity.Input = preActivity.Output;

                    // keep original input
                    if (activity.OriginInput == null)
                    {
                        activity.OriginInput = activity.Input;
                    }
                }
            }

            Console.WriteLine($"------ {workflow.Name.ToUpper()} Completed in {(DateTime.UtcNow - dtStart).TotalSeconds}s ------");

            return preActivity?.Output;
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
