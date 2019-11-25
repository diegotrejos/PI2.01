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
using Newtonsoft.Json;

namespace Proyecto.Controllers
{
    public class ReporteriaController : Controller
    {
        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.EmpleadoDesarrolladorController emplController = new Proyecto.Controllers.EmpleadoDesarrolladorController();
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;
            return View();
        }


        public ActionResult TiemposRequerimiento()
        {
            ViewBag.nombreP = "Seleccione un proyecto";
            /* var item = (from a in db.Requerimiento
                         from b in db.EmpleadoDesarrollador
                         where a.cedulaResponsable_FK == b.cedulaED
                         select new { nombre = b.nombreED, req = a.nombre, suma = a.duracionEstimada - a.duracionReal });*/
            List<string> lista = new List<string>();
            /* foreach (var dato in item)
             {
                 lista.Add(dato.nombre + " " + dato.req + " " + dato.suma);
             }*/

            return View(lista);
        }

        [HttpPost]
        public ActionResult TiemposRequerimiento(string nombre, string nombreProyecto)
        {
            ViewBag.todos = "";
            ViewBag.nombreP = "Seleccione un proyecto";
            var cedula = from a in db.EmpleadoDesarrollador
                         where a.nombreED == nombre
                         select a;
            var item = (from a in db.Requerimiento
                        where a.cedulaResponsable_FK == cedula.FirstOrDefault().cedulaED && a.nombreProyecto_FK == nombreProyecto
                        select new { nombre = a.nombreProyecto_FK, complejidad = a.complejidad,req = a.nombre, duracionEst = a.duracionEstimada, duracionReal = a.duracionReal, diferencia = a.duracionEstimada - a.duracionReal });

            if (nombreProyecto == "" || nombreProyecto == "Todos los proyectos") {
                ViewBag.todos = "si";
                ViewBag.nombreP = "Todos los proyectos";
               item = (from a in db.Requerimiento
                    where a.cedulaResponsable_FK == cedula.FirstOrDefault().cedulaED
                    select new { nombre = a.nombreProyecto_FK, complejidad = a.complejidad, req = a.nombre, duracionEst = a.duracionEstimada, duracionReal = a.duracionReal, diferencia = a.duracionEstimada - a.duracionReal });

            }

            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                if (nombreProyecto != "Todos los proyectos")
                    ViewBag.nombreP = dato.nombre;
                lista.Add(dato.nombre + "," + dato.req + "," +dato.complejidad + "," + dato.duracionEst + "," + dato.duracionReal + "," +dato.diferencia );
            }

            return View(lista);
        }

        public ActionResult HorasProyecto()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;

            var item =
                           from b in db.Requerimiento
                           from a in db.Proyecto
                           from e in db.Equipo
                           from l in db.EmpleadoDesarrollador
                           where b.nombreProyecto_FK == a.nombre && e.nombreProy_FK == a.nombre && e.rol == true && e.cedulaEM_FK == l.cedulaED 
                           group new { a,b, e, l } by a.nombre into c
                           select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
            if (usuario == "Lider") {
                item =
                         from b in db.Requerimiento
                         from a in db.Proyecto
                         from e in db.Equipo
                         from l in db.EmpleadoDesarrollador
                         where b.nombreProyecto_FK == a.nombre  && e.cedulaEM_FK == cedula && e.rol == true && e.nombreProy_FK == a.nombre  &&  a.fechaFinalizacion != null
                         group new { a, b, e, l } by a.nombre into c
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };

            }
            if (item != null)
                item.Distinct();
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + ","+dato.lider+ "," + dato.sumaEst + "," + dato.sumaReal + "," + dato.dif);
            }

            return View(lista);
        }

        [HttpPost]
        public ActionResult HorasProyecto(string nombreProyecto)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            var item =
                           from b in db.Requerimiento
                           from a in db.Proyecto
                           from e in db.Equipo
                           from l in db.EmpleadoDesarrollador
                           where a.nombre == nombreProyecto && b.nombreProyecto_FK == nombreProyecto && e.nombreProy_FK == nombreProyecto && e.rol == true && e.cedulaEM_FK == l.cedulaED
                           group new { a, b, e, l } by a.nombre into c
                           select new { nombre = c.FirstOrDefault().a.nombre, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
            if (item != null)
                item.Distinct();
            if (nombreProyecto =="") {
                 item =
                         from b in db.Requerimiento
                         from a in db.Proyecto
                         from e in db.Equipo
                         from l in db.EmpleadoDesarrollador
                         where b.nombreProyecto_FK == a.nombre && e.nombreProy_FK == a.nombre && e.rol == true && e.cedulaEM_FK == l.cedulaED
                         group new { a, b, e, l } by a.nombre into c
                         select new { nombre = c.FirstOrDefault().b.nombreProyecto_FK, sumaEst = c.Sum(a => a.b.duracionEstimada), sumaReal = c.Sum(d => d.b.duracionReal), dif = c.Sum(a => a.b.duracionEstimada) - c.Sum(d => d.b.duracionReal), lider = c.FirstOrDefault().l.nombreED };
                if (item != null)
                    item.Distinct();
            }
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + "," + dato.lider + "," + dato.sumaEst + "," + dato.sumaReal + "," + dato.dif);
            }

            return View(lista);
        }

     

        public SelectList getDesarrolladores()
        {
   
            return emplController.getDesarrolladores();
        }

        [HttpPost]
        public JsonResult getProyectoDesarrollador(string nombre)
        {
            //System.Diagnostics.Debug.WriteLine("el nombre" + nombre);
            var empleado = from a in db.EmpleadoDesarrollador
                           where a.nombreED == nombre
                           select a.cedulaED;
       
            var item = from a in db.Proyecto
                       from b in db.Equipo
                       where a.nombre == b.nombreProy_FK && a.fechaFinalizacion != null
                       where b.cedulaEM_FK == empleado.FirstOrDefault()
                       select a.nombre;
            if (item != null)
                item.Distinct();
            return  Json(new SelectList(item));
        }

        public SelectList getProyectos()
        {
            return proyController.getProyectosPorRol();
        }


        //ya terminado
        public ActionResult Estadodesarrollorequerimientos()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string nombreProyecto = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;

            if (usuario == "Desarrollador")
            { var query = from a in db.Requerimiento
                              //solo requerimientos en ejecucion o sin iniciar
                          where (a.nombreProyecto_FK == nombreProyecto)
                          where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                          where (a.cedulaResponsable_FK == cedula)
                          select a;
                return View(query.ToList());
            }



            else if (usuario == "Lider")
            {
                var query = from a in db.Requerimiento
                                //solo requerimientos en ejecucion o sin iniciar
                            where (a.nombreProyecto_FK == nombreProyecto)
                            where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                            select a;
                return View(query.ToList());
            }

            else
            {
                var query = from a in db.Requerimiento
                                //solo requerimientos en ejecucion o sin iniciar
                            where (a.estado == "En ejecucion" || a.estado == "Sin iniciar")
                            select a;
                return View(query.ToList());
            }
        }


    //objeto para imprimir resultados bonitos
        public class Desocupacion
            {
            public
            string nombreED;
            public
           string periodosyDias;
            public
           int total;

        }


        //en progreso
        public ActionResult PeriodosDisponibles()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            ViewBag.user = usuario;
            List<Desocupacion> Empleados = new List<Desocupacion>();

            Desocupacion p = new Desocupacion();
            p.nombreED = "Diego";
            p.periodosyDias = "1/02/2006-2/03/2006 = 32";
            p.total = 32;
            //los agrego a la lista 
            Empleados.Add(p);
            return View(Empleados);
           
        }

        [HttpPost]
        public ActionResult PeriodosDisponibles(string Finicio, string Ffinal)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            List<Desocupacion> Empleados = new List<Desocupacion>();
            Finicio = "10/04/2005";
            Ffinal = "2/12/2006";



            //Genero lista de todos los empleados


            //Hago consulta aqui de sus datos
            for (int i = 0; i < 2; i++)
            {
                Desocupacion p = new Desocupacion();
                p.nombreED = "Diego";
                p.periodosyDias = "1/02/2006-2/03/2006 = 32";
                p.total = 32;
                //los agrego a la lista 
                Empleados.Add(p);
            }


            return View(Empleados);
        }


    }
}
