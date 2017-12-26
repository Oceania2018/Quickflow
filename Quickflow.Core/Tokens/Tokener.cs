using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Quickflow.Core.Tokens
{
    public class Tokener
    {
        public static string Replace(Object obj, string text)
        {
            if (!text.Contains("@Input.")) return text;

            // Replace Token
            if (obj.GetType() == typeof(JObject))
            {
                var jObject = obj as JObject;

                dynamic eObject = jObject.ToObject<ExpandoObject>();
                
                (eObject as IDictionary<string, object>).Keys.ToList()
                    .ForEach(key =>
                    {
                        text = text.Replace($"@Input.{key}", jObject[key]?.ToString());
                    });
            }
            else
            {
                var properties = obj.GetType().GetProperties();

                properties.ToList().ForEach(x =>
                {
                    text = text.Replace($"@Input.{x.Name}", x.GetValue(obj)?.ToString());
                });
            }
            

            

            return text;
        }
    }
}
