using System.Linq;
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using System.Web.Mvc;
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD

namespace WebApp_LMIGREATICE.Controllers
{
    public class mountainsController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1(); //La creación de una instancia de la Context Class permitirá 
                                                                  //acceder y manipular la BBDD.
                                                                  
        // GET: mountains
        public ActionResult Index() //Permite leer los datos de la respectiva tabla en la BBDD y enviarlos a la vista Index
        {
                var mountains = db.mountains.OrderByDescending(i => i.stateR).ThenBy(i => i.nameMountain).ToList();
                return View(mountains);
        }

        // GET: mountains/Details/5
        public ActionResult Details(int? id)
        {
            Session["error"] = null;
            Session["message"] = null;
            Session["notification"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mountain mountain = db.mountains.Find(id);
            if (mountain == null)
            {
                return HttpNotFound();
            }
            return View(mountain);
        }

        // GET: mountains/Create
        public ActionResult Create()
        {
            ViewBag.countries = new SelectList(db.projectCountries.ToList(), "idProjectCountry", "nameCountry");
            return View();
        }

        // POST: mountains/Create
        [HttpPost]
        public ActionResult Create(mountain mountain) //Pasa los datos ingresados por el usuario a la BBDD para insertarlos
        {
            try
            {
                var newMountain = db.mountains.Where(a => a.nameMountain.Equals(mountain.nameMountain)).FirstOrDefault();
                if (newMountain == null)
                {
                    db.mountains.Add(mountain);
                    db.SaveChanges();
                    Session.Add("message", "Mountain added successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "This mountain already exists!");
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

        // GET: mountains/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var mountain = db.mountains.Find(id);

            if (mountain == null)
            {
                return HttpNotFound();
            }
            ViewBag.countries = new SelectList(db.projectCountries.ToList(), "idProjectCountry", "nameCountry");
            ViewBag.showSuccessAlert=false;
            return View(mountain);
        }

        // POST: mountains/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, mountain newMountain)
        {
            try
            {
                mountain mountain = db.mountains.Single(i => i.idMountain == id);
                if (db.mountains.Where(i => i.nameMountain == newMountain.nameMountain && i.idMountain != newMountain.idMountain
                 && i.stateR == true).FirstOrDefault() == null)
                {
                    UpdateModel(mountain);
                    db.SaveChanges();
                    Session.Add("message", "Mountain updated successfully");
                    Session["error"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Index");
                }
                else
                {
                    Session.Add("error", "A mountain with the same name alreaddy exists!");
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

        // GET: mountains/Delete/5
        public ActionResult Delete(int id)
        {
            mountain mountain = db.mountains.Single(i => i.idMountain == id);
            mountain.stateR = false;
            db.SaveChanges();
            Session.Add("message", "Mountain desactivated successfully");
            Session["error"] = null;
            Session["notification"] = null;
            return RedirectToAction("Index");          
        }

        // POST: mountains/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
