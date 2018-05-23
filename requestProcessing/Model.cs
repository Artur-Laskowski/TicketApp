using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace requestProcessing {
    static class Model {
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
            var table = ticketClassesDataContext.GetTable<Station>();
            return table.ToArray();
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
    }
}
