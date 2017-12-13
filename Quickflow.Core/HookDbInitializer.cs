using CustomEntityFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Quickflow.Core.Entities;

namespace Quickflow.Core
{
    public class HookDbInitializer : IHookDbInitializer
    {
        public int Priority => 100;

        public void Load(IConfiguration config, EntityDbContext dc)
        {
            Directory.GetFiles(EntityDbContext.Options.ContentRootPath + "\\App_Data\\DbInitializer", "*.Workflows.json")
                .ToList()
                .ForEach(path =>
                {
                    string json = File.ReadAllText(path);
                    var dbContent = JsonConvert.DeserializeObject<JObject>(json);

                    if (dbContent["workflows"] != null)
                    {
                        InitWorkflows(dc, dbContent["workflows"].ToList());
                    }

                });
        }

        private void InitWorkflows(EntityDbContext dc, List<JToken> jBundles)
        {
            jBundles.ToList().ForEach(bundle =>
            {
                var workflow = bundle.ToObject<Workflow>();

                if (!dc.Table<Workflow>().Any(X => X.Id == workflow.Id))
                {
                    if (workflow.Activities != null)
                    {
                        workflow.Activities.ForEach(activity =>
                        {
                            if (activity.Configuration != null)
                            {
                                activity.Options = new List<OptionsInActivity>();
                                activity.Configuration.Children().ToList().ForEach(config =>
                                {
                                    var option = new OptionsInActivity
                                    {
                                        Key = config.Path,
                                        Value = config.Values().FirstOrDefault()?.ToString(),
                                    };

                                    activity.Options.Add(option);
                                });
                            }
                        });
                    }

                    dc.Table<Workflow>().Add(workflow);
                }

            });
        }
    }
}
