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

        public ActionResult Historial()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            var item = from r in db.Requerimiento
                       from p in db.Proyecto
                       from e in db.Equipo
                       from em in db.EmpleadoDesarrollador
                       where p.nombre == r.nombreProyecto_FK &&  r.cedulaResponsable_FK == em.cedulaED && p.fechaFinalizacion!= null
                       && e.nombreProy_FK == p.nombre && e.cedulaEM_FK == em.cedulaED
                       group new { r, p, e, em } by p.nombre into c
                       select new { nombreP = c.FirstOrDefault().p.nombre, nombre = c.FirstOrDefault().em.nombreED, rol = c.FirstOrDefault().e.rol ? "Lider" : "Desarrollador", sumaReal = c.Sum(a => a.r.duracionReal)/*,count= c.Count(), success= c.Where(a => a.r.estado == "Finalizado" ).Count()*/};

            List<string> lista = new List<string>();
           
            foreach (var dato in item)
            {
                if (dato.rol != "Lider")
                    lista.Add(dato.nombreP + "," + dato.nombre + "," + dato.rol + "," + dato.sumaReal);
                else
                    lista.Add(dato.nombreP + "," + dato.nombre + "," + dato.rol + "," + " ");
            }
            return View(lista);
        }

        [HttpPost]
        public ActionResult Historial(string nombre)
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            ViewBag.user = usuario;

            var item =
                    from r in db.Requerimiento
                    from p in db.Proyecto
                    from e in db.Equipo
                    from em in db.EmpleadoDesarrollador
                    where  p.nombre == r.nombreProyecto_FK  && em.nombreED == nombre && r.cedulaResponsable_FK == em.cedulaED && p.fechaFinalizacion != null
                    && e.nombreProy_FK == p.nombre && e.cedulaEM_FK == em.cedulaED
                    group new { r, p ,e, em} by p.nombre into c
                    select new {nombreP = c.FirstOrDefault().p.nombre , nombre = c.FirstOrDefault().em.nombreED, rol = c.FirstOrDefault().e.rol?"Lider":"Desarrollador", sumaReal = c.Sum(a => a.r.duracionReal)/*,count= c.Count(), success= c.Where(a => a.r.estado == "Finalizado" ).Count()*/};
            if (nombre == "" && usuario=="Jefe")
            {
                item = from r in db.Requerimiento
                       from p in db.Proyecto
                       from e in db.Equipo
                       from em in db.EmpleadoDesarrollador
                       where p.nombre == r.nombreProyecto_FK && r.cedulaResponsable_FK == em.cedulaED
                        && e.nombreProy_FK == p.nombre && e.cedulaEM_FK == em.cedulaED
                       group new { r, p, e, em } by p.nombre into c
                       select new { nombreP = c.FirstOrDefault().p.nombre, nombre = c.FirstOrDefault().em.nombreED, rol = c.FirstOrDefault().e.rol ? "Lider" : "Desarrollador", sumaReal = c.Sum(a => a.r.duracionReal)/*,count= c.Count(), success= c.Where(a => a.r.estado == "Finalizado" ).Count()*/};

            }
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                if (dato.rol != "Lider")
                    lista.Add(dato.nombreP + "," + dato.nombre+"," + dato.rol +"," + dato.sumaReal);
                else
                    lista.Add(dato.nombreP + "," + dato.nombre + "," + dato.rol + "," + " ");
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
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            if (usuario == "Desarrollador" || usuario == "Lider") //Si es Lider o Desarrollador se devuelve el mismo proyecto dependiendo de su cédula
            {
                var obj = from a in db.Proyecto
                          from b in db.Equipo
                          where a.nombre == b.nombreProy_FK
                          where b.cedulaEM_FK == cedula
                          select a.nombre;

                return new SelectList(obj);

            }
            else if (usuario == "Cliente") //El cliente ve solo sus proyecto
            {
                var obj = from a in db.Proyecto
                          where a.cedulaCliente == cedula
                          select a.nombre;
                return new SelectList(obj);
            }
            else if (usuario == "Jefe")
            { //Como el Jefe puede ver todo, se le muestran todos los proyectos
                var obj = from a in db.Proyecto
                          select a.nombre;
                return new SelectList(obj);
            }
            SelectList lista = new SelectList("");
            return lista;
        }


    }
}
