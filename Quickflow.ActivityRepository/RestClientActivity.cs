using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using Quickflow.Core.Utilities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class RestClientActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            string baseUrl = activity.GetOptionValue("baseUrl");
            string resource = activity.GetOptionValue("resource");
            string json = JsonConvert.SerializeObject(activity.Input.Data);

            var client = new RestClient(baseUrl);
            var request = new RestRequest(resource, json.Length < 5 ? Method.GET : Method.POST);

            resource.Split("/").ToList().ForEach(seg =>
            {
                if (seg.StartsWith('{'))
                {
                    string name = seg.Substring(1, seg.Length - 2);
                    request.AddUrlSegment(name, activity.Input.Data.GetValue(name).ToString());
                }
            });

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            activity.Output.ErrorMessage = response.ErrorMessage;
            activity.Output.Data = JsonConvert.DeserializeObject(response.Content);
        }
    }
}
