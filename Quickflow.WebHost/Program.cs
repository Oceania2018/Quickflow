using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Quickflow.WebHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    string dir = Path.GetFullPath(env.ContentRootPath + "/..");
                    var settings = Directory.GetFiles(Path.Combine(dir, "Settings"), "*.json");
                    settings.ToList().ForEach(setting =>
                    {
                        config.AddJsonFile(setting, optional: false, reloadOnChange: true);
                    });
                })
                .UseUrls("http://0.0.0.0:3112")
                .UseStartup<Startup>()
                .Build();
    }
}
