using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class CajaViewModel
    {
        public int idcaja { get; set; }

        public int id_mostrador { get; set; }

        public int id { get; set; }

        [Display(Name = "Nombre del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el nombre")]
        public string nombre { get; set; }

        [Display(Name = "Precio venta")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el precio del producto")]
        public double precio_venta { get; set; }

        [Display(Name = "Código del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el código")]
        public string codigo_prod { get; set; }

        [Display(Name = "Cantidad en el mostrador")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria la cantidad")]
        public int cant_prod_mostrador { get; set; }

        [Display(Name = "Cantidad a vender")]
        [Range(1, Int32.MaxValue, ErrorMessage = "El valor debe ser igual o mayor a 1")]
        public Nullable<int> cantidad_vendida { get; set; }


    }
}