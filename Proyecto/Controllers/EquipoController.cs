using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class EquipoController : Controller
    {
        /*Variables que se utilizan en el inicio de sección para guardar datos necesarios*/
        public string usuario = "";     //Guarda el rol del usuario
        public string cedula = "";      //Guarda la cédula de la persona que entra
        public string proy = "";        //Guarda el proyecto en el que tiene participación la persona que entra
        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();


        // GET: Equipo
        public ActionResult Index()
        {
            //Listas que se utilizan para el manejo de los empleados
            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            List<Equipo> empleadosA = new List<Equipo>();

            /*Variables que se utilizan en el inicio de sección para guardar datos necesarios*/
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;   //Guarda el rol del usuario
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;   //Guarda el proyecto en el que tiene participación la persona que entra
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string; //Guarda la cédula de la persona que entra

            //Listas que se usan para el despliegue de los proyectos
            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().gettProyectos(usuario, cedula);
            // List<Proyecto.Models.Equipo> proyectosConLider = getEmployees();

            //Guardan temporalmente los datos
            TempData["Lider"] = "";
            TempData["empleadosDisponibles"] = empleados;
            TempData["empleadosAsignados"] = empleadosA;
            TempData["proyectos"] = proyectos;
            TempData.Keep();
            return View(db.Equipo.ToList());
        }

        [HttpPost]
        public ActionResult Index(string Proyecto)//filtro es el nombre del dropdown que me da el nombre de proyecto
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;   //Guarda el rol del usuario
            ViewBag.user = usuario;

            //codigo para buscar los empleados que se deberian de mostrar dependiendo del proyecto
            var empleadosAsignados = from a in db.Equipo
                                     where a.nombreProy_FK == Proyecto 
                                     select a;

            var lider = from a in db.Equipo
                        where a.nombreProy_FK == Proyecto
                        && a.rol == true
                        select a.EmpleadoDesarrollador.nombreED;

            //asignacion para mostrarlos en la  vista
            TempData["empleadosAsignados"] =  empleadosAsignados.ToList();
            TempData["lider"] = lider.FirstOrDefault();
            return View(/*empleadosAsignados.ToList()*/);
           
        }


      
        //Codigo que traia por default visual y como se referencia 5 veces mejor no lo borro xD
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Codigo que se comunica con la controladora de proyectos para recibir los proyectos actuales creados
        public SelectList getProyectos(String rol, String cedula)
        {
            
            return this.proyController.getProyectos(rol, cedula);
        }

        //Codigo llamado al hacer click en el boton aceptar que permite a los miembros arrastrados agregarlos a la tabla equipo
        [HttpPost]
        public ActionResult Asignar(string Miembros, string Proyecto)
        {
            //condicion para ver si al menos se agrego un empleado y se coloco un proyecto en el dropdown 
            if (Proyecto != "" && Miembros != "")
            {
                //separa el string Miembros en un array de string donde cada casilla es una cedula de desarrollador
                string[] eachMember = Miembros.Split(',');
                foreach (var itemId in eachMember)
                {
                   
                    var empleado = from a in db.EmpleadoDesarrollador
                                   where a.cedulaED == itemId
                                   select a.disponibilidad;
                    //como en la lista se muestran empleados que ya estan asignados en el momento esto toma en cuenta solo los nuevos
                    if (empleado.FirstOrDefault() == true)
                    {
                        //es el equipo con la tupla que se va a agregar a la base
                        db.Equipo.Add(new Equipo
                        {
                            cedulaEM_FK = itemId,
                            nombreProy_FK = Proyecto,
                            rol = false
                        });
                    }
                    try
                    {
                        new EmpleadoDesarrolladorController().modificarEstado(itemId); //para que ese empleado deje de estar disponible
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }
                }
                //retorna al script al success
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Equipo"),
                    isRedirect = true, //se redireccionara
                    error = false //no paso ningun error
                });
            }
            else {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Equipo"),
                    error = true, //paso un error
                    isRedirect = false //como es falso no se va a redirigir
                });
            }
        }

        public SelectList getEmpleadosProyecto(string nombreProy)
        {
            var query = from eq in db.Equipo
                        where eq.nombreProy_FK == nombreProy
                        select eq.EmpleadoDesarrollador.nombreED;
            
            return new SelectList(query);
        }


        public List<Equipo> getEquipos()
        {
            var team = from a in db.Equipo
                       orderby a.Proyecto.fechaFinalizacion ascending
                       select a;
            return team.ToList();
        }

    }
}
