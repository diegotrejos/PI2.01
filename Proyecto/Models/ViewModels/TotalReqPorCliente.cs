using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models.ViewModels
{
    public class TotalReqPorCliente
    {
        public int total { get; set; }
        public string estado { get; set; }
        public string nombreCliente { get; set; }
        public string apellidoCliente { get; set; }
        public string nombreProy { get; set; }
        public DateTime durEstimada { get; set; }
    }
}