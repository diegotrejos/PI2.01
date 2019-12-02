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
        public static List<EmpleadoDesarrollador> empleadosConocedores = new List<EmpleadoDesarrollador>();

        // Metodo que devuelve la vista inicial del controlador, no recibe nada como parametro
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
        
        
        //Es el primer metodo post que se encarga del filtro de proyecto para mostrar los desarrolladores asignados
        [HttpPost]
        public ActionResult Index(string Proyecto)//filtro es el nombre del dropdown que me da el nombre de proyecto
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;   //Guarda el rol del usuario
            ViewBag.user = usuario;

            //codigo para buscar los empleados que se deberian de mostrar dependiendo del proyecto
            var empleadosAsignados = from a in db.Equipo
                                     where a.nombreProy_FK == Proyecto 
                                     select a;
            //Este LINQ se utiliza para encontrar el lider y mostrarselo al usuario 
            var lider = from a in db.Equipo
                        where a.nombreProy_FK == Proyecto
                        && a.rol == true
                        select a.EmpleadoDesarrollador.nombreED;

            //asignacion para mostrarlos en la  vista
            TempData["empleadosDisponibles"] = empleadosConocedores;
            TempData["empleadosAsignados"] =  empleadosAsignados.ToList();
            TempData["lider"] = lider.FirstOrDefault();
            return View(/*empleadosAsignados.ToList()*/);        
        }

        //Este es el segundo metodo post de equipo que se encarga de filtrar por conocimiento a los empleados disponibles
        [HttpPost]
        public ActionResult FiltrarConocimiento(string filtro)//filtro es el nombre del dropdown que me da el nombre de proyecto
        {
            //llamo al controlador de proyecto para que me duvuelva todos los empleados de nuevo
            List<EmpleadoDesarrollador> empleadosTotal = new EmpleadoDesarrolladorController().getEmpleados();
            
            int index = 0;
            foreach (var item in empleadosTotal)
            {
                // por medio de este foreach busco cada conocimiento de ese empleado en especifico
                foreach (var hab in item.Habilidades)
                {
                    //Con este if manejo que solo se le muestre al usuario uno que este disponibles, que sea desarrollador y que tenga ese conocimiento
                    if (hab.conocimientos == filtro && item.disponibilidad == true && item.flg == true)
                    {
                        empleadosConocedores.Add(new EmpleadoDesarrollador());
                        empleadosConocedores[index++] = item;
                    }
                }
            }
            TempData["empleadosDisponibles"] = empleadosConocedores; // envio la nueva informacion a la vista
            return View();
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

        //Este metodo sirve para llenar una lista que paso por parametro desde el controlador de reporteria 
         public void llenarArray(List<Proyecto.Models.ViewModels.infoEmpleados> info, string rol,string cedula)
        {
            //LINQ para obtener todos los empleados que esten en un equipo y no sean lider
            var linq = from a in db.Equipo
                       where a.rol == false
                       select a;

            //Si el que esta realizando la consulta en reporteria es el Jefe puede ver todos los empleados asignados
            if (rol == "Jefe")
            {
                int count = 0;
                foreach (var item in linq.ToList())
                {
                    info.Add(new Proyecto.Models.ViewModels.infoEmpleados()); // necesito agregar primero un elemento vacio
                    info[count].requerimientos = new List<Requerimiento>(); //le agrego una nueva lista de requerimientos vacia a la lista de requerimiento 
                                                                            //(realizo esto aqui porque es el unico controlador donde itero por todo=a la lista porque la instancia de equipo es el corazon de la clase que manejo en la lista)
                    info[count++].equipo = item; //aqui agrego el equipo
                }
            }
            else
            {
                //LINQ que me da los equipos donde el lider respectivo lo fue
                var dondeFueLider = from a in db.Equipo
                           where a.cedulaEM_FK == cedula
                           select a;

                //este codigo es muy similar al de arriba solo que tiene un if que filtra
                int count = 0;
                foreach (var item in linq.ToList())
                {
                    foreach (var ProyDondeFueLider in dondeFueLider)
                    {
                        if (ProyDondeFueLider.nombreProy_FK == item.nombreProy_FK)
                        {
                            info.Add(new Proyecto.Models.ViewModels.infoEmpleados());
                            info[count].requerimientos = new List<Requerimiento>();
                            info[count++].equipo = item;
                        }
                    }
                }
            }
        }

        //metodo que me duvuelve las habilidades para filtrar
        public SelectList getHabilidades()
        {
            return new HabilidadesController().getHabilidades();
        }


    }
}
