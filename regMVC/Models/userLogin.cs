using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class userLogin
    {
        /*[Display(Name = "Nombre")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el nombre")]
        public string fname { get; set; }

        [Display(Name = "Apellido")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el apellido")]
        public string lname { get; set; }*/

        [Display(Name = "Correo")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el correo")]
        public string correo { get; set; }

        public int id { get; set; }

        [Display(Name = "Constraseña")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario la contraseña")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string pass { get; set; }

        [Display(Name = "Recordar mi ususario")]
        public bool RememberMe { get; set; }
    }
}