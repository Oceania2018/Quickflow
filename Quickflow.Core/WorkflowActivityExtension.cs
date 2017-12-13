using Quickflow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.Core
{
    public static class WorkflowActivityExtension
    {
        public static string GetOptionValue(this ActivityInWorkflow activity, string key)
        {
            return activity.Options.Find(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))?.Value;
        }
    }
}
