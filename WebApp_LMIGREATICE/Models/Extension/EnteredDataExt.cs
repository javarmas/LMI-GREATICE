using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models.Extension
{
    public class EnteredDataExt : enteredData
    {
        public String variable {get;set;}

        public int year { get; set; }

        public int idMeasurementType { get; set; }

        public String altitudinalBar { get; set; }
        
    }
}