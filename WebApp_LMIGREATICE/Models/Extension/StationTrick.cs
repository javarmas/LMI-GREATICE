using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models.Extension
{
    public class StationTrick
    {
        public int idStation { get; set; }
        public string nameStation { get; set; }
        public int idLocation { get; set; }
        public decimal latitudeStation { get; set; }
        public decimal longitudeStation { get; set; }
        public double altitudeStation { get; set; }
        public int idGlacier { get; set; }
        public int idMeasurementType { get; set; }
    }
}