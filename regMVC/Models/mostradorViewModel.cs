using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace regMVC.Models
{
    public class mostradorViewModel
    {
        public string prodSeleccionado { get; set; }

        public List<SelectListItem> Products { get; set; } = new List<SelectListItem>();

        public List<productos> Resultados { get; set; } = new List<productos>();

        public int cant_prod_mostrador { get; set; }
    }
}