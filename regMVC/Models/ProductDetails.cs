using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class ProductDetails
    {
        public int id { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Marca")]
        public string marca { get; set; }

        [Display(Name = "Precio de compra")]
        [DataType(DataType.Currency)]
        public double precio_compra { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Categoría")]
        public string categoria { get; set; }

        [Display(Name = "Precio de venta")]
        [DataType(DataType.Currency)]
        public double precio_venta { get; set; }

        [Display(Name = "Grupo alimenticio")]
        public string grupo { get; set; }

        [Display(Name = "Código de producto")]
        public string codigo_prod { get; set; }

        [Display(Name = "Fecha de expiración")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fecha_expiracion { get; set; }

        [Display(Name = "Cantidad en inventario")]
        public Nullable<int> cant_producto_inventario { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fecha_ingreso { get; set; }

        [Display(Name = "Cantidad en mostrador")]
        public int cant_prod_mostrador { get; set; }

    }
}