using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class EconomiaViewModel
    {
        [Display(Name = "Dinero total del local")]
        [DataType(DataType.Currency)]
        public Nullable<double> dinero_total { get; set; }

        public int id_economia { get; set; }

    }
}