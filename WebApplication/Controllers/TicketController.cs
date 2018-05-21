using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetMQ;
using NetMQ.Sockets;

namespace WebApplication.Controllers
{
    public class TicketController : Controller
    {
        private byte[] Communicate(string request)
        {
            using (var server = new RequestSocket())
            {
                server.Connect("tcp://localhost:5555");

                Console.WriteLine($"Sending {request}");
                server.SendFrame(request);

                //server.ReceiveFrameString();
                var message = server.ReceiveFrameBytes();

                return message;

                //Console.WriteLine($"response: {message}");
            }
        }


        [HttpGet("[action]")]
        public int getDistance(string stationA, string stationB)
        {
            string request = "GetDistance;";
            request += stationA + ";" + stationB;

            byte[] response = Communicate(request);
            double distance = BitConverter.ToDouble(response, 0);

            return (int)distance;
        }

        [HttpGet("[action]")]
        public string[] getAllStations()
        {
            string request = "GetAllStationNames";

            byte[] response = Communicate(request);

            string stations = Encoding.ASCII.GetString(response);
            string[] stationNames = stations.Split(";");

            return stationNames;
        }

        // GET: Ticket
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ticket/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ticket/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}