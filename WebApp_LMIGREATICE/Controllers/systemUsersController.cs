using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD
using System.Net; // Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using System.Security.Cryptography; //Permitirá utilizar SHA-256 para generar la función hash del password e insertarlo en la BBDD
using System.Text; //Permitirá utilizar Encoding para generar la función Hash del password 
using System.Net.Mail; // Permitirá enviar el correo de creación de usuario para confirmación al administrador.

namespace WebApp_LMIGREATICE.Controllers
{
    public class systemUsersController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1();

        private static MailAddress origen = new MailAddress("lmigreatice@gmail.com", "LMI GREAT ICE", System.Text.Encoding.UTF8);

        MailMessage correo = new MailMessage()
        {
            From = origen,
        };

        SmtpClient protocolo = new SmtpClient()
        {
            Credentials = new NetworkCredential("lmigreatice@gmail.com", "lgreate4228MI"),
            Port = 587,
            Host = "smtp.gmail.com",
            EnableSsl = true
        };

        // GET: systemUsers
        public ActionResult Index()
        {
            var systemUsers = db.systemUsers.ToList();
            return View(systemUsers);
        }

        // GET: systemUsers/Details/5
        public ActionResult Details(int? id)
        {
            Session["message"] = null;
            Session["notification"] = null;
            Session["error"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            systemUser user = db.systemUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: systemUsers/Create
        public ActionResult Create()
        {
            ViewBag.genders = new SelectList(db.genders, "idGender", "nameGender");
            ViewBag.titles = new SelectList(db.titles, "idTitle", "nameTitle");
            ViewBag.countries = new SelectList(db.countries, "idCountry", "nameCountry");
            ViewBag.institutions = new SelectList(db.institutions, "idInstitution", "nameInstitution");
            ViewBag.userStates = new SelectList(db.userStates, "idUserState", "nameState");
            ViewBag.userPrivileges = new SelectList(db.userPrivileges, "idUserPrivilege", "namePrivilege");
            return View();
        }

        // POST: systemUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(systemUser userRegister)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.systemUsers.Where(a => a.email.Equals(userRegister.email)).FirstOrDefault();
                    if (user == null) //Si no existe usuario con el correo indicado, se crea el usuario.
                    {
                            if (Session["activeSession"] != null && Session["activeSession"].ToString() != "" && (bool)Session["activeSession"] == true)
                            {
                                sendEmailForConfirmation(userRegister);
                                //userRegister.idUserPrivilege = 3; //seteo temporalmente los permisos para download al usuario hasta que se confirme, porque la bbdd no permite nulos
                                // userRegister.idUserState = 3; //seteo el valor de 3 (PENDING), porque la bbdd no admite nulos                                            
                                userRegister.passwordU = ComputeHash256(userRegister.passwordU);
                                db.systemUsers.Add(userRegister);
                                db.SaveChanges();

                                Session.Add("message", "User added successfully");
                                Session["error"] = null;
                                Session["notification"] = null;
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                sendEmailForConfirmation(userRegister);
                                userRegister.idUserPrivilege = 3; //seteo temporalmente los permisos para download al usuario hasta que se confirme, porque la bbdd no permite nulos
                                userRegister.idUserState = 3; //seteo el valor de 3 (PENDING), porque la bbdd no admite nulos                                            
                                userRegister.passwordU = ComputeHash256(userRegister.passwordU);
                                db.systemUsers.Add(userRegister);
                                db.SaveChanges();

                                Session.Add("message", "Your account has been created, however it must be activated by the system Administrator. Meanwhile you will not be able to login. Please wait for an approval");
                                Session["error"] = null;
                                Session["notification"] = null;
                                return RedirectToAction("Create");
                            }
                   
                    }
                    else
                    {
                        Session.Add("error", "User already exists!");
                        Session["message"] = null;
                        Session["notification"] = null;
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Session.Add("error", ex.ToString());
                Session["message"] = null;
                Session["notification"] = null;
                return RedirectToAction("Create");
            }
        }

        // GET: systemUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.systemUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.genders = new SelectList(db.genders, "idGender", "nameGender", "1");
            ViewBag.titles = new SelectList(db.titles, "idTitle", "nameTitle", "1");
            ViewBag.countries = new SelectList(db.countries, "idCountry", "nameCountry", "1");
            ViewBag.institutions = new SelectList(db.institutions, "idInstitution", "nameInstitution", "1");
            ViewBag.privileges = new SelectList(db.userPrivileges, "idUserPrivilege", "namePrivilege", "1");
            ViewBag.states = new SelectList(db.userStates, "idUserState", "nameState", "1");
            return View(user);
        }

        // POST: systemUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Edit(FormCollection collection, int id, systemUser u)
        {
            try
            {                
                // TODO: Add update logic here
                systemUser user = db.systemUsers.Single(i => i.idSystemUser == id);

                if (db.systemUsers.Where(i => i.email == u.email && i.idSystemUser != u.idSystemUser
               && i.idUserState == 1).FirstOrDefault() == null)
                {
                    int aux2 = user.idUserState;

                    user.firstName = u.firstName;
                    user.lastName = u.lastName;
                    user.email = u.email;

                    var aux = db.systemUsers.Where(a => a.idSystemUser.Equals(u.idSystemUser)).FirstOrDefault();

                    if (aux.passwordU == u.passwordU)
                    {
                        user.passwordU = aux.passwordU;
                    }
                    else
                    {
                        user.passwordU = ComputeHash256(u.passwordU);
                    }
                    user.idGender = u.idGender;
                    user.idTitle = u.idTitle;
                    user.idCountry = u.idCountry;
                    user.dateOfBirth = u.dateOfBirth;
                    user.idInstitution = u.idInstitution;

                    if (((int)Session["idUserPrivilege"]) == 1) //Se edita el usuario incluyendo los permisos y el estado
                    {
                        user.idUserPrivilege = u.idUserPrivilege;
                        user.idUserState = u.idUserState;
                        //UpdateModel(user);
                        db.SaveChanges();

                        if (aux2 == 3 && u.idUserState == 1) //Envío correo al usuario indicando que su cuenta ya se activó
                        {
                            sendEmailForChangeState(user);
                        }

                        Session.Add("message", "User updated successfully");
                        Session["error"] = null;
                        Session["notification"] = null;
                        return RedirectToAction("Index");
                    }
                    else //Se edita el usuario sin incluir los permisos y el estado puesto que no tienen autorización
                    {
                        //user.idUserPrivilege = u.idUserPrivilege;
                        //user.idUserState = u.idUserState;
                        //UpdateModel(user);
                        db.SaveChanges();
                        Session.Add("message", "User updated successfully");
                        Session["error"] = null;
                        Session["notification"] = null;
                        return RedirectToAction("UserDashBoard");
                    }
                }
                else
                {
                    Session.Add("error", "A user with the same email alreaddy exists!");
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

        // GET: systemUsers/Delete/5
        public ActionResult Delete(int id)
        {
            systemUser user = db.systemUsers.Single(i => i.idSystemUser == id);
            user.idUserState = 2;
            db.SaveChanges();
            Session.Add("message", "User desactivated successfully");
            Session["error"] = null;
            Session["notification"] = null;
            return RedirectToAction("Index");
        }

        // POST: systemUsers/Delete/5
        [HttpPost]
        public ActionResult Delete(int? id)
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

        // GET: systemUsers/Login/5
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult Login()
        {
            //Session["error"] = null;
            return View();
        }

        // POST: systemUsers/Login/5
        [HttpPost]
        public ActionResult Login(systemUser u) //Allows users start a session
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                string password = ComputeHash256(u.passwordU);
                var user = db.systemUsers.Where(a => a.email.Equals(u.email) && a.passwordU.Equals(password)).FirstOrDefault();

                if (user != null)
                {
                    if (user.idUserState == 1) //Activo
                    {
                        Session["idSystemUser"] = user.idSystemUser;
                        Session["email"] = user.email;
                        Session["name"] = user.firstName;
                        Session["privilege"] = user.userPrivilege;
                        Session["idUserPrivilege"] = user.idUserPrivilege;
                        Session["activeSession"] = true;
                        Session["error"] = null;
                        Session["userLogged"] = u;

                        if (user.idUserPrivilege == 1)
                        {
                            return RedirectPermanent("Index");
                        }
                        else
                        {
                            return RedirectToAction("UserDashBoard", user);
                        }
                        // return RedirectToAction("UserDashboard");
                    }
                    else if (user.idUserState == 3) //Pendiente
                    {
                        Session["error"] = "Your account is not activated yet, please wait for an approval";
                        Session["message"] = null;
                        Session["notification"] = null;
                        return RedirectToAction("Login");
                    }
                    else //Inactivo
                    {
                        Session["error"] = "Your account has been desactivated!";
                        Session["message"] = null;
                        Session["notification"] = null;
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    Session.Add("error", "Invalid Email or Password");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("Login");
                }
                // }
                // return View();
            }
            catch (Exception e)
            {
                Session.Add("error", " ERROR! Please contact marcos.villacis@epn.edu.ec or thomas.condom@ird.fr");
                Session["message"] = null;
                Session["notification"] = null;
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["idSystemUser"] = "";
            Session["email"] = "";
            Session["name"] = "";
            Session["privilege"] = "";
            Session["activeSession"] = false;
            Session["idUserPrivilege"] = "";
            Session["userLogged"] = null;
            return RedirectToAction("Search", "enteredData");
        }

        //[HttpGET]
        public ActionResult UserDashBoard()

        {
            Session["message"] = null;
            Session["notification"] = null;
            Session["error"] = null;

            int id = int.Parse(Session["idSystemUser"].ToString());
            var user = db.systemUsers.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.genders = new SelectList(db.genders, "idGender", "nameGender", "1");
            ViewBag.titles = new SelectList(db.titles, "idTitle", "nameTitle", "1");
            ViewBag.countries = new SelectList(db.countries, "idCountry", "nameCountry", "1");
            ViewBag.institutions = new SelectList(db.institutions, "idInstitution", "nameInstitution", "1");
            ViewBag.privileges = new SelectList(db.userPrivileges, "idUserPrivilege", "namePrivilege", "1");
            ViewBag.states = new SelectList(db.userStates, "idUserState", "nameState", "1");
            return View(user);
        }

        [HttpPost]
        public ActionResult UserDashBoard(FormCollection collection, int id)
        {

            return View();
        }

        public string ComputeHash256(string password) //Allows to compute the hash function of the password and returns it as a string
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public string sendEmailForConfirmation(systemUser newUser)
        {
            string cuerpo = "A new user has registered. A status of pending has been assigned until you confirm the registration. User`s information summary: \n Name: " + newUser.firstName + " \n Last Name: " + newUser.lastName + "\n Email: " + newUser.email + " \n\n To review complete information please login in the system";
            // + " \n Country " + newUser.country.nameCountry + "\n Institution: " + newUser.institution.nameInstitution
            correo.Subject = "New User Confirmation Required";
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.To.Add("javier.armas@epn.edu.ec");
            correo.Body = cuerpo;
            correo.BodyEncoding = System.Text.Encoding.UTF8;
            try
            {
                protocolo.Send(correo);
                return "OK";
            }
            catch (SmtpException e)
            {
                return e.ToString();
            }
        }

        public ActionResult passwordRecovery()
        {
            return View();
        }

        [HttpPost]
        public ActionResult passwordRecovery(string email)
        {
            if (ModelState.IsValid)
            {
                var user = db.systemUsers.Where(a => a.email.Equals(email)).FirstOrDefault();
                if (user != null)
                {
                    if (user.idUserState == 1)
                    {
                        //Generate temporary password
                        int max = 999999999;
                        int min = 9999999;
                        Random r = new Random();
                        int num = r.Next(min, max);
                        string temporaryPassword = ComputeHash256(Convert.ToString(num));
                        user.passwordU = temporaryPassword;
                        sendEmailForRecovery(user, temporaryPassword, num);
                        db.SaveChanges();
                        Session.Add("message", "A temporary password was sent to your email. Use this password to log in and do not forget to change it!");
                        Session["error"] = null;
                        Session["notification"] = null;
                    }
                    else
                    {
                        Session.Add("error", "Your user has no been activated yet!");
                        Session["message"] = null;
                        Session["notification"] = null;
                    }

                    return RedirectToAction("Login");
                }
                else
                {
                    Session.Add("error", "User does not exist! Please insert a valid email or create an account");
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("passwordRecovery");
                }
            }
            return View();
        }

        public string sendEmailForRecovery(systemUser user, string temporaryPassword, int pass)
        {
            string subject = "Request for Password Recovery";
            string cuerpo = ("You have requested a password recovery. This is your temporary password, make sure you update this password once you login in the platform: \n\n" + "Password: " + pass);
            correo.Subject = subject;
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.To.Add(user.email);
            correo.Body = cuerpo;
            correo.BodyEncoding = System.Text.Encoding.UTF8;
            try
            {
                protocolo.Send(correo);
                return "OK";
            }
            catch (SmtpException e)
            {
                return e.ToString();
            }
        }

        public string sendEmailForChangeState(systemUser user)
        {
            string subject = "LMI GREAT ICE account activated";
            string cuerpo = ("Your LMI GREAT ICE account has been activated by the system administrator. From this moment you can login to the system. Welcome!");
            correo.Subject = subject;
            correo.SubjectEncoding = System.Text.Encoding.UTF8;
            correo.To.Add(user.email);
            correo.Body = cuerpo;
            correo.BodyEncoding = System.Text.Encoding.UTF8;
            try
            {
                protocolo.Send(correo);
                return "OK";
            }
            catch (SmtpException e)
            {
                return e.ToString();
            }
        }

        [HttpGet]
        public JsonResult downAllMessages() {
            Session["error"] = null;
            Session["message"] = null;
            Session["notification"] = null;
            return Json(new systemUser(), JsonRequestBehavior.AllowGet);
        }
    }
}
