using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace requestProcessing {
    static class Controller {
        static public double CalculateDistance(Station start, Station end) {
            double latDst = 110.574 * Math.Abs(end.Lat - start.Lat);
            double lngDst = 111.320 * Math.Cos((end.Lat + start.Lat) / 2 / 180 * Math.PI) * Math.Abs(end.Lng - start.Lng);
            return Math.Sqrt(latDst * latDst + lngDst * lngDst);
        }

        static public byte[] GetDistance(string[] vs) {
            Station start = Model.GetStationFromString(vs[1]);
            Station end = Model.GetStationFromString(vs[2]);

            double distance = CalculateDistance(start, end);

            return BitConverter.GetBytes(distance);
        }

        static public byte[] GetAllStationNames(string[] vs) {
            StringBuilder names = new StringBuilder();
            Station[] stations = Model.GetAllStations();
            foreach (var s in stations) {
                names.Append(s.Name);
                names.Append(";");
            }
            names.Remove(names.Length - 1, 1);

            return Encoding.ASCII.GetBytes(names.ToString());
        }

        static public byte[] GetUserByLogin(string[] vs) {
            var result = Model.GetUserByLogin(vs[1]);
            return BitConverter.GetBytes(result);
        }

        static public byte[] AddUser(string[] vs) {
            Model.AddUser(vs[1], vs[2]);

            return BitConverter.GetBytes(true);
        }

        static public byte[] Authenticate(string[] vs) {
            var userId = Model.GetUserByLogin(vs[1]);

            bool success = false;

            if (userId != -1) {
                success = Model.Authenticate(vs[1], vs[2]);
            }

            return BitConverter.GetBytes(success);
        }
    }
}
