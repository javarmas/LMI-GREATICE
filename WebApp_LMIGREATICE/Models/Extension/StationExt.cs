using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models.Extension
{
    public class StationExt:station
    {
        public List<EnteredDataSimplify> docs { get; set; }
    }
}