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
                           where b.nombreProyecto_FK == a.nombre && e.nombreProy_FK == a.nombre && e.rol == true && e.cedulaEM_FK == l.cedulaED && a.fechaFinalizacion != null
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

    }
}
