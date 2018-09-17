using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quickflow.Core.Interfaces
{
    public interface IInitializationLoader
    {
        int Priority { get; }
        void Initialize(IConfiguration config, IHostingEnvironment env);
    }
}
