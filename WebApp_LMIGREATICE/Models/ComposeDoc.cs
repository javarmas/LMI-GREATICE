using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models
{
    public class ComposeDoc
    {
        public string key { get; set; }
        public string rowName { get; set; }

        public ComposeDoc() { }

        public List<ComposeDoc> getMeteoStructure() {
            ComposeDoc col1 = new ComposeDoc() { key = "inShortWaveRadiation", rowName = "Incoming shortwave radiation" };
            ComposeDoc col2 = new ComposeDoc() { key = "outShortWaveRadiation", rowName = "Outgoing shortwave radiation" };
            ComposeDoc col3 = new ComposeDoc() { key = "albedo", rowName = "Albedo" };
            ComposeDoc col4 = new ComposeDoc() { key = "inLongWaveRadiation", rowName = "Incoming longwave radiation" };
            ComposeDoc col5 = new ComposeDoc() { key = "outLongWaveRadiation", rowName = "Outgoing longwave radiation" };
            ComposeDoc col6 = new ComposeDoc() { key = "relativeHumidity", rowName = "Relative humidity" };
            ComposeDoc col7 = new ComposeDoc() { key = "ventilatedAirTemperature", rowName = "Ventilated air temperature" };
            ComposeDoc col8 = new ComposeDoc() { key = "nonVentilatedAirTemperature", rowName = "Non ventilated air temperature" };
            ComposeDoc col9 = new ComposeDoc() { key = "windSpeed", rowName = "Wind speed" };
            ComposeDoc col10 = new ComposeDoc() { key = "windDirection", rowName = "Wind direction" };
            ComposeDoc col11 = new ComposeDoc() { key = "precipitationAmount", rowName = "Precipitation amount" };
            ComposeDoc col12 = new ComposeDoc() { key = "precipitationRate", rowName = "Precipitation rate over the past 30 minutes" };
            ComposeDoc col13 = new ComposeDoc() { key = "distanceSensor_Snow", rowName = "Distance sensor-snow" };
            ComposeDoc col14 = new ComposeDoc() { key = "snowDepth", rowName = "Snow depth" };
            ComposeDoc col15 = new ComposeDoc() { key = "groundFlux", rowName = "Ground atmosphere flux" };
            ComposeDoc col16 = new ComposeDoc() { key = "groundTemp_3cm", rowName = "Ground temperature -3cm" };
            ComposeDoc col17 = new ComposeDoc() { key = "groundTemp_10cm", rowName = "Ground temperature -10cm" };
            ComposeDoc col18 = new ComposeDoc() { key = "groundTemp_30cm", rowName = "Ground temperature -30cm" };

            List<ComposeDoc> aux = new List<ComposeDoc>();
            aux.Add(col1);
            aux.Add(col2);
            aux.Add(col3);
            aux.Add(col4);
            aux.Add(col5);
            aux.Add(col6);
            aux.Add(col7);
            aux.Add(col8);
            aux.Add(col9);
            aux.Add(col10);
            aux.Add(col11);
            aux.Add(col12);
            aux.Add(col13);
            aux.Add(col14);
            aux.Add(col15);
            aux.Add(col16);
            aux.Add(col17);
            aux.Add(col18);
            return aux;
        }

        public List<ComposeDoc> getGlacialStructure()
        {
            ComposeDoc col1 = new ComposeDoc() { key = "massBalance", rowName = "Mass balance" };
            List<ComposeDoc> aux = new List<ComposeDoc>();
            aux.Add(col1);
            return aux;
        }

        public List<ComposeDoc> getHidroStructure()
        {
            ComposeDoc col1 = new ComposeDoc() { key = "h_cm_", rowName = "level (cm)" };
            ComposeDoc col2 = new ComposeDoc() { key = "flow_Q_l_s_", rowName = "flow (lt/s)" };
            List<ComposeDoc> aux = new List<ComposeDoc>();
            aux.Add(col1);
            aux.Add(col2);
            return aux;
        }
    }
}