using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;
using Microsoft.AspNetCore.SignalR;

namespace WebApplication.Controllers {
    public class TicketController : Controller {


        static private Object commsLock = new Object();
        static PushSocket sender = new PushSocket("@tcp://*:5557");
        static PullSocket responseReceiver = new PullSocket("@tcp://localhost:5558");

        static public byte[] Communicate(string request) {
            lock (commsLock) {
                Console.WriteLine($"Sending {request}");
                sender.SendFrame(request);

                //server.ReceiveFrameString();
                var message = responseReceiver.ReceiveFrameBytes();

                if (message.Length == 0)
                    throw new Exception("empty response");

                return message;
            }
        }

        public class LogOnModel {
            [Required]
            [Display(Name = "User name")]
            public string username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string password { get; set; }
        }

        public class TicketPurchaseModel {
            [Required]
            [Display(Name = "Start station")]
            public string stationA { get; set; }

            [Required]
            [Display(Name = "End station")]
            public string stationB { get; set; }
        }

        [HttpGet("[action]")]
        public int getDistance(string stationA, string stationB) {
            string request = "GetDistance;";
            request += stationA + ";" + stationB;

            byte[] response;
            try {
                response = Communicate(request);
            } catch (Exception) {
                response = BitConverter.GetBytes(-1.0);
            }
            double distance = BitConverter.ToDouble(response, 0);

            return (int)distance;
        }

        [HttpGet("[action]")]
        public string[] getAllStations() {
            string request = "GetAllStationNames";

            byte[] response;
            try {
                response = Communicate(request);
            } catch (Exception) {
                response = Encoding.ASCII.GetBytes("ERROR");
            }

            string stations = Encoding.ASCII.GetString(response);
            string[] stationNames = stations.Split(";");

            Array.Sort(stationNames);

            return stationNames;
        }

        [HttpPost("[action]")]
        public bool loginUser([FromBody]LogOnModel logOnModel) {
            byte[] userBytes = HttpContext.Session.Get("currentUser");

            if (userBytes != null) {
                string user = Encoding.ASCII.GetString(userBytes);

                if (user.Equals(logOnModel.username))
                    return true;
            }

            if (logOnModel.username == null || logOnModel.password == null)
                return false;

            string request = "Authenticate;";
            request += logOnModel.username + ";";
            request += GetBase64FromString(logOnModel.password);

            byte[] response;
            try {
                response = Communicate(request);
            } catch (Exception) {
                response = response = BitConverter.GetBytes(false);
            }

            if (BitConverter.ToBoolean(response, 0)) {
                HttpContext.Session.Set("currentUser", Encoding.ASCII.GetBytes(logOnModel.username));
                return true;
            }
            else {
                return false;
            }

        }

        [HttpGet("[action]")]
        public bool isLoggedIn() {
            return HttpContext.Session.Get("currentUser") != null;
        }

        [HttpGet("[action]")]
        public string getLoggedUsername() {
            if (HttpContext.Session.Get("currentUser") != null) {
                return Encoding.ASCII.GetString(HttpContext.Session.Get("currentUser"));
            }

            return "guest";
        }

        [HttpPost("[action]")]
        public void logoutUser() {
            HttpContext.Session.Remove("currentUser");
        }

        [HttpPost("[action]")]
        public bool registerUser([FromBody]LogOnModel logOnModel) {
            if (logOnModel.username == null || logOnModel.password == null)
                return false;

            if (checkIfLoginAvailable(logOnModel.username)) {
                addUser(logOnModel.username, logOnModel.password);
                return true;
            } else {
                return false;
            }
        }

        public bool checkIfLoginAvailable(string login) {
            string request = "CheckIfLoginAvailable;";
            request += login;

            byte[] response;
            try {
                response = Communicate(request);
            } catch (Exception) {
                response = BitConverter.GetBytes(-1);
            }

            return BitConverter.ToInt32(response, 0) == -1;
        }

        private string GetBase64FromString(string data) {
            using (var sha1 = new SHA1CryptoServiceProvider()) {
                var sha1data = sha1.ComputeHash(Encoding.ASCII.GetBytes(data));
                return Convert.ToBase64String(sha1data);
            }
        }

        private void addUser(string username, string password) {
            string request = "AddUser;";
            request += username + ";";
            request += GetBase64FromString(password);

            try {
                Communicate(request);
            } catch (Exception) {

            }
        }

        [HttpPost("[action]")]
        public bool buyTicket([FromBody]TicketPurchaseModel ticketPurchaseDetails) {
            if (ticketPurchaseDetails.stationA == null || ticketPurchaseDetails.stationB == null)
                return false;

            if (HttpContext.Session.Get("currentUser") == null)
                return false;

            string request = "BuyTicket;";
            request += Encoding.ASCII.GetString(HttpContext.Session.Get("currentUser")) + ";";
            request += ticketPurchaseDetails.stationA + ";";
            request += ticketPurchaseDetails.stationB;

            byte[] response;
            try {
                response = Communicate(request);
            } catch (Exception) {
                response = BitConverter.GetBytes(false);
            }

            if (BitConverter.ToBoolean(response, 0)) {
                return true;
            } else {
                return false;
            }
        }

        [HttpGet("[action]")]
        public IEnumerable<requestProcessing.Model.TicketData> getTicketsByUser(string username) {

            if (getLoggedUsername() != username)
                return null;

            string request = "GetTicketsByUser;";
            request += username;

            var response = Communicate(request);

            requestProcessing.Model.TicketData[] tickets;
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(response)) {
                tickets = (requestProcessing.Model.TicketData[])formatter.Deserialize(stream);
            }

            return tickets;
        }

        // GET: Ticket
        public ActionResult Index() {
            return View();
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int id) {
            return View();
        }

        // GET: Ticket/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection) {
            try {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int id) {
            return View();
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection) {
            try {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int id) {
            return View();
        }

        // POST: Ticket/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection) {
            try {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            } catch {
                return View();
            }
        }
    }
}