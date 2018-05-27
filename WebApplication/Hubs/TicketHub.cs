using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace WebApplication.Hubs
{
    public class TicketHub : Hub {
        public override async Task OnConnectedAsync() {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex) {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "left");
        }

        public async Task RequestTickets(string username) {
            string request = "GetTicketsByUser;";
            request += username;

            var response = Controllers.TicketController.Communicate(request);

            requestProcessing.Model.TicketData[] tickets;
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(response)) {
                tickets = (requestProcessing.Model.TicketData[])formatter.Deserialize(stream);
            }
            
            await Clients.Client(Context.ConnectionId).SendAsync("SendTickets", tickets);
        }

        public async Task Send(string message) {
            await Clients.Client(Context.ConnectionId).SendAsync("SendTickets", message);
        }
    }
}
