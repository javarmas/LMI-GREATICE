using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD

namespace WebApp_LMIGREATICE.Controllers
{
    public class glaciersController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1();


        // GET: glaciers
        public ActionResult Index()
        {
            var glaciers = db.glaciers.OrderByDescending(i => i.stateR).ThenBy(i => i.nameGlacier).ToList();
           // Session["error"] = null;
           // Session["message"] = null;
            return View(glaciers);
        }

        // GET: glaciers/Details/5
        public ActionResult Details(int? id)
        {
            Session["error"] = null;
            Session["notification"] = null;
            Session["message"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            glacier glacier = db.glaciers.Find(id);
            if (glacier == null)
            {
                return HttpNotFound();
            }
            //Session["error"] = null;
            //Session["message"] = null;
            return View(glacier);
        }

        // GET: glaciers/Create
        public ActionResult Create()
        {
            ViewBag.mountains = new SelectList(db.mountains.Where(i => i.stateR == true).ToList(), "idMountain", "nameMountain");
            return View();
        }

        // POST: glaciers/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, glacier glacier)
        {
            try
            {
                var newGlacier = db.glaciers.Where(a => a.nameGlacier.Equals(glacier.nameGlacier)).FirstOrDefault();
                if (newGlacier == null)
                {
                    db.glaciers.Add(glacier);
                    db.SaveChanges();
                    Session.Add("message", "Glacier added successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "This glacier already exists!");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Create");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: glaciers/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var glacier = db.glaciers.Single(i => i.idGlacier == id);
            if (glacier == null)
            {
                return HttpNotFound();
            }
            ViewBag.mountains = new SelectList(db.mountains.Where(i => i.stateR == true).ToList(), "idMountain", "nameMountain");
            return View(glacier);
        }

        // POST: glaciers/Edit/5

        //public ActionResult Edit(int id, FormCollection collection, glacier glaciar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore =true, Duration =0, VaryByParam ="*")]
        public ActionResult Edit(int id, FormCollection collection, glacier glaciar)
        {
            try
            {
                glacier glacier = db.glaciers.Single(i => i.idGlacier == id);
                if (db.glaciers.Where(i => i.nameGlacier == glaciar.nameGlacier && i.idGlacier != glaciar.idGlacier
                && i.stateR == true).FirstOrDefault() == null)
                {
                    UpdateModel(glacier);
                    db.SaveChanges();
                    Session.Add("message", "Glacier updated successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "A glacier with the same name alreaddy exists!");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Edit", id);
                }                       
            }
            catch
            {
                Session.Add("error", "Error!");
                Session["message"] = null;
                Session["notification"] = null;
                return View();
            }
        }

        // GET: glaciers/Delete/5
        public ActionResult Delete(int id)
        {
            glacier glacier = db.glaciers.Single(i => i.idGlacier == id);
            glacier.stateR = false;
            db.SaveChanges();
            Session.Add("message", "Glacier desactivated successfully");
            Session["error"] = null;
            Session["notification"] = null;
            return RedirectToAction("Index");
        }

        // POST: glaciers/Delete/5
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
