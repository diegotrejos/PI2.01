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


            //info_empleados[] arrayInfo = new info_empleados[longitudArray];

            //arrayInfo[1].longitud = longitudArray;

            new EquipoController().llenarArray(lista_datos);

            new RequerimientoController().llenarArray(lista_datos);

            calculoDurEstimada(lista_datos,longitudArray);

            //ordenamientoElka(lista_datos,longitudArray);

            TempData["datos"] = lista_datos;
            return View();
        }

        private void calculoDurEstimada(List<info_empleados> info, int longitud)
        {
            foreach (var item1 in info)
            {
                int totalHoras = 0;
                foreach (var item in item1.requerimientos)
                {
                    totalHoras += item.duracionEstimada;
                }
                totalHoras = totalHoras / 8;
                item1.durEstimada.AddDays(totalHoras);
            }
        }

        /*
        private void ordenamientoElka(info_empleados[] info, int longitud) //puede ser que tenga que devolver una array pero por ahora lo dejare void 
        {

        }

    */
      
    }
}
