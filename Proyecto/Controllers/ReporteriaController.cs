using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Windows;

namespace Proyecto.Controllers
{

    public class info_empleados
    {
        public Equipo equipo { get;set;}
        public List<Requerimiento> requerimientos { get; set; }
        public DateTime durEstimada { get; set;}
        public int longitud { get; set; }
    }




    public class ReporteriaController : Controller
    {
        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            return View();
        }

        public ActionResult DesarrolladoresProyDis()
        {
            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            
            TempData["empleadosDisponibles"] = empleados;
            List<info_empleados> lista_datos = new List<info_empleados>();

            int longitudArray = new EmpleadoDesarrolladorController().getNoDisponibles();


            info_empleados[] arrayInfo = new info_empleados[longitudArray];

            arrayInfo[1].longitud = longitudArray;

            new EquipoController().llenarArray(arrayInfo);

            new RequerimientoController().llenarArray(arrayInfo);

            calculoDurEstimada(arrayInfo,longitudArray);

            ordenamientoElka(arrayInfo,longitudArray);

            ViewBag.info = arrayInfo;
            return View();
        }

        private void calculoDurEstimada(info_empleados[] info, int longitud)
        {
            for (int i = 0; i < longitud; ++i)
            {
                int totalHoras = 0;
                foreach (var item in info[i].requerimientos)
                {
                    totalHoras += item.duracionEstimada;
                }
                totalHoras = totalHoras / 8;
                info[i].durEstimada.AddDays(totalHoras);
            }
        }

        private void ordenamientoElka(info_empleados[] info, int longitud) //puede ser que tenga que devolver una array pero por ahora lo dejare void 
        {

        }


      
    }
}
