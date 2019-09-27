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
    public class ProyectoController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: Proyecto
        public ActionResult Index()
        {
            var proyecto = db.Proyecto.Include(p => p.Cliente);
            return View(proyecto.ToList());
        }

        // GET: Proyecto/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // GET: Proyecto/Create
        public ActionResult Create()
        {
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre");
            return View();
        }

        // POST: Proyecto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombre,duracionEstimada,costoTrabajo,costoEstimado,objetivo,fechaFinalizacion,fechaInicio,cedulaCliente")] Proyecto.Models.Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                db.Proyecto.Add(proyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
            return View(proyecto);
        }

        // GET: Proyecto/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
            return View(proyecto);
        }

        // POST: Proyecto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nombre,duracionEstimada,costoTrabajo,costoEstimado,objetivo,fechaFinalizacion,fechaInicio,cedulaCliente")] Proyecto.Models.Proyecto proyecto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proyecto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
            return View(proyecto);
        }

        // GET: Proyecto/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            if (proyecto == null)
            {
                return HttpNotFound();
            }
            return View(proyecto);
        }

        // POST: Proyecto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Proyecto.Models.Proyecto proyecto = db.Proyecto.Find(id);
            db.Proyecto.Remove(proyecto);
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
