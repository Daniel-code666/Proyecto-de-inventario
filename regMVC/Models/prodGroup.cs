using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace regMVC.Models
{
    public class prodGroup
    {
        public string grupoSeleccionado { get; set; }

        public List<SelectListItem> grupos { get; set; } = new List<SelectListItem>();
        public List<productos> ResultadosBusqueda { get; set; } = new List<productos>();

    }
}