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
    public class HabilidadesController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: Habilidades
        public ActionResult Index(string id)//id para conectar habilidad con empleado
        {
            Habilidades modelo = new Habilidades();
            List<Habilidades> aList;//lista de habilidades
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            aList = new List<Habilidades>();
            modelo.listaHabilidades = db.Habilidades.ToList();
            for (int j = 0; j < modelo.listaHabilidades.Count; j++)
            {
                if (id.Equals(modelo.listaHabilidades.ElementAt(j).cedulaEmpleadoPK_FK))
                {
                    aList.Add(modelo.listaHabilidades.ElementAt(j));
                }
            }
            return View(aList.ToList());
        }

        // GET: Habilidades/Details/5
        public ActionResult Details(string id)//id para conectar habilidad con empleado
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habilidades habilidades = db.Habilidades.Find(id);
            if (habilidades == null)
            {
                return HttpNotFound();
            }

            return View(habilidades);
        }

        // GET: Habilidades/Create
        public ActionResult Create()
        {
            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED");
            return View();
        }

        // POST: Habilidades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cedulaEmpleadoPK_FK,conocimientos")] Habilidades habilidades)
        {
            if (ModelState.IsValid)
            {   //Valida que no tenga dos conocimientos iguales para un mismo empleado
                if (!db.Habilidades.Any(model => model.conocimientos == habilidades.conocimientos))
                {
                    db.Habilidades.Add(habilidades);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = habilidades.cedulaEmpleadoPK_FK });
                }
                else
                {//Si ya esatab esta habilidad, muestra mensaje de error
                    Response.Write("<script>alert('Esta habilidada ya fue agregada.');</script>");//Si la cédula ya existe, muestra mensaje de error)
                }
            }

            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", habilidades.cedulaEmpleadoPK_FK);
            return View(habilidades);
        }

        // GET: Habilidades/Edit/5
        public ActionResult Edit(string id, string habilidad)//id para conectar habilidad con empleado y la habilidad
        {
            if (id == null || habilidad == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habilidades habilidades = db.Habilidades.Find(id, habilidad);
            if (habilidades == null)
            {
                return HttpNotFound();
            }
            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", habilidades.cedulaEmpleadoPK_FK);
            return View(habilidades);
        }

        // POST: Habilidades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cedulaEmpleadoPK_FK,conocimientos")] Habilidades habilidades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(habilidades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = habilidades.cedulaEmpleadoPK_FK });
            }
            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", habilidades.cedulaEmpleadoPK_FK);
            return View(habilidades);
        }

        // GET: Habilidades/Delete/5
        public ActionResult Delete(string id, string habilidad)//id para conectar habilidad con empleado y la habilidad
        {
            if (id == null || habilidad == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Habilidades habilidades = db.Habilidades.Find(id, habilidad);
            if (habilidades == null)
            {
                return HttpNotFound();
            }
            return View(habilidades);
        }

        // POST: Habilidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string habilidad)//id para conectar habilidad con empleado y la habilidad
        {
            Habilidades habilidades = db.Habilidades.Find(id, habilidad);
            db.Habilidades.Remove(habilidades);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = habilidades.cedulaEmpleadoPK_FK });
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
