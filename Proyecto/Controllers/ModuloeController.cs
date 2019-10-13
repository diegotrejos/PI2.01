
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
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        // GET: Moduloe
        public ActionResult Index(string sortOrder)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                


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
        public ActionResult Create([Bind(Include = "NombreProy,Nombre")] Modulo modulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)//Valida si Id y Proyecto esten correctos
                {
                    if (db.Proyecto.Any(model => model.nombre == modulo.NombreProy))//Reviso que el proyecto seleccionado sea valido
                    {
                        if (db.Modulo.Any(model => (model.NombreProy == modulo.NombreProy) == true))//reviso si es el primer modulo de este proyecto
                        {
                            if (db.Modulo.Any(model => (model.NombreProy == modulo.NombreProy) && !(model.Nombre == modulo.Nombre)))//reviso que no tenga modulos con nombres iguales
                            {
                                db.Modulo.Add(modulo);
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                Response.Write("<script>alert('El nombre de este modulo ya existe en este proyecto. Intente con uno nuevo');</script>");
                            }
                        }
                        else
                        {
                             db.Modulo.Add(modulo);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        
                    }
                    else//Si la cédula ya existe, muestra mensaje de error
                        Response.Write("<script>alert('Este proyecto no existe. Intente con otro');</script>");
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

    







        // GET: Moduloe/Delete/5
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
        public ActionResult Delete([Bind(Include = "NombreProy,Id")] Modulo modulo)
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

        public SelectList getProyectos()
        {

            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {

                return this.proyController.getProyectos();


            }
        }


    }
}