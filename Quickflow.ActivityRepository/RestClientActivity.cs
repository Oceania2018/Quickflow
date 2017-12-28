using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Quickflow.Core;
using Quickflow.Core.Entities;
using Quickflow.Core.Interfacess;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.ActivityRepository
{
    public class RestClientActivity : IWorkflowActivity
    {
        public async Task Run(Database dc, Workflow wf, ActivityInWorkflow activity, ActivityInWorkflow preActivity)
        {
            string baseUrl = WorkflowEngine.Configuration.GetSection(activity.GetOptionValue("baseUrl")).Value;
            string resource = activity.GetOptionValue("resource");

            var client = new RestClient(baseUrl);
            var request = new RestRequest(resource, Method.POST);
            request.AddParameter("application/json; charset=utf-8", "", ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            activity.Output.ErrorMessage = response.ErrorMessage;
            activity.Output.Data = JsonConvert.DeserializeObject(response.Content);
        }
    }
}
