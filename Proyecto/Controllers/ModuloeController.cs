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
        /* public ActionResult Details(string id)
         {
             if (id == null)
             {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
             }
             Modulo modulo = db.Modulo.Find(id);
             if (modulo == null)
             {
                 return HttpNotFound();
             }
             return View(modulo);
         } */

        // GET: Moduloe/Create
        public ActionResult Create()
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo");
            }
            return View();
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
        public ActionResult Edit(int Id) // comunica con el modelo
        {
            Modulo model = new Modulo();
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var mTabla = db.Modulo.Find(Id);
                model.NombreProy = mTabla.NombreProy;
                model.Id = mTabla.Id;
                model.Nombre = mTabla.Nombre;
            }
            return View(model);
        }


        //Creo que este metodo es necesario para que sirva el edit

        [HttpPost]
        public ActionResult Edit(Modulo model)
        {
               try
               {
                   if (ModelState.IsValid)
                   {
                       using (Gr02Proy3Entities db = new Gr02Proy3Entities())
                       {
                           var mTabla = db.Modulo.Find(model.Id); //relaciona con la BD
                           model.NombreProy = mTabla.NombreProy;
                           model.Id = mTabla.Id;
                           model.Nombre = mTabla.Nombre;

                           db.Entry(mTabla).State = System.Data.Entity.EntityState.Modified;
                           db.SaveChanges();
                       }
                       return Redirect("/"); //Path al que se direcciona
                   }
                   return View(model);
               }
               catch (Exception ex)
               {
                throw new Exception(ex.Message); //Para notificar la exception
               }
           } 

        // POST: Moduloe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NombreProy,Id,Nombre")] Modulo modulo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modulo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo", modulo.NombreProy);
            return View(modulo);
        }*/

        // GET: Moduloe/Delete/5
        [HttpGet]
         public ActionResult Delete(int Id)
         {
                Modulo model = new Modulo();
                using (Gr02Proy3Entities db = new Gr02Proy3Entities())
                {
                    var mTabla = db.Modulo.Find(Id);
                    db.Modulo.Remove(mTabla);
                    db.SaveChanges();
                }
                return Redirect("~/Moduloe/");
        }
         



        /*        // POST: Moduloe/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public ActionResult DeleteConfirmed(string id)
                {
                    Modulo modulo = db.Modulo.Find(id);
                    db.Modulo.Remove(modulo);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
        */

        /*
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
    */
        //}
    }
}
