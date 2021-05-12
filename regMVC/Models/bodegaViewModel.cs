using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class bodegaViewModel
    {
        [Display(Name = "Id en bodega")]
        public int id_bodega { get; set; }

        [Display(Name = "Cantidad de producto en el inventario")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria la cantidad")]
        public Nullable<int> cant_producto_inventario { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria una fecha de ingreso")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fecha_ingreso { get; set; }

        [Display(Name = "Código del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Necesario el código del producto")]
        public string cod_prod { get; set; }

        [Display(Name = "Cantidad para asignar al mostrador")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria la cantidad")]
        [Range(1, Int32.MaxValue, ErrorMessage = "El valor debe ser igual o mayor a 1")]
        public int cant_prod_mostrador { get; set; }
        
    }
}