using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD

namespace WebApp_LMIGREATICE.Controllers
{
    public class stationsController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1();

        // GET: stations
        public ActionResult Index()
        {
            var stations = db.stations.OrderByDescending(i => i.stateR).ThenBy(i => i.nameStation).ToList();
            return View(stations);
        }

        // GET: stations/Details/5
        public ActionResult Details(int? id)
        {
            Session["error"] = null;
            Session["notification"] = null;
            Session["message"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            station station = db.stations.Single(i => i.idStation == id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // GET: stations/Create
        public ActionResult Create()
        {
            ViewBag.locations = new SelectList(db.locations.ToList(), "idLocation", "nameLocation");
            ViewBag.glaciers = new SelectList(db.glaciers.Where(i => i.stateR ==true).ToList(), "idGlacier", "nameGlacier");
            ViewBag.measurementTypes = new SelectList(db.measurementTypes.ToList(), "idMeasurementType", "nameMT");
            return View();
        }

        // POST: stations/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, station station)
        {
            try
            {
                var newStation = db.stations.Where(a => a.nameStation.Equals(station.nameStation)).FirstOrDefault();
                if (newStation == null)
                {
                    db.stations.Add(station);
                    db.SaveChanges();
                    Session.Add("message", "Station added successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "This station already exists!");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Create");
                }
            }
            catch(Exception ex)
            {
                Session["error"] = ex.Message;
                return View();
            }
        }

        // GET: stations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var station = db.glaciers.Single(i => i.idGlacier == id);
            var station = db.stations.Single(i => i.idStation == id);
            if (station == null)
            {
                return HttpNotFound();
            }
            ViewBag.locations = new SelectList(db.locations.ToList(), "idLocation", "nameLocation");
            ViewBag.glaciers = new SelectList(db.glaciers.ToList(), "idGlacier", "nameGlacier");
            ViewBag.measurementTypes = new SelectList(db.measurementTypes.ToList(), "idMeasurementType", "nameMT");
            return View(station);
        }

        // POST: stations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int id, FormCollection collection, station newStation)
        {
            try
            {
                station station = db.stations.Single(i => i.idStation == id);
                if (db.stations.Where(i => i.nameStation == newStation.nameStation && i.idStation != newStation.idStation
                && i.stateR == true).FirstOrDefault() == null)
                {
                    UpdateModel(station);
                    db.SaveChanges();
                    Session.Add("message", "Station updated successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "A station with the same name alreaddy exists!");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Edit", id);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: stations/Delete/5
        public ActionResult Delete(int id)
        {
            station station = db.stations.Single(i => i.idStation == id);
            station.stateR = false;
            db.SaveChanges();
            Session.Add("message", "Station desactivated successfully");
            Session["error"] = null;
            Session["notification"] = null;
            return RedirectToAction("Index");
        }

        // POST: stations/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
