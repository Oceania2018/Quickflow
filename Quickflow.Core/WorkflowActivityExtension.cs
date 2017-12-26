using Quickflow.Core.Entities;
using Quickflow.Core.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.Core
{
    public static class WorkflowActivityExtension
    {
        public static string GetOptionValue(this ActivityInWorkflow activity, string key)
        {
            String text = activity.Options.Find(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))?.Value;

            if (!String.IsNullOrEmpty(text))
            {
                text = Tokener.Replace(activity.Input.Data, text);
            }

            return text;
        }
    }
}
