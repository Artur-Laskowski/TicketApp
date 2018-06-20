using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using NetMQ.Sockets;
using NetMQ;
using System.Threading;

namespace requestProcessing {
    class Program {
        static void Main(string[] args) {

            using (var receiver = new PullSocket(">tcp://localhost:5557"))
            using (var sender = new PushSocket(">tcp://localhost:5558"))
            using (Model.ticketClassesDataContext = new TicketClassesDataContext("Server=(localdb)\\ProjectsV13;Database=TicketsDatabase;")) {

                Model.ticketClassesDataContext.Connection.Open();
                var result = Model.ticketClassesDataContext.ExecuteQuery<Station>(
                    @"SELECT * FROM dbo.Station"
                );
                foreach (var r in result) ;

                while (true) {
                    Console.WriteLine("waiting");
                    
                    var message = receiver.ReceiveFrameString();
                    string[] vs = message.Split(';');
                    Console.WriteLine("Received {0}", message);

                    byte[] answer = { };
                    switch (vs[0]) {
                        case "GetDistance":
                            answer = Controller.GetDistance(vs);
                            break;
                        case "GetAllStationNames":
                            answer = Controller.GetAllStationNames(vs);
                            break;
                        case "CheckIfLoginAvailable":
                            answer = Controller.GetUserByLogin(vs);
                            break;
                        case "AddUser":
                            answer = Controller.AddUser(vs);
                            break;
                        case "Authenticate":
                            answer = Controller.Authenticate(vs);
                            break;
                        case "BuyTicket":
                            answer = Controller.BuyTicket(vs);
                            break;
                        case "GetTicketsByUser":
                            answer = Controller.GetTicketsByUser(vs);
                            break;
                    }

                    Thread.Sleep(1000);
                    //Send an answer

                    //Console.WriteLine("Sending: " + Encoding.ASCII.GetString(answer));

                    sender.SendFrame(answer);
                }

            }
        }
    }
}
