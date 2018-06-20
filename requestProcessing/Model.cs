using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace requestProcessing {
    public static class Model {

        [Serializable]
        public class TicketData {
            public string StartStation { get; set; }
            public string EndStation { get; set; }
            public int Distance { get; set; }
        }

        static public TicketClassesDataContext ticketClassesDataContext;
        static public Station GetStationFromString(string name) {
            var result = ticketClassesDataContext.ExecuteQuery<Station>(
                @"SELECT Id, Name, Lat, Lng
                FROM dbo.Station
                WHERE Name = {0}", name
            );
            return result.First();
        }

        static public Station[] GetAllStations() {
            //var table = ticketClassesDataContext.GetTable<Station>();
            //return table.ToArray();
            var result = ticketClassesDataContext.ExecuteQuery<Station>(
                @"SELECT * FROM dbo.Station"
            );
            return result.ToArray();
        }

        static public string GetStationName(int id) {
            var result = ticketClassesDataContext.ExecuteQuery<Station>(
                @"SELECT Id, Name
                FROM dbo.Station
                WHERE Id = {0}", id
            );

            var array = result.ToArray();

            return array[0].Name;
        }

        static public int GetUserByLogin(string name) {
            var result = ticketClassesDataContext.ExecuteQuery<User>(
                @"SELECT Id
                FROM dbo.[User]
                WHERE Login = {0}", name
            );

            var array = result.ToArray();

            if (array.Length == 0) {
                return -1;
            } else {
                return array[0].Id;
            }
        }

        static public void AddUser(string login, string password) {
            ticketClassesDataContext.Users.InsertOnSubmit(new User { Login = login, Password = Convert.FromBase64String(password) });
            ticketClassesDataContext.SubmitChanges();
        }

        static public bool Authenticate(string login, string password) {
            var result = ticketClassesDataContext.ExecuteQuery<User>(
                @"SELECT Id
                FROM dbo.[User]
                WHERE Login = {0} AND Password = {1}", login, Convert.FromBase64String(password)
            );

            var array = result.ToArray();

            if (array.Length == 0) {
                return false;
            } else {
                return true;
            }
        }

        static public void AddTicket(string userName, string startStation, string endStation) {
            var userId = GetUserByLogin(userName);
            var stationA = GetStationFromString(startStation);
            var stationB = GetStationFromString(endStation);

            ticketClassesDataContext.Tickets.InsertOnSubmit(new Ticket { StartId = stationA.Id, DestinationId = stationB.Id, UserId = userId });
            ticketClassesDataContext.SubmitChanges();
        }

        static public TicketData[] GetTicketsByUser(int userId) {
            var result = ticketClassesDataContext.ExecuteQuery<Ticket>(
                @"SELECT Id, StartId, DestinationId, UserId
                FROM dbo.Ticket
                WHERE UserId = {0}", userId
             );

            var temp = result.ToArray();

            List<TicketData> tickets = new List<TicketData>();

            foreach (var t in temp) {
                tickets.Add(new TicketData { StartStation = GetStationName(t.StartId), EndStation = GetStationName(t.DestinationId)});
            }

            return tickets.ToArray();
        }
    }
}
