using EntityFrameworkCore.BootKit;
using Newtonsoft.Json.Linq;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using RazorLight;
using System;
using System.Collections.Generic;
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

            var result = await engine.CompileRenderAsync(template, activity.Input.Data);
            activity.Output.Data = JObject.Parse(result);
        }
    }
}
