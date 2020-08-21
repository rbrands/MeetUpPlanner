using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MeetUpPlanner.Server.Hubs
{
    /// <summary>
    /// Implements the SignalR-Hub to notify all client except the caller about a change
    /// </summary>
    /// <seealso cref="https://www.c-sharpcorner.com/article/easily-create-a-real-time-application-with-blazor-and-signalr/"/>
    public class BroadcastHub : Hub
    {
        public async Task SendMessage()
        {
            await Clients.Others.SendAsync("ReceiveMessage");
        }
    }
}
