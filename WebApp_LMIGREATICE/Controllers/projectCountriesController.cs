using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"

namespace WebApp_LMIGREATICE.Controllers
{
    public class projectCountriesController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1(); //La creación de una instancia de la Context Class permitirá 
                                                                  //acceder y manipular la BBDD.

        // GET: projectCountries
        public ActionResult Index() //Permite leer los datos de la respectiva tabla en la BBDD y enviarlos a la vista Index
        {
            var countries = db.projectCountries.ToList();
            return View(countries);
        }

        // GET: projectCountries/Details/5
        public ActionResult Details(int? id) 
        {
            Session["error"] = null;
            Session["notification"] = null;
            Session["message"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            projectCountry country = db.projectCountries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: glaciers/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, projectCountry country)
        {
            try
            {
                var newCountry  = db.projectCountries.Where(a => a.nameCountry.Equals(country.nameCountry)).FirstOrDefault();
                if (newCountry == null)
                {
                    db.projectCountries.Add(country);
                    db.SaveChanges();
                    Session.Add("message", "Country added successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "This country already exists!");
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
    }
}
