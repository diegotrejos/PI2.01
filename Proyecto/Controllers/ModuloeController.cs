
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
    public class ModuloeController : Controller
    {
        //private Gr02Proy3Entities db = new Gr02Proy3Entities(); 

        // GET: Moduloe
        public ActionResult Index()
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var modulo = db.Modulo.Include(m => m.Proyecto);
                return View(modulo.ToList());
            }
        }

        // GET: Moduloe/Details/5
        public ActionResult Details(int id, string nombreProy)
        {

            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                /*Aqui podria poner un filtro*/
                var query = from a in db.Modulo
                            where ((a.Id.Equals(id)).Equals(a.NombreProy.Equals(nombreProy)))
                            select a;

                var item = query.FirstOrDefault();

                if (item != null)
                    return View(item);
                else
                    return View("NotFound");

            }

        }
        
        // GET: Moduloe/Create
        public ActionResult Create()
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo");
                return View();
            }
           
        }
        
        // POST: Moduloe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken] //Buscar para que sirve 
        public ActionResult Create([Bind(Include = "NombreProy,Id,Nombre")] Modulo modulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    db.Modulo.Add(modulo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo", modulo.NombreProy);
            }
            return View(modulo);
        }

        // GET: Moduloe/Edit/5
        public ActionResult Edit(int id, string nombreProy) // comunica con el modelo
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                /*Aqui podria poner un filtro*/
                var query = from a in db.Modulo
                            where ((a.Id.Equals(id)).Equals(a.NombreProy.Equals(nombreProy)))
                            select a;

                var item = query.FirstOrDefault();

                if (item != null)
                    return View(item);
                else
                    return View("NotFound");

            }

        }



        // POST: Moduloe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NombreProy,Id,Nombre")] Modulo modulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(modulo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
               
                return View(modulo);
            }
        }


        public ActionResult Delete(int id, string nombreProy) // comunica con el modelo
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                /*Aqui podria poner un filtro*/
                var query = from a in db.Modulo
                            where ((a.Id.Equals(id)).Equals(a.NombreProy.Equals(nombreProy)))
                            select a;

                var item = query.FirstOrDefault();

                if (item != null)
                    return View(item);
                else
                    return View("NotFound");

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "NombreProy,Id,Nombre")] Modulo modulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                db.Entry(modulo).State = EntityState.Deleted;
                db.Modulo.Remove(modulo);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }
        }


        protected override void Dispose(bool disposing)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }


    }
}