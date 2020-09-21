using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD

namespace WebApp_LMIGREATICE.Controllers
{
    public class institutionsController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1(); //La creación de una instancia de la Context Class permitirá 
                                                                  //acceder y manipular la BBDD.

        // GET: institutions
        public ActionResult Index()
        {
            var institutions = db.institutions.ToList();
            return View(institutions);
        }

        // GET: institutions/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: institutions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: institutions/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, institution institution)
        {
            try
            {
                var newInstitution = db.institutions.Where(a => a.nameInstitution.Equals(institution.nameInstitution)).FirstOrDefault();
                if (newInstitution == null)
                {
                    db.institutions.Add(institution);
                    db.SaveChanges();
                    Session.Add("message", "Institution added successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "This institution already exists!");
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

        // GET: institutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var institution = db.institutions.Single(i => i.idInstitution == id);
            if (institution == null)
            {
                return HttpNotFound();
            }
            return View(institution);
        }

        // POST: institutions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(int id, FormCollection collection, institution institution)
        {
            try
            {
                institution old = db.institutions.Single(i => i.idInstitution == id);
                if (db.institutions.Where(i => i.nameInstitution == institution.nameInstitution && i.idInstitution != institution.idInstitution).FirstOrDefault() == null)
                {
                    UpdateModel(institution);
                    db.SaveChanges();
                    Session.Add("message", "Institution updated successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "An institution with the same name alreaddy exists!");
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

        // GET: institutions/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: institutions/Delete/5
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
