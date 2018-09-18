using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.Core
{
    public class EssentialActivity
    {
        public IHubClients SignalHub { get; set; }
    }
}
