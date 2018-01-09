using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class DbRecordUpdateActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            string json = "{" + activity.GetOptionValue("values") + "}";
            var values = JObject.Parse(json);

            var dic = JObject.FromObject(activity.Input.Data).ToDictionary();

            dic.ToList().ForEach(p => values[p.Key] = JToken.FromObject(p.Value));

            var paramters = activity.GetOptionValue("params");
            var createIfNotExists = bool.Parse(activity.GetOptionValue("createIfNotExists") ?? "false");

            JObject.FromObject(activity.Input.Data).Properties()
                .ToList()
                .ForEach(d =>
                {
                    if (paramters.StartsWith("{"))
                    {
                        paramters = paramters.Replace("{" + d.Name + "}", d.Value.ToString());
                        dic.Remove(d.Name);
                    }
                });
            
            var patch = new DbPatchModel
            {
                Table = activity.GetOptionValue("table"),
                Where = activity.GetOptionValue("where"),
                Params = new object[] { paramters },
                Values = dic
            };

            // check if exists
            if (dc.Table(patch.Table).Any(patch.Where, patch.Params))
            {
                dc.Patch<IDbRecord>(patch);
            }
            else
            {
                var tableType = TypeHelper.GetType(patch.Table, Database.Assemblies);
                dc.Add(values.ToObject(tableType));
            }

            activity.Output = activity.Input;
        }
    }
}
