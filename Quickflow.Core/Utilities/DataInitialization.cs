using EntityFrameworkCore.BootKit;
using Newtonsoft.Json.Linq;
using Quickflow.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quickflow.Core.Utilities
{
    public class DataInitialization
    {
        public static void InitWorkflows(Database dc, List<JToken> jWorkflows)
        {
            jWorkflows.ToList().ForEach(bundle =>
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
