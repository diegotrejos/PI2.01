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
            List<EmpleadoDesarrollador> empleadosA = new List<EmpleadoDesarrollador>();

            /*Variables que se utilizan en el inicio de sección para guardar datos necesarios*/
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;   //Guarda el rol del usuario
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;   //Guarda el proyecto en el que tiene participación la persona que entra
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string; //Guarda la cédula de la persona que entra

            //Listas que se usan para el despliegue de los proyectos
            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().gettProyectos(usuario, cedula);
            // List<Proyecto.Models.Equipo> proyectosConLider = getEmployees();

            //Guardan temporalmente los datos
            TempData["empleadosDisponibles"] = empleados;
            TempData["empleadosAsignados"] = empleadosA;
            TempData["proyectos"] = proyectos;
            TempData.Keep();
            return View(db.Equipo.ToList());
        }

        [HttpPost]
        public ActionResult Index(string filtro)//filtro es el nombre del dropdown que me da el nombre de proyecto
        {
            
                //Equipo que pertenece al filtro 
                var empleadosAsignados = from a in db.Equipo
                                         from b in db.EmpleadoDesarrollador
                                         where a.nombreProy_FK == filtro && b.cedulaED == a.cedulaEM_FK
                                         select a;
                return View(empleadosAsignados.ToList());
            


            //Intento de hacerle saber a la vista quien es el lider y los miembros del equipo
            //ViewBag.Lider = lider.FirstOrDefault().cedulaEM_FK;


            // TempData.Keep();
           
        }



        // GET: Equipo/Details/5
        //Método que devuelve los detalles 
        public ActionResult Details(string nombreEquipo)
        {
            return View();
        }

        // GET: Equipo/Create
        public ActionResult AsignarLider()
        {
            return View();
        }


        //metodo de tipo Post que al presionar el boton de submit envia el nombre de proyecto y empleado seleccionado para ser el lider del equipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarLider(string nombre,string cedula, Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                new EmpleadoDesarrolladorController().modificarEstado(cedula); //llama a un metodo que hace que el empleado no este disponible
                equipo.nombreProy_FK = nombre;
                equipo.cedulaEM_FK = cedula;
                equipo.rol = true; //Declaro que es lider
                db.Equipo.Add(equipo);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            else
                Response.Write("<script>alert('Este proyecto no existe. Intente con otro');</script>");

            return RedirectToAction("AsignarLider"); // cambiar esto para saber que algo fue mal 
        }

        public ActionResult AsignarMiembros()
        {
            return View();
        }

        // POST: Equipo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        // GET: Equipo/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipo equipo = db.Equipo.Find(id);
            if (equipo == null)
            {
                return HttpNotFound();
            }
            return View(equipo);
        }

        // POST: Equipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Equipo equipo = db.Equipo.Find(id);
            db.Equipo.Remove(equipo);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        //Codigo que se llama en el script de create que permiete realizar el evento de arrastre 
        //Aporte del grupo #1
         [HttpPost]
        public ActionResult Asignar(string Miembros, string Proyecto)
        {
            if (Proyecto != "" && Miembros != "")
            {
                //separa el string Miembros en un array de string donde cada casilla es una cedula de desarrollador
                string[] eachMember = Miembros.Split(',');
                foreach (var itemId in eachMember)
                {
                    //es el equipo con la tupla que se va a agregar a la base
                    db.Equipo.Add(new Equipo
                    {
                        cedulaEM_FK = itemId,
                        nombreProy_FK = Proyecto,
                        rol = false
                    });
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

        [HttpPost]
        public ActionResult Eliminar(string Proyecto)
        {
            if (Proyecto != "")
            {
                //separa el string Miembros en un array de string donde cada casilla es una cedula de desarrollador
                var eachTeam = from a in db.Equipo
                               where a.nombreProy_FK == Proyecto
                               select a;
                
               
                foreach (var item in eachTeam.ToList())
                {
                    db.Equipo.Remove(item);
                    try
                    {
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
            else
            {
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




    }
}
