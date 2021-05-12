using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace regMVC.Models
{
    public class prodCategory
    {
        public string categoriaSeleccionada { get; set; }

        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public List<productos> SearchResults { get; set; } = new List<productos>();

        public string cod_prod { get; set; }

        public List<productos> prod { get; set; }

        public List<bodega> bod { get; set; }
    }
}