using DotNetToolkit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Quickflow.Core.Interfaces;
using System;
using System.Linq;

namespace Quickflow.WebHost
{
    public class InitializationLoader
    {
        public IHostingEnvironment Env { get; set; }
        public IConfiguration Config { get; set; }
        public void Load()
        {
            var assemblies = (string[])AppDomain.CurrentDomain.GetData("Assemblies");
            var appsLoaders = TypeHelper.GetInstanceWithInterface<IInitializationLoader>(assemblies);
            appsLoaders.ForEach(loader =>
            {
                loader.Initialize(Config, Env);
            });
        }
    }
}
