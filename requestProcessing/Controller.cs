using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace requestProcessing
{
    static class Controller
    {
        static public double CalculateDistance(Station start, Station end)
        {
            double latDst = 110.574 * Math.Abs(end.Lat - start.Lat);
            double lngDst = 111.320 * Math.Cos((end.Lat + start.Lat) / 2 / 180 * Math.PI) * Math.Abs(end.Lng - start.Lng);
            return Math.Sqrt(latDst * latDst + lngDst * lngDst);
        }

        static public byte[] GetDistance(string[] vs)
        {
            Station start = Model.GetStationFromString(vs[1]);
            Station end = Model.GetStationFromString(vs[2]);

            double distance = CalculateDistance(start, end);

            return BitConverter.GetBytes(distance);
        }

        static public byte[] GetAllStationNames(string[] vs)
        {
            StringBuilder names = new StringBuilder();
            Station[] stations = Model.GetAllStations();
            foreach (var s in stations)
            {
                names.Append(s.Name);
                names.Append(";");
            }

            Console.WriteLine("Sending: " + names.ToString());

            return Encoding.ASCII.GetBytes(names.ToString());
        }
    }
}
