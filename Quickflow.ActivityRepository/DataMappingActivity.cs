using EntityFrameworkCore.BootKit;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class DataMappingActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            var template = activity.GetOptionValue("Template");

            var engine = new RazorLightEngineBuilder()
              .UseFilesystemProject(WorkflowEngine.ContentRootPath + "\\App_Data")
              .UseMemoryCachingProvider()
              .Build();

            var jObject = JObject.FromObject(activity.Input.Data);
            jObject.Properties().ToList().ForEach(p => {
                char letter = p.Name[0];
                int i = letter - '0';
                if (i >= 0 && i <= 9)
                {
                    jObject.Add('n' + p.Name, p.Value);
                    jObject.Remove(p.Name);
                }
            });

            var result = await engine.CompileRenderAsync(template, jObject);
            activity.Output.Data = JObject.Parse(result);
        }
    }
}
