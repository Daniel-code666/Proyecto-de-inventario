//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace regMVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class productos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public productos()
        {
            this.bodega = new HashSet<bodega>();
        }

        public int id { get; set; }

        [Display(Name = "Nombre del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el nombre")]
        public string nombre { get; set; }

        [Display(Name = "Marca del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario una marca")]
        public string marca { get; set; }

        [Display(Name = "Precio compra")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el precio del producto")]
        [DataType(DataType.Currency)]
        public double precio_compra { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Categoría")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario una categoría")]
        public string categoria { get; set; }

        [Display(Name = "Precio venta")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el precio del producto")]
        [DataType(DataType.Currency)]
        public double precio_venta { get; set; }

        [Display(Name = "Grupo alimenticio")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el grupo para alimentos")]
        public string grupo { get; set; }


        [Display(Name = "Código del producto")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el código")]
        public string codigo_prod { get; set; }

        [Display(Name = "Fecha de expiración")]
        [DataType(DataType.Date)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria una fecha de expiración")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fecha_expiracion { get; set; }

        [Display(Name = "Cantidad de producto en el inventario")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria la cantidad")]
        [Range(1, Int32.MaxValue, ErrorMessage = "El valor debe ser igual o mayor a 1")]
        public Nullable<int> cant_producto_inventario { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesaria una fecha de ingreso")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fecha_ingreso { get; set; }
        public string cedula { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<bodega> bodega { get; set; }
        public virtual user user { get; set; }
    }
}
