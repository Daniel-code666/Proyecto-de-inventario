using regMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace regMVC.Controllers
{
    public class EconomiaController : Controller
    {
        //cálculo de los gastos y listado de los productos con sus respectivos precios
        
        public ActionResult P_List()
        {
            using(inventarioEntitiesDBA dc = new inventarioEntitiesDBA())
            {
                var p = dc.productos.ToList();

                var b = dc.bodega.ToList();

                var m = dc.mostrador.ToList();

                var e = dc.economia.ToList();

                ProductandBodViewModel finalitem = new ProductandBodViewModel();

                finalitem.prod = p;

                finalitem.bod = b;

                finalitem.mostrador = m;

                finalitem.economic = e;

                return View(finalitem);
            } 
        }
    }
}