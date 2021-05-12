using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace regMVC.Models
{
    public class ProductandBodViewModel
    {
        public List<productos> prod { get; set; }

        public List<bodega> bod { get; set; }

        public List<mostrador> mostrador { get; set; }

        public List<economia> economic { get; set; }
    }
}