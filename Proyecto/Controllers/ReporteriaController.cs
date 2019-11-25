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


        public ActionResult ComparacionComplejidad()
        {
            List<string> datos = new List<string>();
           
            return View(datos);
        }

        [HttpPost]
        public ActionResult ComparacionComplejidad(string complejidad)
        {
            var item = from req in db.Requerimiento
                       from proy in db.Proyecto
                       where proy.nombre == req.nombreProyecto_FK
                       where req.complejidad == complejidad
                       group req by req.complejidad into g
                       select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = g.Average(c => c.duracionReal) };

            if (complejidad == "" || complejidad == "Todos los niveles") {
                     item = from req in db.Requerimiento
                           group req by 1 into g
                           select new { total = g.Count(), minimo = g.Min(a => a.duracionEstimada - a.duracionReal), maximo = g.Max(b => b.duracionEstimada - b.duracionReal), promedio = g.Average(c => c.duracionReal) };
            }

            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.total + " " + dato.minimo + " " + dato.maximo + " " + dato.promedio);
            }
            return View(datos);
        }


        public ActionResult HistorialDesarrollador(string nombre)
        {
            var item = from proy in db.Proyecto
                       from req in db.Requerimiento
                       from eq in db.Equipo
                       from emp in db.EmpleadoDesarrollador
                       where emp.nombreED == nombre
                       where emp.cedulaED == req.cedulaResponsable_FK
                       where req.nombreProyecto_FK == proy.nombre
                       where proy.nombre == eq.nombreProy_FK
                       where eq.cedulaEM_FK == emp.cedulaED
                       where proy.fechaFinalizacion != null
                       group new { proy, req, eq } by new { proy.nombre, eq.rol } into g
                       select new { nom = g.Key.nombre, r = g.Key.rol, duracion = g.Sum(x => x.req.duracionReal) };

            List<string> datos = new List<string>();
            foreach (var dato in item)
            {
                datos.Add(dato.nom + " " + dato.r + " " + dato.duracion);
            }
            return View(datos);
        }

        //Crea la lista predefinida con las complejidades
        public List<SelectListItem> getListaComplejidad()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "Simple"});
            items.Add(new SelectListItem() { Text = "Mediano" });
            items.Add(new SelectListItem() { Text = "Complejo" });
            items.Add(new SelectListItem() { Text = "Muy Complejo" });
             return items;

        }

        public List<SelectListItem> getDesarrolladores()
        {
            return emplController.getDesarrolladores();
        }
    }
}
