using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models.Extension
{
    public class EnteredDataSimplify
    {
        public int idDoc { get; set; }
        public int idStation { get; set; }
        public string name { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }
}