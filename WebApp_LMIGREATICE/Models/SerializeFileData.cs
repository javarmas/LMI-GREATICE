using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp_LMIGREATICE.Models
{
    public class SerializeFileData
    {
        public HttpPostedFileBase file { get;set;}
        public enteredData entered { get; set; }
    }
}