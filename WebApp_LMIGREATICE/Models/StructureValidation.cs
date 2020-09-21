using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models
{
    public class StructureValidation
    {
        public Holder checkMeteorologicalStructure(StreamReader csvReader, enteredData enteredData, station selectedStation) //REVISAR DOCS METEOROLOGICOS
        {
            Holder hold = new Holder();
            int rowNumber = 1;
            string[] aux = null;
            string error = null;
            bool auxStatus = true;

            while (!csvReader.EndOfStream)
            {
                string reader = csvReader.ReadLine();
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();

                if (selectedStation.idLocation == 1) //LA ESTACIÓN SELECCIONADA ESTÁ DENTRO DEL GLACIAR
                {
                    if (rowNumber == 13) //VALIDACION DE NOMBRES DE COLUMNAS PARA DATOS METEOROLÓGICOS (Inside Glacier)
                    {
                        if (aux[0].ToString() == " #date (GMT+ 0)" && aux[1].ToString() == "Swinc" && aux[2].ToString() == "Swout"
                            && aux[3].ToString() == "albedo" && aux[4].ToString() == "RH" && aux[5].ToString() == "T"
                            && aux[6].ToString() == "wind_speed" && aux[7].ToString() == "wind_direction")
                        {
                            auxStatus = true;
                        }
                        else
                        {
                            auxStatus = false;
                            error = "ROW " + rowNumber + ": COLUMN NAMES ARE INCORRECT, PLEASE VERIFY THEM";
                        }
                    }
                    else if (rowNumber >= 14) //VALIDACIÓN DATOS METEOROLÓGICOS (Inside Glacier)
                    {
                        //VALIDACION AUX[0]
                        if (aux[0] != null && validateDates(aux[0]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST BE DATES (dd/MM/yyyy HH:mm or dd/MM/yyyy H:mm)";
                        }

                        //VALIDACION AUX[1]
                        if (aux[1] != null && validateNumbers(aux[1]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SECOND COLUMN (Swinc) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[2]
                        if (aux[2] != null && validateNumbers(aux[2]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON THIRD COLUMN (Swout) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[3]
                        if (aux[3] != null && validateNumbers(aux[3]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FOURTH COLUMN (albedo) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[4]
                        if (aux[4] != null && validateNumbers(aux[4]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIFTH COLUMN (RH) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[5]
                        if (aux[5] != null && validateNumbers(aux[5]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SIXTH COLUMN (T) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[6]
                        if (aux[6] != null && validateNumbers(aux[6]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SEVENTH COLUMN (wind_speed) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[7]
                        if (aux[7] != null && validateNumbers(aux[7]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON EIGHTH COLUMN (wind_direction) MUST BE NUMBERS";
                        }
                    }
                }
                else //LA ESTACIÓN SELECCIONADA ESTÁ FUERA DEL GLACIAR, TIENE OTRA ESTRUCTURA
                {
                    if (rowNumber == 22) //VALIDACION DE NOMBRES DE COLUMNAS PARA DATOS METEOROLÓGICOS (Outside Glacier)
                    {
                        if (aux[0].ToString() == " #date (GMT + 0)" && aux[1].ToString() == "Swinc" && aux[2].ToString() == "Swout"
                            && aux[3].ToString() == "albedo" && aux[4].ToString() == "Lwinc" && aux[5].ToString() == "Lwout"
                            && aux[6].ToString() == "RH" && aux[7].ToString() == "T" && aux[8].ToString() == "Tnv"
                            && aux[9].ToString() == "wind_speed" && aux[10].ToString() == "wind_direction" && aux[11].ToString() == "Precipitation_Amount"
                            && aux[12].ToString() == "Precipitation_Rate" && aux[13].ToString() == "H_above_snow" && aux[14].ToString() == "Snow_depth"
                            && aux[15].ToString() == "Ground_flux" && aux[16].ToString() == "T_-3cm" && aux[17].ToString() == "T_-10cm"
                            && aux[18].ToString() == "T_-30cm")
                        {
                            auxStatus = true;
                        }
                        else
                        {
                            auxStatus = false;
                            error = "ROW " + rowNumber + ": COLUMN NAMES ARE INCORRECT, PLEASE VERIFY THEM";
                        }
                    }
                    else if (rowNumber >= 23) //VALIDACIÓN DATOS METEOROLÓGICOS (Outside Glacier)
                    {
                        //VALIDACION AUX[0]
                        if (aux[0] != null && validateDates(aux[0]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST BE DATES (dd/MM/yyyy HH:mm or dd/MM/yyyy H:mm)";
                        }

                        //VALIDACION AUX[1]
                        if (aux[1] != null && validateNumbers(aux[1]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SECOND COLUMN (Swinc) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[2]
                        if (aux[2] != null && validateNumbers(aux[2]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON THIRD COLUMN (Swout) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[3]
                        if (aux[3] != null && validateNumbers(aux[3]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FOURTH COLUMN (albedo) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[4]
                        if (aux[4] != null && validateNumbers(aux[4]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIFTH COLUMN (Lwinc) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[5]
                        if (aux[5] != null && validateNumbers(aux[5]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SIXTH COLUMN (Lwout) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[6]
                        if (aux[6] != null && validateNumbers(aux[6]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SEVENTH COLUMN (RH) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[7]
                        if (aux[7] != null && validateNumbers(aux[7]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON EIGHTH COLUMN (T) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[8]
                        if (aux[8] != null && validateNumbers(aux[8]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON NINTH COLUMN (Tnv) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[9]
                        if (aux[9] != null && validateNumbers(aux[9]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON TENTH COLUMN (wind_speed) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[10]
                        if (aux[10] != null && validateNumbers(aux[10]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON ELEVENTH COLUMN (wind_direction) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[11]
                        if (aux[11] != null && validateNumbers(aux[11]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON TWELFTH COLUMN (Precipitation_Amount) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[12]
                        if (aux[12] != null && validateNumbers(aux[12]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON THIRTEENTH COLUMN (Precipitation_Rate) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[13]
                        if (aux[13] != null && validateNumbers(aux[13]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FOURTEENTH COLUMN (H_above_snow) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[14]
                        if (aux[14] != null && validateNumbers(aux[14]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIFTEENTH COLUMN (Snow_depth) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[15]
                        if (aux[15] != null && validateNumbers(aux[15]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SIXTEENTH COLUMN (Ground_flux) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[16]
                        if (aux[16] != null && validateNumbers(aux[16]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SEVENTEENTH COLUMN (T_-3cm) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[17]
                        if (aux[17] != null && validateNumbers(aux[17]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON EIGHTEENTH COLUMN (T_-10cm) MUST BE NUMBERS";
                        }

                        //VALIDACION AUX[18]
                        if (aux[18] != null && validateNumbers(aux[18]))
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON NINETEENTH COLUMN (T_-30cm) MUST BE NUMBERS";
                        }
                    }
                }
                rowNumber++;
            }
            hold.status = auxStatus;
            hold.message = error;
            return hold;
        }

        public Holder checkGlaciologicalStructure(StreamReader csvReader) //REVISAR DOCS GLACIOLOGICOS
        {
            Holder hold = new Holder();
            int rowNumber = 1;
            string[] aux = null;
            string error = null;
            bool auxStatus = true;

            while (!csvReader.EndOfStream)
            {
                string reader = csvReader.ReadLine();
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                if (rowNumber == 2) //VALIDACION DE NOMBRES DE COLUMNAS PARA DATOS GLACIOLÓGICOS
                {
                    if (aux[0].ToString() == "#station_name" && aux[1].ToString() == "jj_debut" && aux[2].ToString() == "mm_debut"
                        && aux[3].ToString() == "aaaa_debut" && aux[4].ToString() == "jj_fin" && aux[5].ToString() == "mm_fin"
                            && aux[6].ToString() == "aaaa_fin" && aux[7].ToString() == "lat_wgs84" && aux[8].ToString() == "lon_wgs84"
                                && aux[9].ToString() == "altitude" && aux[10].ToString() == "BM_par_tranche_altitude" && aux[11].ToString() == "mois")
                    {
                        auxStatus = true;
                    }
                    else
                    {
                        auxStatus = false;
                        error = "ROW " + rowNumber + ": COLUMN NAMES ARE INCORRECT, PLEASE VERIFY THEM";
                        //break;
                    }
                }
                else if (rowNumber >= 3) //VALIDACION DE DATOS GLACIOLÓGICOS
                {
                    //VALIDACION AUX[0]
                    try
                    {
                        var aux2 = aux[0].Split('_');
                        if (aux2[0].Length == 4 && aux2[1].Length == 4)
                        {
                            if (validateDigits(aux2[0]) == true && validateDigits(aux2[1]))
                            {
                                auxStatus = (auxStatus && true);
                            }
                            else
                            {
                                auxStatus = (auxStatus && false);
                                error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST HAVE A FORMAT LIKE THIS xxxx_xxxx (x must be an integer)";
                                // break;
                            }
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST HAVE A LENGTH OF 9 CHARACTERS AND A FORMAT LIKE THIS xxxx_xxxx (x must be a number)";
                            //break;
                        }
                    }
                    catch
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST HAVE A LENGTH OF 9 CHARACTERS AND A FORMAT LIKE THIS xxxx_xxxx (x must be a number)";
                        //break;
                    }

                    //VALIDACION AUX[1]
                    if (aux[1] != null && validateNumbers(aux[1]) && Convert.ToInt32(aux[1]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SECOND COLUMN (START DAY - JOUR DE DEBUT) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[2]
                    if (aux[2] != null && validateNumbers(aux[2]) && Convert.ToInt32(aux[2]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON THIRD COLUMN (START MONTH - MOIS DE DEBUT) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[3]
                    if (aux[3] != null && validateNumbers(aux[3]) && Convert.ToInt32(aux[3]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FOURTH COLUMN (START YEAR - ANNÉE DE DEBUT) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[4]
                    if (aux[4] != null && validateNumbers(aux[4]) && Convert.ToInt32(aux[4]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIFTH COLUMN (END DAY - JOUR FINAL) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[5]
                    if (aux[5] != null && validateNumbers(aux[5]) && Convert.ToInt32(aux[5]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SIXTH COLUMN (END MONTH - MOIS FINAL) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[6]
                    if (aux[6] != null && validateNumbers(aux[6]) && Convert.ToInt32(aux[6]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SEVENTH COLUMN (END YEAR - ANNÉEE FINALE) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[7]
                    if (aux[7] != null && validateNumbers(aux[7])) //LATITUD
                    {
                        if (Convert.ToDouble(aux[7]) >= -90 && Convert.ToDouble(aux[7]) <= 90)
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON EIGHTH COLUMN (LATITUDE) MUST BE BETWEEN -90 AND 90";
                        }
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON EIGHTH COLUMN (LATITUDE) MUST BE NUMBERS, NOT NULL";
                    }

                    //VALIDACION AUX[8]
                    if (aux[8] != null && validateNumbers(aux[8])) //LONGITUD
                    {
                        if (Convert.ToDouble(aux[8]) >= -180 && Convert.ToDouble(aux[8]) <= 180)
                        {
                            auxStatus = (auxStatus && true);
                        }
                        else
                        {
                            auxStatus = (auxStatus && false);
                            error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON NINTH COLUMN (LONGITUDE) MUST BE BETWEEN -180 AND 180";
                        }
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON NINTH COLUMN (LONGITUDE) MUST BE NUMBERS, NOT NULL";
                    }

                    //VALIDACION AUX[9]
                    if (aux[9] != null && validateNumbers(aux[9]) && Convert.ToInt32(aux[9]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON TENTH COLUMN (ALTITUDE) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }

                    //VALIDACION AUX[10]
                    if (aux[10] != null && validateNumbers(aux[10]))
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON ELEVENTH COLUMN (MASS BALANCE) MUST BE NUMBERS, NOT NULL";
                    }

                    //VALIDACION AUX[11]
                    if (aux[11] != null && validateNumbers(aux[11]) && Convert.ToInt32(aux[11]) != 0)
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON TWELFTH COLUMN (MONTH - MOIS) MUST BE NUMBERS, NOT NULL AND DIFFERENT FROM 0";
                    }
                }
                rowNumber++;
            }
            hold.status = auxStatus;
            hold.message = error;
            return hold;
        }

        public Holder checkHydrologicalStructure(StreamReader csvReader) //REVISAR DOCS HIDROLÓGICOS
        {
            Holder hold = new Holder();
            int rowNumber = 1;
            string[] aux = null;
            string error = null;
            bool auxStatus = true;

            while (!csvReader.EndOfStream)
            {
                string reader = csvReader.ReadLine();
                aux = reader.Split(';').Where(x => !String.IsNullOrEmpty(x)).ToArray();
                if (rowNumber == 1) //VALIDACION DE NOMBRES DE COLUMNAS PARA DATOS HIDROLÓGICOS
                {
                    if (aux[0].ToString() == "Fecha" && aux[1].ToString() == "cotas(cm)" && aux[2].ToString() == "Q(lt/s)")
                    {
                        auxStatus = true;
                    }
                    else
                    {
                        auxStatus = false;
                        error = "ROW " + rowNumber + ": COLUMN NAMES ARE INCORRECT, PLEASE VERIFY THEM";
                        //break;
                    }
                }
                else if (rowNumber >= 2) //VALIDACIÓN DATOS HIDROLÓGICOS
                {
                    //VALIDACION AUX[0]
                    if (aux[0] != null && validateDates(aux[0]))
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON FIRST COLUMN MUST BE DATES (dd/MM/yyyy HH:mm or dd/MM/yyyy H:mm)";
                    }

                    //VALIDACION AUX[1]
                    if (aux[1] != null && validateNumbers(aux[1]))
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON SECOND COLUMN (cotas(cm)) MUST BE NUMBERS";
                    }

                    //VALIDACION AUX[2]
                    if (aux[2] != null && validateNumbers(aux[2]))
                    {
                        auxStatus = (auxStatus && true);
                    }
                    else
                    {
                        auxStatus = (auxStatus && false);
                        error += "\n ROW" + " " + rowNumber + ":" + "VALUES ON THIRD COLUMN (Q(lt/s))) MUST BE NUMBERS";
                    }
                }
                rowNumber++;
            }

            hold.status = auxStatus;
            hold.message = error;
            return hold;
        }

        public bool validateDigits(string forValidate) //Valida si los valores pasados son números (dígitos)
        {
            bool isNumber = false;

            foreach (char item in forValidate.ToCharArray())
            {
                if (char.IsDigit(item))
                {
                    isNumber = true;
                }
                else
                {
                    isNumber = false;
                    //break;
                }
            }
            return isNumber;
        }

        public bool validateNumbers(string forValidate) //Valida si los valores pasados son números 
        {
            bool isNumber = false;
            double test;
            try
            {
                if (double.TryParse(forValidate, out test))
                {
                    isNumber = true;
                }
            }
            catch
            {
                isNumber = false;
            }
            return isNumber;
        }

        public bool validateDates(string forValidate) //Valida formato de fecha y hora
        {
            DateTime date;
            bool isValid = false;

            try
            {
                if (DateTime.TryParseExact(forValidate, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out date) ||
                DateTime.TryParseExact(forValidate, "dd/MM/yyyy H:mm", null, System.Globalization.DateTimeStyles.None, out date))
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            }
            catch
            {
                isValid = false;
            }
            return isValid;
        }
    }
}