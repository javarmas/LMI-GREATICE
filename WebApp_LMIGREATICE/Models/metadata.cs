//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApp_LMIGREATICE.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class metadata
    {
        public int idMetaData { get; set; }
        public string variable_code { get; set; }
        public string variable_name { get; set; }
        public string sensor_manufacturer { get; set; }
        public string sensor_model { get; set; }
        public string unit { get; set; }
        public string dataQuality { get; set; }
        public int idEnteredData { get; set; }
    
        public virtual enteredData enteredData { get; set; }
    }
}
