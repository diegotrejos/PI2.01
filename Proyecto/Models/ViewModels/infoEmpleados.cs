using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto.Models.ViewModels
{
    public class infoEmpleados
    {
        public Equipo equipo { get; set; }
        public List<Requerimiento> requerimientos { get; set; }
        public DateTime durEstimada { get; set; }
        public int cantidadReq { get; set; }
    }
}