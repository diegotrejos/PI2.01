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
        public class ReporteController : Controller
    {
        /*Variables que se utilizan en el inicio de sección para guardar datos necesarios*/
        public string usuario = "";     //Guarda el rol del usuario
        public string cedula = "";      //Guarda la cédula de la persona que entra
        public string proy = "";        //Guarda el proyecto en el que tiene participación la persona que entra

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.EmpleadoDesarrolladorController emplController = new Proyecto.Controllers.EmpleadoDesarrolladorController();

        public ActionResult Index()
        {
            //Uso de variables temporales que guardan los datos necesarias para el inicio de sección
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;//coment
            return View();
        }

        public ActionResult TiemposRequerimiento() {

            /*var item = (from a in db.Requerimiento
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
                         select new { nombre = cedula.FirstOrDefault().nombreED, req = a.nombre ,suma = a.duracionEstimada - a.duracionReal});
           List<string> lista = new List<string>();
            foreach (var dato in item) {
                lista.Add(dato .nombre+" "+dato.req+" "+dato.suma);
            }
            
            return View(lista);
        }

        public ActionResult HorasProyecto() {
            /*var item = 
                       from b in  db.Requerimiento
                       from a in db.Proyecto
                       where a.nombre == b.nombreProyecto_FK
                       group b by b.nombreProyecto_FK into c
                       select new { sumaEst = c.Sum(a => a.duracionEstimada), sumaReal = c.Sum(d=> d.duracionReal)};*/
            List<string> lista = new List<string>();
            /*foreach (var dato in item)
            {
                lista.Add( dato.sumaEst + " " + dato.sumaReal);
            }*/

            return View(lista);
        }

        [HttpPost]
        public ActionResult HorasProyecto(string nombreProyecto)
        {
            var item =
                      from b in db.Requerimiento
                      where b.nombreProyecto_FK == nombreProyecto
                      group b by b.nombreProyecto_FK into c
                      select new { sumaEst = c.Sum(a => a.duracionEstimada), sumaReal = c.Sum(d => d.duracionReal) };
            List<string> lista = new List<string>();
            foreach (var dato in item)
            {
                lista.Add(dato.sumaEst + " " + dato.sumaReal);
            }

            return View(lista);
        }

        public ActionResult Historal()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Hstorial(string nombre)
        {
            return View();
        }

        public List<SelectListItem> getEmpleadosTrabajando()
        {
            return emplController.getEmpleadosTrabajando();
        }

        public SelectList getProyectos()
        {
            return proyController.getProyectos();
        }

    }
}