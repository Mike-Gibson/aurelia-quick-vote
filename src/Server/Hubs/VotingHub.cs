using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebAPIApplication.Hubs
{
    [HubName("voting")]
    public class VotingHub : Hub
    {
        [HubMethodName("InvokedFromClient")]
        public void InvokedFromClient()
        {
            throw new Exception("HERE");
        } 
    }
}