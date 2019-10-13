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
        // GET: Equipo
        public ActionResult Index()
        {
            var equipo = db.Equipo.Include(e => e.EmpleadoDesarrollador).Include(e => e.Proyecto);
            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            TempData["empleados"] = empleados;
            TempData.Keep();
            return View(equipo.ToList());
        }

        // GET: Equipo/Details/5
        public ActionResult Details(string id)
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

        // GET: Equipo/Create
        public ActionResult Create()
        {
            ViewBag.cedulaEM_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED");
            ViewBag.nombreProy_FK = new SelectList(db.Proyecto, "nombre", "objetivo");
            return View();
        }

        // POST: Equipo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cedulaEM_FK,nombreProy_FK,nombreEquipo,rol")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                db.Equipo.Add(equipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cedulaEM_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", equipo.cedulaEM_FK);
            ViewBag.nombreProy_FK = new SelectList(db.Proyecto, "nombre", "objetivo", equipo.nombreProy_FK);
            return View(equipo);
        }

        // GET: Equipo/Edit/5
        /*public ActionResult Edit(string id)
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
            ViewBag.cedulaEM_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", equipo.cedulaEM_FK);
            ViewBag.nombreProy_FK = new SelectList(db.Proyecto, "nombre", "objetivo", equipo.nombreProy_FK);
            return View(equipo);
        }*/

        // POST: Equipo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cedulaEM_FK,nombreProy_FK,nombreEquipo,rol")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cedulaEM_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", equipo.cedulaEM_FK);
            ViewBag.nombreProy_FK = new SelectList(db.Proyecto, "nombre", "objetivo", equipo.nombreProy_FK);
            return View(equipo);
        }*/

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public SelectList getProyectos()
        {
            return new Proyecto.Controllers.ProyectoController().getProyectos();
        }

        //Codigo igual a grupo 1, hay que cambiarlo
        public ActionResult UpdateItem(string itemIds)
        {
            Gr02Proy3Entities db = new Gr02Proy3Entities();
            int count = 0;
            List<int> itemIdList = new List<int>();
            itemIdList = itemIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            foreach (var itemId in itemIds)
            {
                try
                {
                    EmpleadoDesarrollador item = db.EmpleadoDesarrollador.Where(x => x.cedulaED == itemId.ToString()).FirstOrDefault();
                    item.disponibilidad = false;  //intento de que se pase a la otra columna 
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
