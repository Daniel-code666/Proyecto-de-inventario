using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string check_pass { get; set; }
    }

    public class UserMetadata
    {
        public int id { get; set; }

        [Display(Name = "Cedula")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el número de cedula")]
        public string cedula { get; set; }

        [Display(Name = "Nombre")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el nombre")]
        public string fname { get; set; }

        [Display(Name = "Apellido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el apellido")]
        public string lname { get; set; }

        [Display(Name = "Contraseña")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario una contraseña")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string pass { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [DataType(DataType.Password)]
        [Compare("pass", ErrorMessage = "Las Contraseñas no coinciden")]
        public string check_pass { get; set; }

        [Display(Name = "Correo")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario un correo")]
        [DataType(DataType.EmailAddress)]
        public string correo { get; set; }

    }
}