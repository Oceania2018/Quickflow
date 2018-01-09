using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            string method = activity.GetOptionValue("method")?.ToUpper() ?? "GET";
            string querys = activity.GetOptionValue("query");
            string json = JsonConvert.SerializeObject(activity.Input.Data);

            var request = new RestRequest(resource);
            
            var jObject = JObject.FromObject(activity.Input.Data);

            // add url segment
            resource.Split("/").ToList().ForEach(seg =>
            {
                if (!String.IsNullOrEmpty(seg) && seg.StartsWith('{'))
                {
                    string name = seg.Substring(1, seg.Length - 2);
                    request.AddUrlSegment(name, jObject[name]);
                }
            });

            // add query parameter
            querys.Split("&").ToList().ForEach(seg =>
            {
                var query = seg.Split("=");
                if (query.Length == 2)
                {
                    if (query[1].StartsWith('{'))
                    {
                        string name = query[1].Substring(1, query[1].Length - 2);
                        request.AddQueryParameter(query[0], jObject[name].ToString());
                    }
                    else
                    {
                        request.AddQueryParameter(query[0], query[1]);
                    }
                }
            });

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            request.Method = Enum.Parse<Method>(method);

            var client = new RestClient(baseUrl);
            IRestResponse response = client.Execute(request);

            activity.Output.ErrorMessage = response.ErrorMessage;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                activity.Output.Data = JsonConvert.DeserializeObject(response.Content);
            }
            else
            {
                activity.Output.ErrorMessage = activity.Output.ErrorMessage ?? response.StatusDescription;
            }
        }
    }
}
