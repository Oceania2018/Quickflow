using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quickflow.Core
{
    public class SignalHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"connection: {Context.ConnectionId} established.");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"connection: {Context.ConnectionId} disconnected.");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
