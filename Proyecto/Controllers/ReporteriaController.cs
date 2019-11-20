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
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().gettProyectos(usuario, cedula);
           
            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED");
            ViewBag.user = usuario;
            TempData["Complejidad"] = crearListaComplejidad();
           
            TempData.Keep();
            return View();
        }


        public ActionResult ComparacionComplejidad()
        {
            return View();
        }

        public ActionResult HistorialDesarrollador()
        {
            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED");
            return View();
        }

        //Crea la lista predefinida con las complejidades
        private List<string> crearListaComplejidad()
        {
            List<string> listaLocal = new List<string>();
            listaLocal.Add("Simple");
            listaLocal.Add("Mediano");
            listaLocal.Add("Complejo");
            listaLocal.Add("Muy Complejo");
            return listaLocal;
        }
    }
}
