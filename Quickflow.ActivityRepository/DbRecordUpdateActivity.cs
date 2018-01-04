using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using Quickflow.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class DbRecordUpdateActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            string json = "{" + activity.GetOptionValue("values") + "}";
            var values = JObject.FromObject(JsonConvert.DeserializeObject(json));

            var dic = (activity.Input.Data as JObject).ToDictionary();

            values.Properties().ToList().ForEach(p => dic[p.Name] = p.Value);

            var paramters = activity.GetOptionValue("params");

            (activity.Input.Data as JObject).Properties()
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
                Id = activity.GetOptionValue("id"),
                Values = dic
            };
            
            dc.Patch<IDbRecord>(patch);

            activity.Output = activity.Input;
        }
    }
}
