using EntityFrameworkCore.BootKit;
using ExpressionEvaluator;
using Newtonsoft.Json.Linq;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class DecisionActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            // find a first TURE statement
            bool decision = false;

            for (int i = 0; i < activity.Options.Count; i++)
            {
                var option = activity.Options[i];

                var cc = new CompiledExpression() { StringToParse = option.Value };
                object input = JObject.FromObject(activity.Input.Data).ToObject<ExpandoObject>();
                cc.RegisterType("Input", input);
                cc.RegisterDefaultTypes();
                decision = (bool)cc.Eval();

                if (decision)
                {
                    activity.NextActivityId = option.Key;
                    activity.Output = activity.Input;
                    break;
                }
            }

            
        }
    }
}
