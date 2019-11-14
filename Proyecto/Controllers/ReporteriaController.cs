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
        public ActionResult TiemposRequerimiento(string nombre)
        {
            var cedula = from a in db.EmpleadoDesarrollador
                         where a.nombreED == nombre
                         select a;
            var item = (from a in db.Requerimiento
                        where a.cedulaResponsable_FK == cedula.FirstOrDefault().cedulaED
                        select new { nombre = cedula.FirstOrDefault().nombreED, req = a.nombre, suma = a.duracionEstimada - a.duracionReal });
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + " " + dato.req + " " + dato.suma);
            }

            return View(lista);
        }

        public ActionResult HorasProyecto()
        {
             string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            var item =
                           from b in db.Requerimiento
                           from a in db.Proyecto
                           where a.nombre == b.nombreProyecto_FK
                           group b by b.nombreProyecto_FK into c
                           select new { nombre = c.FirstOrDefault().nombreProyecto_FK, sumaEst = c.Sum(a => a.duracionEstimada), sumaReal = c.Sum(d => d.duracionReal) };

            if (usuario == "Lider") {
                item =
                          from b in db.Requerimiento
                          from a in db.Proyecto
                          where a.nombre == proy && b.nombreProyecto_FK == proy
                          group b by b.nombreProyecto_FK into c
                          select new { nombre = c.FirstOrDefault().nombreProyecto_FK, sumaEst = c.Sum(a => a.duracionEstimada), sumaReal = c.Sum(d => d.duracionReal) };
            }
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + " " + dato.sumaEst + " " + dato.sumaReal);
            }

            return View(lista);
        }

        [HttpPost]
        public ActionResult HorasProyecto(string nombreProyecto)
        {
            var item =
                      from b in db.Requerimiento
                      where b.nombreProyecto_FK == nombreProyecto
                      group b by b.nombreProyecto_FK into c
                      select new { nombre = c.FirstOrDefault().nombreProyecto_FK,sumaEst = c.Sum(a => a.duracionEstimada), sumaReal = c.Sum(d => d.duracionReal) };
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombre + " " + dato.sumaEst + " " + dato.sumaReal);
            }

            return View(lista);
        }

        public ActionResult Historial()
        {
            List<string> lista = new List<string>();
            return View(lista);
        }

        [HttpPost]
        public ActionResult Historial(string nombre)
        {
            var item =
                    from r in db.Requerimiento
                    from p in db.Proyecto
                    from e in db.Equipo
                    from em in db.EmpleadoDesarrollador
                    where  p.nombre == r.nombreProyecto_FK  && em.nombreED == nombre && r.cedulaResponsable_FK == em.cedulaED
                    && e.nombreProy_FK == p.nombre && e.cedulaEM_FK == em.cedulaED
                    group new { r, p ,e, em} by p.nombre into c
                    select new {nombreP = c.FirstOrDefault().p.nombre , nombre = c.FirstOrDefault().em.nombreED, rol = c.FirstOrDefault().e.rol?"Lider":"Desarrollador",sumaReal = c.Sum(a => a.r.duracionReal)};
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.nombreP + " " + dato.nombre+" " +dato.rol +" " + dato.sumaReal);
            }
            
            return View(lista);
        }

        public SelectList getDesarrolladores()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            if (usuario == "Lider")//Si el usuarion en sesion es lider
            {//Puede ver los empleados que pertenecen a su equipo
                var info = from b in db.Equipo
                           from c in db.Proyecto
                           where b.cedulaEM_FK == cedula
                           where c.nombre == b.nombreProy_FK
                           select b;


                var obj = from a in db.EmpleadoDesarrollador
                          from b in db.Equipo
                          where b.nombreProy_FK == info.FirstOrDefault().nombreProy_FK
                          where b.cedulaEM_FK == a.cedulaED
                          select a.nombreED;

                return new SelectList(obj);
            }
            else if (usuario == "Cliente")//Si el usuarion en sesion es cliente
            {//Solo puede ver los empleados que estan en su proyecto
                var obj = from a in db.EmpleadoDesarrollador
                          from b in db.Equipo
                          from c in db.Proyecto
                          where a.cedulaED == b.cedulaEM_FK
                          where c.nombre == b.nombreProy_FK
                          where c.cedulaCliente == cedula
                          select a.nombreED;

                return new SelectList(obj);
            }
            else if (usuario == "Desarrollador")//Si el usuarion en sesion es desarrollador
            {//solo puede ver su propia información y no la de otros desarrolladores
                var obj = from a in db.EmpleadoDesarrollador
                          where a.cedulaED == cedula
                          select a.nombreED;

                return new SelectList(obj);
            }
            else if (usuario == "Jefe")//Si el usuarion en sesion es jefe
            {//Tiene acceso a todo el sistema
                var obj = from a in db.EmpleadoDesarrollador
                          select a.nombreED;
                return new SelectList(obj);
            }
            SelectList lista = new SelectList("");
            return lista;
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
