using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class userRecoverPass
    {

        [Display(Name = "Cedula")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario el número de cedula")]
        public string cedula { get; set; }

        [Display(Name = "Nueva Contraseña")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Es necesario una contraseña")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string newpass { get; set; }
    }
}