using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace requestProcessing
{
    static class Model
    {
        static public TicketClassesDataContext ticketClassesDataContext;
        static public Station GetStationFromString(string name)
        {
            var result = ticketClassesDataContext.ExecuteQuery<Station>(
                @"SELECT Id, Name, Lat, Lng
                FROM dbo.Station
                WHERE Name = {0}", name
            );
            return result.First();
        }

        static public Station[] GetAllStations()
        {
            var table = ticketClassesDataContext.GetTable<Station>();
            return table.ToArray();
        }
    }
}
