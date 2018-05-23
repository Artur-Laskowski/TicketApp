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
            Model.ticketClassesDataContext = new TicketClassesDataContext();

            /*
            var stationRows = ticketClassesDataContext.GetTable<Station>();

            IQueryable<Station> l = from s in stationRows select s;
            
            foreach (var row in stationRows)
            {
                Console.WriteLine($"Id: {row.Id}, name: {row.Name}, lat: {row.Lat}, lng: {row.Lng}");
                foreach (var row2 in stationRows) {
                    Console.WriteLine($"Distance to {row2.Name}: {GetDistance(row, row2):N2}km");
                }
            }
            */

            using (var client = new ResponseSocket()) {
                client.Bind("tcp://*:5555");

                while (true) {
                    Console.WriteLine("waiting");

                    var message = client.ReceiveFrameString();
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
                    }

                    Thread.Sleep(1000);
                    //Send an answer

                    Console.WriteLine("Sending: " + Encoding.ASCII.GetString(answer));

                    client.SendFrame(answer);

                }

            }

            Console.WriteLine("Finished");
            Console.ReadKey();
        }
    }
}
