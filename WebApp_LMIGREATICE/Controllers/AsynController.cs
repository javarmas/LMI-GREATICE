using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp_LMIGREATICE.Models;
using WebApp_LMIGREATICE.Models.Extension;
using Hangfire;
using WebApp_LMIGREATICE.Controllers;
using System.Globalization;

namespace WebApp_LMIGREATICE.Controllers
{
    public class AsynController : Controller
    {
        DB_LMIGREATICEEntities1 db = new DB_LMIGREATICEEntities1();

        public ActionResult UploadFile()
        {
            ViewBag.dataTypes = new SelectList(db.dataTypes, "idDataType", "nameDataType", "1");
            ViewBag.measurementTypes = new SelectList(db.measurementTypes, "idMeasurementType", "nameMT");
            //ViewBag.stations = new SelectList(db.stations, "idStation", "nameStation", "1");
            ViewBag.users = new SelectList(db.systemUsers, "idSystemUser", "firstName", "1");
            //Session["error"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase fileToCheck, enteredData enteredData) //HttpPostedFileBase FileUpload
        {
            if (Request != null)//control generico de existencia de archivo, que se ha seleccionado el archivo
            {
                //HttpPostedFileBase file = Request.Files["files"]; //leo el archivo subido por el usuario. Elnombre debe coincidir con el declarado en el la Vista
                try
                {
                    if (fileToCheck.ContentLength > 0) //TAMAÑO DEL ARCHIVO MAYOR A 0 BYTES
                    {
                        if (validateFileExtension(fileToCheck)) //extension csv
                        {
                           var fileName = Path.GetFileName(fileToCheck.FileName);
                           var docAux = db.enteredDatas.Where(a => a.nameEnteredData.Equals(fileName)).FirstOrDefault();

                            if (docAux == null) //si no existe un archivo en la bbdd con el mismo nombre, continúo subiendo
                            {
                                StreamReader csvReader = new StreamReader(fileToCheck.InputStream);
                                Holder hold = validateFileStructure(csvReader, enteredData); //validación de estructura de archivo
                                if (hold.status == true)
                                {
                                    int idUser = int.Parse(Session["idSystemUser"].ToString());
                                    var path = Path.Combine(Server.MapPath("~/UploadedFiles/TemporaryFiles"), fileName);
                                    fileToCheck.SaveAs(path);
                                    string finalName = "ORIGINAL-" + fileName;//Path.GetFileName(fileToCheck.FileName);
                                    string finalPath = Path.Combine(Server.MapPath("~/UploadedFiles"), finalName);
                                    BackgroundJob.Enqueue(() => saveFileData(fileName, path, finalPath, enteredData, idUser ));//, System.Web.HttpContext.Current));
                                    Session.Add("message", "Upload process started. DO NOT close the website until this process finished!");
                                    Session["error"] = null;
                                    Session["notification"] = null;
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
                                Session["error"] = "It seems that someone has already upload a file with the same name. Please verify it!";
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
                catch(Exception ex)
                {
                    Session["error"] = ex.Message + "\nFile upload failed!" + " " + "Please verify that there are no NULL fields";
                    Session["message"] = null;
                    Session["notification"] = null;
                    return RedirectToAction("UploadFile");
                }
            }
            return RedirectToAction("UploadFile");
        }

        public void saveFileData(string nameOfFile,string path, string finalPath,enteredData enteredData, int idUser)//, HttpContext mainContext)
        {
            //await Task.Run(() => {
            using (var csvReader = new StreamReader(path)) {
                csvReader.BaseStream.Seek(0, SeekOrigin.Begin); //Resetea la posición del stream para que empiece de nuevo
                
                //Insertar Datos en la BBDD
                int rowNumber = 1;
                int idDocument = checkAndInsertDocumentFeatures(idUser, enteredData, nameOfFile);//Path.GetFileName(fileToCheck.FileName));
                var selectedStation = db.stations.Where(a => a.idStation.Equals(enteredData.idStation)).FirstOrDefault();
                //{
                while (!csvReader.EndOfStream)
                {
                    
                    if (selectedStation.measurementType.nameMT == "Meteorological")
                    {
                        string reader = csvReader.ReadLine();
                        if (!reader.Contains("#"))
                        {
                            //Session["message"] = null;
                           // Session["notification"] = "Loading data...";
                           // Session["error"] = null;
                            bool respHeaders = insertMeteorologicalData(idDocument, reader, selectedStation);                           
                        }
                        else
                        {
                            //Session["message"] = null;
                           // Session["notification"] = "Loading data...";
                           // Session["error"] = null;
                            bool respMetData = insertMeteoMetadata(idDocument, rowNumber, reader, enteredData);
                            rowNumber = rowNumber + 1;
                        }
                    }
                    else if (selectedStation.measurementType.nameMT == "Glaciological")
                    {
                        string reader = csvReader.ReadLine();
                        if (!reader.Contains("#"))
                        {
                            //Session["message"] = null;
                            //Session["notification"] = "Loading data...";
                            //Session["error"] = null;
                            bool respGlaData = insertGlaciologicalData(idDocument, reader);
                        }
                    }
                    else
                    {
                        //Session["message"] = null;
                        //Session["notification"] = "Loading data...";
                        //Session["error"] = null;
                        string reader = csvReader.ReadLine();
                        bool respHydrData = insertHydrologicalData(idDocument, rowNumber, reader);
                        rowNumber = rowNumber + 1;
                    }
                }
                try
                {
                    System.IO.File.Copy(path, finalPath);
                    //FinalizarCarga("Upload data process has finished", null, null,mainContext);
                    Session.Add("message", "Upload data process has finished");
                    Session["notification"] = null;
                    Session["error"] = null;
                    var name = Url.Action();
                    RedirectToAction(name);
                }
                catch (Exception ex) {
                    //FinalizarCarga(null, null, ex.Message, mainContext);
                }                                        
            }   
           // });
            //return await Task.FromResult(true);
        }

        public void FinalizarCarga(string message, string notif, string error, HttpContext context)
        {
            context.Session.Add("message", message);
            context.Session["notification"] = notif;
            context.Session["error"] = error;
            var name = Url.Action();
            RedirectToRoute(name);
            //directToAction(name);
        }

        public bool validateFileExtension(HttpPostedFileBase fileToCheck) //VALIDA QUE EL ARCHIVO A SUBIR TENGA EXTENSION .csv
        {
            try
            {
                var supportedTypes = new[] { "csv" };
                var fileExtension = Path.GetExtension(fileToCheck.FileName).Substring(1);
                return (!supportedTypes.Contains(fileExtension) ? false : true);
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

        public int checkAndInsertDocumentFeatures(int idUser,enteredData enteredData, string fileName)
        {
            try
            {
                enteredData document = new enteredData();
                document.nameEnteredData = fileName;
                document.startDate = enteredData.startDate;
                document.endDate = enteredData.endDate;
                document.idDataType = enteredData.idDataType;
                document.idStation = enteredData.idStation;
                document.idSystemUser = idUser;//int.Parse(Session["idSystemUser"].ToString());
                document.stateR = true;
                db.enteredDatas.Add(document);
                db.SaveChanges();
                return (document.idEnteredData);
            }
            catch (Exception ex)
            {
                var sms = ex.Message;
                return 0;
            }
            //return 0;
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
                var aux1 = aux[0].Split(' '); //SEPARO LA FECHA DE LA HORA
                var aux2 = aux1[0].Split('/'); //SEPARO MES/DIA/AÑO EN LA FECHA PARA LUEGO CONVERTIR A DIA/MES/AÑO
                var auxF = aux2[1] + "/" + aux2[0] + "/" + aux2[2] + " " + aux1[1]; //CONVIERTO A DIA/MES/AÑO

                if (selectedStation.idLocation == 1) //Datos Meteorológicos dentro del glaciar
                {
                    //data.dateData = Convert.ToDateTime(aux[0]);
                    data.dateData = Convert.ToDateTime(auxF);
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
                    //data.dateData = Convert.ToDateTime(aux[0]);
                    data.dateData = Convert.ToDateTime(auxF);
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

                var aux1 = aux[0].Split(' '); //SEPARO LA FECHA DE LA HORA
                var aux2 = aux1[0].Split('/'); //SEPARO MES/DIA/AÑO EN LA FECHA PARA LUEGO CONVERTIR A DIA/MES/AÑO
                var auxF = aux2[1] + "/" + aux2[0] + "/" + aux2[2] + " " + aux1[1];

                data.dateData = Convert.ToDateTime(auxF);

                //DateTime dt = DateTime.ParseExact(aux[0].ToString(), "MM/dd/yyyy h:mm", CultureInfo.InvariantCulture);                              
                //data.dateData = Convert.ToDateTime(s);     
                
                data.h_cm_ = Convert.ToDouble(aux[1]);
                data.flow_Q_l_s_ = Convert.ToDouble(aux[2]);
                data.idEnteredData = idDocument;
                db.hydrologicalDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    
    public static class HtmlHelperExtensions
    {
        public static string CurrentViewName(this HtmlHelper html)
        {
            return System.IO.Path.GetFileNameWithoutExtension(
                ((RazorView)html.ViewContext.View).ViewPath
            );
        }
    }
}