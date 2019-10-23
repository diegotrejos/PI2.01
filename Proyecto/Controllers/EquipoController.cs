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
        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        string rol = new AutenticarController().getUsuario();
        // GET: Equipo
        public ActionResult Index()
        {
            //Listas que se utilizan para el manejo de los empleados
            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            List<EmpleadoDesarrollador> empleadosA = new List<EmpleadoDesarrollador>();
            ViewBag.user = rol;

            //Listas que se usan para el despliegue de los proyectos
            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().gettProyectos();
           // List<Proyecto.Models.Equipo> proyectosConLider = getEmployees();

            //Guardan temporalmente los datos
            TempData["empleadosDisponibles"] = empleados;
            TempData["empleadosAsignados"] = empleadosA;
            TempData["proyectos"] = proyectos;
            TempData.Keep();
            return View(db.Equipo.ToList());
        }



        // GET: Equipo/Details/5
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
        public SelectList getProyectos()
        {
            return new Proyecto.Controllers.ProyectoController().getProyectos();
        }

        //Codigo que se llama en el script de create que permiete realizar el evento de arrastre 
        //Aporte del grupo #1
        public ActionResult UpdateItem(string itemIds)
        {
            Gr02Proy3Entities db = new Gr02Proy3Entities();
            int count = 1;
            List<int> itemIdList = new List<int>(); 
            itemIdList = itemIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            foreach (var itemId in itemIds)
            {
                try
                {
                    EmpleadoDesarrollador item = db.EmpleadoDesarrollador.Where(x => x.cedulaED == itemId.ToString()).FirstOrDefault();
                    //item.disponibilidad = false;  //intento de que se pase a la otra columna 
                    //var equipo()
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    continue;
                }
                count++;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
