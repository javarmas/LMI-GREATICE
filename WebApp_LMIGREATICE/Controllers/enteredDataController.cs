using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO; //Permitirá usar Path
using WebApp_LMIGREATICE.Models; //Espacio de nombres que permitirá acceder a los modelos creados por EF a partir de la BBDD
using System.Net; //Permite acceder al tipo Enum HttpStatusCode que es utilizado en la acción "Details"
using System.Data.Entity.SqlServer;
using System.Web.Helpers;
using WebApp_LMIGREATICE.Models.Extension;
using System.Threading.Tasks;
//using System.Web.UI.DataVisualization.Charting;
using System.ComponentModel;
namespace WebApp_LMIGREATICE.Controllers
{
    public class enteredDataController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1();

        //IEnumerable<FilteredData> resultTemp;


        EnteredDataExt aux = new EnteredDataExt();

        // GET: enteredData
        public ActionResult Index()
        {
            var documents = db.enteredDatas.ToList().OrderByDescending(i => i.idEnteredData);
            return View(documents);
        }

        // GET: enteredData/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: enteredData/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: enteredData/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: enteredData/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: enteredData/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: enteredData/Delete/5
        public ActionResult GoDown(int id)
        {
            try
            {
                enteredData doc = db.enteredDatas.Single(i => i.idEnteredData == id);
                if (doc.station.idMeasurementType == 1)
                {
                    //METEREOLOGICAL
                    db.metadatas.RemoveRange(doc.metadatas);
                    db.SaveChanges();

                    db.meteorologicalDatas.RemoveRange(doc.meteorologicalDatas);
                    db.SaveChanges();
                }
                else if (doc.station.idMeasurementType == 2)
                {
                    //GLACIOLOGICAL
                    var data = db.glaciologicalDatas.Where(i => i.idEnteredData == id);
                    db.glaciologicalDatas.RemoveRange(data);
                }
                else if (doc.station.idMeasurementType == 3)
                {
                    //HIDROLOGICAL
                    var data = db.hydrologicalDatas.Where(i => i.idEnteredData == id);
                    db.hydrologicalDatas.RemoveRange(data);
                }
                string partName = "ORIGINAL-" + doc.nameEnteredData;
                var path = Path.Combine(Server.MapPath("~/UploadedFiles"), partName);
                System.IO.File.Delete(path);
                db.enteredDatas.Remove(doc);
                db.SaveChanges();
                Session["error"] = null;
                Session["message"] = null;
                Session["notification"] = null;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Session["error"] = ex.Message;
                Session["message"] = null;
                Session["notification"] = null;
                return RedirectToAction("Index");
            }
        }

        public JsonResult GetStationList(int idMType) //Obtiene la lista de estaciones para el Dropdownlist
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<station> stations = db.stations.Where(x => x.idMeasurementType == idMType && x.stateR == true).ToList();
            return Json(stations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlacierList(int idMount) //Obtiene la lista de glaciares para el Dropdownlist
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<glacier> glaciers = db.glaciers.Where(x => x.idMountain == idMount && x.stateR == true).ToList();
            return Json(glaciers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMountainList(int idCountrySelec) //Obtiene la lista de montañas para el Dropdownlist
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<mountain> mountains = db.mountains.Where(x => x.idProjectCountry == idCountrySelec && x.stateR == true).ToList();
            return Json(mountains, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStationSearchList(int idGlacier, int idMType) //Obtiene la lista de estaciones para el Dropdownlist
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<station> stations = db.stations.Where(x => x.idGlacier == idGlacier && x.idMeasurementType == idMType && x.stateR == true).ToList();
            return Json(stations, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocsByStation(int idStation) //Obtiene la lista de documentos por estacion para mostrarlos
        {
            db.Configuration.ProxyCreationEnabled = false;
            StationExt result = new StationExt();
            result.glacier = new glacier();
            result.measurementType = new measurementType();
            station aux = db.stations.Include("glacier").Single(x => x.idStation == idStation);
            glacier gaxu = db.glaciers.Single(i => i.idGlacier == aux.idGlacier);
            measurementType maxu = db.measurementTypes.Single(i => i.idMeasurementType == aux.idMeasurementType);
            var docs = db.enteredDatas.Where(x => x.idStation == idStation && x.stateR == true).Select(p => new EnteredDataSimplify
            {
                idDoc = p.idEnteredData,
                idStation = p.idStation,
                name = p.nameEnteredData,
                startDate = p.startDate.ToString(),
                endDate = p.endDate.ToString()
            }).ToList();
            result.docs = docs;
            result.nameStation = aux.nameStation;
            result.latitudeStation = aux.latitudeStation;
            result.longitudeStation = aux.longitudeStation;
            result.altitudeStation = aux.altitudeStation;
            result.glacier.nameGlacier = gaxu.nameGlacier;
            result.measurementType.nameMT = maxu.nameMT;
            result.location = aux.location;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStationInfo(int idStation) //Obtiene la informacion de la estación seleccionada como marcador en el mapa
        {
            db.Configuration.ProxyCreationEnabled = false;
            var station = db.stations.Where(x => x.idStation == idStation && x.stateR == true);

            return Json(station, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileStrcutureByidMeasurementType(int idMeasurmentType) //Obtiene la lista de documentos por estacion para mostrarlos
        {
            List<ComposeDoc> docs = new List<ComposeDoc>();
            if (idMeasurmentType == 1)
            {
                docs = new ComposeDoc().getMeteoStructure();
            }
            else if (idMeasurmentType == 2)
            {
                docs = new ComposeDoc().getGlacialStructure();
            }
            else if (idMeasurmentType == 3)
            {
                docs = new ComposeDoc().getHidroStructure();
            }
            return Json(docs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile() //GET de cargar documentos
        {
            ViewBag.dataTypes = new SelectList(db.dataTypes, "idDataType", "nameDataType", "1");
            ViewBag.measurementTypes = new SelectList(db.measurementTypes, "idMeasurementType", "nameMT");
            ViewBag.users = new SelectList(db.systemUsers, "idSystemUser", "firstName", "1");
            Session["error"] = null;
            Session["message"] = null;
            Session["notification"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase fileToCheck, enteredData enteredData) //HttpPostedFileBase FileUpload
        {
            if (Request != null)//control generico de existencia de archivo
            {
                //HttpPostedFileBase file = Request.Files["files"]; //leo el archivo subido por el usuario. Elnombre debe coincidir con el declarado en el la Vista
                try
                {
                    if (fileToCheck.ContentLength > 0) //TAMAÑO DEL ARCHIVO MAYOR A 0 BYTES
                    {
                        if (validateFileExtension(fileToCheck)) //extension csv
                        {
                            StreamReader csvReader = new StreamReader(fileToCheck.InputStream);
                            Holder hold = validateFileStructure(csvReader, enteredData); //validación de estructura de archivo
                            if (hold.status == true)
                            {
                                saveFileData(csvReader, fileToCheck, enteredData);
                            }
                            else
                            {
                                Session["error"] = hold.message;
                                Session["message"] = null;
                                Session["notification"] = null;
                                return RedirectToAction("UploadFile");
                            }
                        }
                        else
                        {
                            Session["error"] = "The file must have an extension .csv";
                            Session["message"] = null;
                            Session["notification"] = null;
                        }
                        return RedirectToAction("UploadFile");
                    }
                }
                catch
                {
                    Session["error"] = "File upload failed!" + " " + "Please verify that there are no NULL fields";
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("UploadFile");
                }

                //StreamReader csvReader = new StreamReader(file.InputStream);


            }
            return RedirectToAction("UploadFile");
        }


        public void saveFileData(StreamReader csvReader, HttpPostedFileBase fileToCheck, enteredData enteredData) //Guardar datos en la bbdd
        {
            csvReader.BaseStream.Seek(0, SeekOrigin.Begin); //Resetea la posición del stream para que empiece de nuevo
                                                            //Insertar Datos en la BBDD
            int rowNumber = 1;
            int idDocument = checkAndInsertDocumentFeatures(enteredData, Path.GetFileName(fileToCheck.FileName));
            var selectedStation = db.stations.Where(a => a.idStation.Equals(enteredData.idStation)).FirstOrDefault();

            while (!csvReader.EndOfStream)
            {
                if (selectedStation.measurementType.nameMT == "Meteorological")
                {
                    string reader = csvReader.ReadLine();
                    if (!reader.Contains("#"))
                    {
                        insertMeteorologicalData(idDocument, reader, selectedStation);
                    }
                    else
                    {
                        insertMeteoMetadata(idDocument, rowNumber, reader, enteredData);
                        rowNumber = rowNumber + 1;
                    }
                }
                else if (selectedStation.measurementType.nameMT == "Glaciological")
                {
                    string reader = csvReader.ReadLine();
                    if (!reader.Contains("#"))
                    {
                        insertGlaciologicalData(idDocument, reader);
                    }
                }
                else
                {
                    string reader = csvReader.ReadLine();
                    insertHydrologicalData(idDocument, rowNumber, reader);
                    rowNumber = rowNumber + 1;
                }
            }

            //Insertar documento en la carpeta UploadedFiles
            string fileName = "ORIGINAL-" + Path.GetFileName(fileToCheck.FileName);
            string path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
            fileToCheck.SaveAs(path);
            //Session["error"] = "File uploaded successfully!";
            //return RedirectToAction("UploadFile");
        }

        public bool validateFileExtension(HttpPostedFileBase fileToCheck) //VALIDA QUE EL ARCHIVO A SUBIR TENGA EXTENSION .csv
        {
            try
            {
                var supportedTypes = new[] { "csv" };
                var fileExtension = System.IO.Path.GetExtension(fileToCheck.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExtension))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Holder validateFileStructure(StreamReader csvReader, enteredData enteredData) //VALIDA ESTRUCTURA A TRAVÉS DE LA CLASE StructureValidation
        {
            Holder hold = new Holder();
            var selectedStation = db.stations.Where(a => a.idStation.Equals(enteredData.idStation)).FirstOrDefault();

            if (enteredData.idDataType == 1) //LOS DATOS A VALIDAR SON OBSERVACIONES
            {
                if (selectedStation.idMeasurementType == 1) //VALIDAR ESTRUCTURA DATOS METEOROLOGICOS
                {
                    hold = new StructureValidation().checkMeteorologicalStructure(csvReader, enteredData, selectedStation);
                }
                else if (selectedStation.idMeasurementType == 2) //VALIDAR ESTRUCTURA DATOS GLACIOLOGICOS
                {
                    hold = new StructureValidation().checkGlaciologicalStructure(csvReader);
                }
                else //VALIDAR ESTRUCTURA DATOS HIDROLOGICOS
                {
                    hold = new StructureValidation().checkHydrologicalStructure(csvReader);
                }
            }
            else //DATOS A VALIDAR SON SIMULACIONES
            {
                hold.status = true;
                hold.message = "NO";
            }
            return hold;
        }

        public int checkAndInsertDocumentFeatures(enteredData enteredData, string fileName)
        {
            try
            {
                //systemUsersController u = new systemUsersController();
                enteredData document = new enteredData();
                document.nameEnteredData = fileName;
                document.startDate = enteredData.startDate;
                document.endDate = enteredData.endDate;
                document.idDataType = enteredData.idDataType;
                document.idStation = enteredData.idStation;
                document.idSystemUser = int.Parse(Session["idSystemUser"].ToString());
                document.stateR = true;
                db.enteredDatas.Add(document);
                db.SaveChanges();
                return document.idEnteredData;
            }
            catch
            {
                return 0;
            }
        }

        public bool insertMeteoMetadata(int idDocument, int rowNumber, string reader, enteredData enteredData)
        {
            try //SOLO HAY UNO INDEPENDIENTE SI ES DENTRO O FUERA DEL GLACIAR PORQUE EL NÚMERO DE COLUMNAS DE METADATOS ES EL MISMO
            {
                if (rowNumber > 3)
                {
                    if (!reader.Contains("#date"))
                    {
                        string[] aux = null;
                        aux = reader.Split(';').ToArray();
                        metadata metadata = new metadata();
                        metadata.variable_code = aux[0];
                        metadata.variable_name = aux[1];
                        metadata.sensor_manufacturer = aux[2];
                        metadata.sensor_model = aux[3];
                        metadata.unit = aux[4];
                        metadata.dataQuality = aux[5];
                        metadata.idEnteredData = idDocument;
                        db.metadatas.Add(metadata);
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool insertMeteorologicalData(int idDocument, string reader, station selectedStation)
        {
            try
            {
                string[] aux = null;
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                meteorologicalData data = new meteorologicalData();

                if (selectedStation.idLocation == 1) //Datos Meteorológicos dentro del glaciar
                {
                    data.dateData = Convert.ToDateTime(aux[0]);
                    data.inShortWaveRadiation = (Math.Round((float.Parse(aux[1])), 3));
                    data.outShortWaveRadiation = (Math.Round((float.Parse(aux[2])), 3));
                    data.albedo = (Math.Round((float.Parse(aux[3])), 3));
                    data.relativeHumidity = (Math.Round((float.Parse(aux[4])), 3));
                    data.ventilatedAirTemperature = (Math.Round((float.Parse(aux[5])), 3));
                    data.windSpeed = (Math.Round((float.Parse(aux[6])), 3));
                    data.windDirection = (Math.Round((float.Parse(aux[7])), 3));
                }
                else //Datos Meteorológicos fuera del glaciar
                {
                    data.dateData = Convert.ToDateTime(aux[0]);
                    data.inShortWaveRadiation = (Math.Round((float.Parse(aux[1])), 3));
                    data.outShortWaveRadiation = (Math.Round((float.Parse(aux[2])), 3));
                    data.albedo = (Math.Round((float.Parse(aux[3])), 3));
                    data.inLongWaveRadiation = (Math.Round((float.Parse(aux[4])), 3));
                    data.outLongWaveRadiation = (Math.Round((float.Parse(aux[5])), 3));
                    data.relativeHumidity = (Math.Round((float.Parse(aux[6])), 3));
                    data.ventilatedAirTemperature = (Math.Round((float.Parse(aux[7])), 3));
                    data.nonVentilatedAirTemperature = (Math.Round((float.Parse(aux[8])), 3));
                    data.windSpeed = (Math.Round((float.Parse(aux[9])), 3));
                    data.windDirection = (Math.Round((float.Parse(aux[10])), 3));
                    data.precipitationAmount = (Math.Round((float.Parse(aux[11])), 3));
                    data.precipitationRate = (Math.Round((float.Parse(aux[12])), 3));
                    data.distanceSensor_Snow = (Math.Round((float.Parse(aux[13])), 3));
                    data.snowDepth = (Math.Round((float.Parse(aux[14])), 3));
                    data.groundFlux = (Math.Round((float.Parse(aux[15])), 3));
                    data.groundTemp_3cm = (Math.Round((float.Parse(aux[16])), 3));
                    data.groundTemp_10cm = (Math.Round((float.Parse(aux[17])), 3));
                    data.groundTemp_30cm = (Math.Round((float.Parse(aux[18])), 3));
                }
                data.idEnteredData = idDocument;
                db.meteorologicalDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool insertGlaciologicalData(int idDocument, string reader)
        {
            try
            {
                string[] aux = null;
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                glaciologicalData data = new glaciologicalData();
                data.altitudinalBar = aux[0];
                data.startDay = Convert.ToInt16(aux[1]);
                data.startMonth = Convert.ToInt16(aux[2]);
                data.startYear = Convert.ToInt16(aux[3]);
                data.endDay = Convert.ToInt16(aux[4]);
                data.endMonth = Convert.ToInt16(aux[5]);
                data.endYear = Convert.ToInt16(aux[6]);
                data.latitude = Convert.ToDouble(aux[7]);
                data.longitude = Convert.ToDouble(aux[8]);
                data.altitude = Convert.ToDouble(aux[9]);
                data.massBalance = Convert.ToDouble(aux[10]);
                data.monthData = Convert.ToInt16(aux[11]);
                data.idEnteredData = idDocument;
                db.glaciologicalDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool insertHydrologicalData(int idDocument, int rowNumber, string reader)
        {
            try
            {
                string[] aux = null;
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                hydrologicalData data = new hydrologicalData();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ViewResult Search()
        {

            ViewBag.projectCountries = new SelectList(db.projectCountries, "idProjectCountry", "nameCountry");
            ViewBag.dataTypes = new SelectList(db.dataTypes, "idDataType", "nameDataType");
            ViewBag.measurementTypes = new SelectList(db.measurementTypes, "idMeasurementType", "nameMT");
            ViewBag.locations = new SelectList(db.locations, "idLocation", "nameLocation");
            ViewBag.stations = new SelectList(db.stations.Where(i => i.stateR == true), "idStation", "nameStation");
            ViewBag.docs = new SelectList(db.enteredDatas.Where(i => i.stateR == true && i.station.idMeasurementType == 2), "idEnteredData", "nameEnteredData");
            ViewBag.variable = new SelectList(new ComposeDoc().getMeteoStructure(), "key", "rowName");
            List<int> years = new List<int>();
            for (int i = DateTime.Now.Year; i >= (DateTime.Now.Year - 30); i--)
            {
                years.Add(i);
            }
            ViewBag.year = new SelectList(years);

            List<string> altitudinalBars = new List<string>();
            altitudinalBars.Add("4800_4850");
            altitudinalBars.Add("4850_4900");
            altitudinalBars.Add("4900_4950");
            altitudinalBars.Add("4950_5000");
            altitudinalBars.Add("5000_5050");
            altitudinalBars.Add("5050_5100");
            altitudinalBars.Add("5000-5150");
            ViewBag.altitudinalBar = new SelectList(altitudinalBars);

            db.Configuration.ProxyCreationEnabled = false;
            ViewBag.fullStation = db.stations.Where(i => i.stateR == true).Select(p => new StationTrick
            {
                idStation = p.idStation,
                nameStation = p.nameStation,
                idLocation = p.idLocation,
                latitudeStation = p.latitudeStation,
                longitudeStation = p.longitudeStation,
                altitudeStation = p.altitudeStation,
                idGlacier = p.idGlacier,
                idMeasurementType = p.idMeasurementType
            }).ToList();
            return View();
        }


        [HttpPost]
        public ActionResult Search(EnteredDataExt u)
        {
            int year = u.year;
            string variable = u.variable;
            int elId = u.idMeasurementType;

            EnteredDataExt searchedInfo = u;
            Session.Add("SearchedInfo", searchedInfo);
            if (u.idStation != 0)
            {
                if (u.idDataType == 1) //Búsqueda para tipos de datos Observation
                {
                    if (u.idMeasurementType == 1) //Búsqueda cuando la información es de meteorología
                    {
                        List<meteorologicalData> searchedData = (from doc in db.enteredDatas
                                                                 join data in db.meteorologicalDatas on doc.idEnteredData equals data.idEnteredData
                                                                 where doc.idStation == u.idStation && (SqlFunctions.DatePart("year", doc.startDate) == year || SqlFunctions.DatePart("year", doc.endDate) == year) && SqlFunctions.DatePart("year", data.dateData) == year  //&& Convert.ToDouble(data.Get<meteorologicalData>(variable).ToString()) != -6999
                                                                 select data).ToList();

                        if (searchedData.Any())
                        {
                            List<FilteredData> summary = new List<FilteredData>();
                            foreach (var item in searchedData)
                            {
                                if (Double.Parse(item.Get<double>(variable).ToString()) != -6999)
                                {
                                    FilteredData aux = new FilteredData()
                                    {
                                        key1 = item.dateData,
                                        Key2 = Double.Parse(item.Get<double>(variable).ToString())
                                    };
                                    summary.Add(aux);
                                }
                            }
                            IEnumerable<FilteredData> result;
                            if (variable == "precipitationAmount") //Para precipitaciones se suma (valor acumulado)
                            {
                                result = from f in summary
                                         group f by f.key1.Date into g
                                         select new FilteredData { key1 = g.Key, Key2 = g.Sum(x => Convert.ToDouble(x.Key2.ToString())) };
                                //generateGraphics(result, variable);

                            }
                            else //Para el resto de variables se promedia
                            {
                                result = from f in summary
                                         group f by f.key1.Date into g
                                         select new FilteredData { key1 = g.Key, Key2 = g.Average(x => Convert.ToDouble(x.Key2.ToString())) };
                                //generateGraphics(result, variable);                
                            }
                            Session.Add("stationInfo", db.stations.Single(i => i.idStation == u.idStation));
                            Session.Add("resultInfo", result);
                            Session.Add("variableInfo", variable);

                            generateGraphics(result, variable, u);

                            aux = u;
                            Session.Add("results", result);
                            return View("ShowResults", result);
                        }
                        else
                        {
                            Session.Add("notification", "No data found with the selected search criteria");
                            Session["message"] = null;
                            Session["error"] = null;
                            return RedirectToAction("Search", u);
                        }
                    }

                    else if (u.idMeasurementType == 2) //Búsqueda cuando la información es de glaciología
                    {

                        List<glaciologicalData> searchedData = (from doc in db.enteredDatas
                                                                join data in db.glaciologicalDatas on doc.idEnteredData equals data.idEnteredData
                                                                where doc.idEnteredData == u.idEnteredData && data.altitudinalBar == u.altitudinalBar
                                                                select data).ToList();
                        if (searchedData.Any())
                        {
                            List<FilteredData> summary = new List<FilteredData>();
                            foreach (var item in searchedData)
                            {
                                var cnkjdvbhdfbvdf = item.Get<double>(variable).ToString();
                                if (Double.Parse(item.Get<double>(variable).ToString()) != -6999)
                                {
                                    if (item.startDay != -6999)
                                    {
                                        FilteredData aux = new FilteredData()
                                        {
                                            key1 = (DateTime.Parse(item.startDay.ToString() + "/" + item.startMonth.ToString() + "/" + item.startYear.ToString())),
                                            //key3 = item.startMonth,
                                            Key2 = Double.Parse(item.Get<double>(variable).ToString())
                                        };
                                        summary.Add(aux);
                                    }
                                    else
                                    {
                                        FilteredData aux = new FilteredData()
                                        {
                                            key1 = (DateTime.Parse("01" + "/" + item.startMonth.ToString() + "/" + item.startYear.ToString())),
                                            //key3 = item.startMonth,
                                            Key2 = Double.Parse(item.Get<double>(variable).ToString())
                                        };
                                        summary.Add(aux);
                                    }
                                }
                            }
                            IEnumerable<FilteredData> result;
                            result = (IEnumerable<FilteredData>)(summary);

                            Session.Add("stationInfo", db.stations.Single(i => i.idStation == u.idStation));
                            Session.Add("resultInfo", result);
                            Session.Add("variableInfo", variable);
                            Session.Add("altitudinalBar", u.altitudinalBar);

                            generateGraphics(result, variable, u);

                            aux = u;
                            Session.Add("results", result);
                            return View("ShowResults", result);
                        }
                        else
                        {
                            Session.Add("notification", "No data found with the selected search criteria");
                            Session["message"] = null;
                            Session["error"] = null;
                            return RedirectToAction("Search", u);
                        }
                    }

                    else             //Búsqueda cuando la información es de hidrología
                    {
                        List<hydrologicalData> searchedData = (from doc in db.enteredDatas
                                                               join data in db.hydrologicalDatas on doc.idEnteredData equals data.idEnteredData
                                                               where doc.idStation == u.idStation && (SqlFunctions.DatePart("year", doc.startDate) == year || SqlFunctions.DatePart("year", doc.endDate) == year) && SqlFunctions.DatePart("year", data.dateData) == year  //&& Convert.ToDouble(data.Get<meteorologicalData>(variable).ToString()) != -6999
                                                               select data).ToList();

                        if (searchedData.Any())
                        {
                            List<FilteredData> summary = new List<FilteredData>();
                            foreach (var item in searchedData)
                            {
                                var cnkjdvbhdfbvdf = item.Get<double>(variable).ToString();
                                if (Double.Parse(item.Get<double>(variable).ToString()) != -6999)
                                {
                                    FilteredData aux = new FilteredData()
                                    {
                                        key1 = item.dateData,
                                        Key2 = Double.Parse(item.Get<double>(variable).ToString())
                                    };
                                    summary.Add(aux);
                                }
                            }
                            IEnumerable<FilteredData> result;

                            result = from f in summary
                                     group f by f.key1.Date into g
                                     select new FilteredData { key1 = g.Key, Key2 = g.Average(x => Convert.ToDouble(x.Key2.ToString())) };

                            //generateGraphics(result, variable);                
                            Session.Add("stationInfo", db.stations.Single(i => i.idStation == u.idStation));
                            Session.Add("resultInfo", result);
                            Session.Add("variableInfo", variable);

                            generateGraphics(result, variable, u);

                            aux = u;
                            Session.Add("results", result);
                            return View("ShowResults", result);
                        }
                        else
                        {
                            Session.Add("notification", "No data found with the selected search criteria");
                            Session["message"] = null;
                            Session["error"] = null;
                            return RedirectToAction("Search", u);
                        }
                    }
                }
                else
                {
                    Session.Add("notification", "Simulations are not available at this moment. We are sorry!");
                    Session["message"] = null;
                    Session["error"] = null;
                    return RedirectToAction("Search", u);
                }
            }
            else
            {
                Session.Add("notification", null);
                Session["message"] = null;
                Session["error"] = "Select all fields";
                return RedirectToAction("Search");
            }


        }

        public ViewResult ShowResults()
        {
            return View();
        }

        public void generateGraphics(IEnumerable<FilteredData> data, string variable, EnteredDataExt searchedInfo)
        {
            List<DateTime> dates = new List<DateTime>();
            List<double?> values = new List<double?>();
            try
            {
                foreach (var item in data)
                {
                    dates.Add(item.key1);
                    values.Add(item.Key2);
                }
            }
            catch (Exception ex) { }
            new Chart(width: 600, height: 400, theme: ChartTheme.Green).AddTitle("Results").
                AddSeries(chartType: "Line", xValue: dates, yValues: values).AddLegend(variable, null).
                Save("~/Content/images/hola.jpeg", "jpeg");
        }

        //public void validateBeforeExport()
        //{
        //    if (Session["activeSession"] != null && ((bool)Session["activeSession"]) == true)
        //    {
        //        if (variableSearched == "meteorological")
        //        {

        //        }
        //        else if (variableSearched == "glaciological")
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    else if (((bool)Session["activeSession"]) == false)
        //    {

        //    }
        //}


        [HttpPost]
        public FileResult exportCSV()
        {
            //if (Session["activeSession"] != null && ((bool)Session["activeSession"]) == true)
            //{
                var filename = Convert.ToString(Session["variableInfo"]);
                var results = (IEnumerable<FilteredData>)(Session["resultInfo"]);
                var myCSV = new Jitbit.Utils.CsvExport();

                foreach (var iter in results)
                {
                    myCSV.AddRow();
                    myCSV["Date"] = iter.key1.Date;
                    myCSV["Value"] = iter.Key2;
                }
                return File(myCSV.ExportToBytes(), "text/csv", filename+".csv");
            //}
            //else
            //{
            //    Session.Add("notification", "Please login for downloading data");
            //    Session["message"] = null;
            //    Session["error"] = null;
            //    IEnumerable<FilteredData> var = (IEnumerable<FilteredData>)(Session["results"]);
            //    return View("ShowResults", var);
            //}
        }

        public FileResult DownloadFile(int id)
        {
            enteredData doc = db.enteredDatas.Single(I => I.idEnteredData == id);
            
            var filename = Server.MapPath("~/UploadedFiles/" +"ORIGINAL-" +doc.nameEnteredData);
            byte[] datos = System.IO.File.ReadAllBytes(filename);
            return File(datos,".csv",doc.nameEnteredData);
        }
    }


    static class ObjectExtensions
    {
        public static TResult Get<TResult>(this object @this, string propertyName)
        {
            return (TResult)@this.GetType().GetProperty(propertyName).GetValue(@this, null);
        }
    }


}
