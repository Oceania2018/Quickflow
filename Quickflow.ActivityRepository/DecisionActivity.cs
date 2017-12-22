using CustomEntityFoundation;
using CustomEntityFoundation.Utilities;
using ExpressionEvaluator;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class DecisionActivity : IWorkflowActivity
    {
        public async Task Run(EntityDbContext dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            // find a first TURE statement
            bool decision = false;

            for (int i = 0; i < activity.Options.Count; i++)
            {
                var option = activity.Options[i];

                var cc = new CompiledExpression() { StringToParse = option.Value };
                object obj = activity.Input.ToObject<ExpandoObject>();
                cc.RegisterType("Input", obj);
                cc.RegisterDefaultTypes();
                decision = (bool)cc.Eval();

                if (decision)
                {
                    activity.NextActivityId = option.Key;
                    activity.Output.Data = activity.Input;
                    break;
                }
            }

            
        }
    }
}
