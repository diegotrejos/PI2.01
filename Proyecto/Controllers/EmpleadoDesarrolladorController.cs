using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;



namespace Proyecto.Controllers
{
    public class EmpleadoDesarrolladorController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: EmpleadoDesarrollador
        public ActionResult Index()
        {
            return View(db.EmpleadoDesarrollador.ToList());
        }

        // GET: EmpleadoDesarrollador/Details/5
        public ActionResult Details(string id)//String id para conectar empleado con la tabla habilidades
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // GET: EmpleadoDesarrollador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpleadoDesarrollador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cedulaED,nombreED,apellido1ED,apellido2ED,fechaInicio,fechaNacimiento,edad,telefono,correo,disponibilidad,direccionExacta,distrito,canton,provincia,flg")] EmpleadoDesarrollador empleadoDesarrollador)
        {

           //Para válidar 
            if (ModelState.IsValid)
            {   //Valida si la cédula es nueva
               if (!db.EmpleadoDesarrollador.Any(model => model.cedulaED == empleadoDesarrollador.cedulaED))
                {
                    
                            db.EmpleadoDesarrollador.Add(empleadoDesarrollador);
                            db.SaveChanges();
                            return RedirectToAction("Index");

                }
                    else//Si la cédula ya existe, muestra mensaje de error
                        Response.Write("<script>alert('La cédula de este cliente ya existe. Intente con una nueva');</script>");

            }
           

            return View(empleadoDesarrollador);
        }

        // GET: EmpleadoDesarrollador/Edit/5
        public ActionResult Edit(string id)//String id para conectar empleado con la tabla habilidades
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // POST: EmpleadoDesarrollador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cedulaED,nombreED,apellido1ED,apellido2ED,fechaInicio,fechaNacimiento,edad,telefono,correo,disponibilidad,direccionExacta,distrito,canton,provincia,flg")] EmpleadoDesarrollador empleadoDesarrollador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleadoDesarrollador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empleadoDesarrollador);
        }

        // GET: EmpleadoDesarrollador/Delete/5
        public ActionResult Delete(string id)//String id para conectar empleado con la tabla habilidades
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            if (empleadoDesarrollador == null)
            {
                return HttpNotFound();
            }
            return View(empleadoDesarrollador);
        }

        // POST: EmpleadoDesarrollador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            EmpleadoDesarrollador empleadoDesarrollador = db.EmpleadoDesarrollador.Find(id);
            db.EmpleadoDesarrollador.Remove(empleadoDesarrollador);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
