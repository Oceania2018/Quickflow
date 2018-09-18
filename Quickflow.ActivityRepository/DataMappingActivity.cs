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
    public class DataMappingActivity : EssentialActivity, IWorkflowActivity
    {
        private static RazorLightEngine engine;

        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            var template = activity.GetOptionValue("Template");

            if (engine == null)
            {
                engine = new RazorLightEngineBuilder()
                  .UseFilesystemProject(AppDomain.CurrentDomain.GetData("ContentRootPath").ToString() + "\\App_Data")
                  .UseMemoryCachingProvider()
                  .Build();
            }

            var model = CleanJObject(activity);

            string result = "";

            var cacheResult = engine.TemplateCache.RetrieveTemplate(template);
            if (cacheResult.Success)
            {
                result = await engine.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), model);
            }
            else
            {
                result = await engine.CompileRenderAsync(template, model);
            }

            activity.Output.Data = JObject.Parse(result);
        }

        private JObject CleanJObject(ActivityInWorkflow activity)
        {
            // convert illegal variable name
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

            return jObject;
        }
    }
}
