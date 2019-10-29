using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class RequerimientoController : Controller
    {

      
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        Proyecto.Controllers.ModuloeController moduloController = new Proyecto.Controllers.ModuloeController();

        // GET: Requerimiento
        public async Task<ActionResult> Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                var requerimiento = db.Requerimiento.Include(r => r.EmpleadoDesarrollador).Include(r => r.Modulo);
                return View(await requerimiento.ToListAsync());
            }
        }
        //luego de dropdowns index view se ajusta con esto
        [HttpPost]
        public ActionResult Index(string nombreProyecto,string nombreModulo)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
                ViewBag.user = usuario;
                string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
                string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
                ViewBag.proyectos = getProyectos(usuario,cedula);
                
                var queryMod = from a in db.Modulo
                               where a.NombreProy.Equals(nombreProyecto)&&(a.Nombre.Equals(nombreModulo))
                               select a.Id;

                if (queryMod != null)
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK == nombreProyecto && a.idModulo_FK== queryMod.FirstOrDefault()
                                   select a;
                    return View(queryReq.ToList());
                }
                else
                {
                    var queryReq = from a in db.Requerimiento
                                   where a.nombreProyecto_FK==nombreProyecto
                                   select a;


                    return View(queryReq.ToList());
                }

              
            }
        }



        // GET: Requerimiento/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(id);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }
                return View(requerimiento);
            }
        }

        // GET: Requerimiento/Create
        public ActionResult Create()
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED");
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre");
                return View();
            }
        }

        // POST: Requerimiento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Proyecto, string Modulo,[Bind(Include = "nombreProyecto_FK,idModulo_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    var queryMod = from a in db.Modulo
                                   where a.NombreProy.Equals(Proyecto) && (a.Nombre.Equals(Modulo))
                                   select a.Id;
                    string input = requerimiento.nombre;
                    string output = input.Replace("requerimiento", "");
                    requerimiento.nombre = output;
                    requerimiento.nombreProyecto_FK = Proyecto;
                    requerimiento.idModulo_FK = queryMod.FirstOrDefault();
                    requerimiento.cedulaResponsable_FK = "302250355";
                    db.Requerimiento.Add(requerimiento);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", requerimiento.cedulaResponsable_FK);
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre", requerimiento.nombreProyecto_FK);
                return View(requerimiento);
            }
        }

        // GET: Requerimiento/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
           
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(id);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }
                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", requerimiento.cedulaResponsable_FK);
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre", requerimiento.nombreProyecto_FK);
                return View(requerimiento);
            }
        }

        // POST: Requerimiento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "nombreProyecto_FK,idModulo_FK,nombre,complejidad,duracionEstimada,duracionReal,cedulaResponsable_FK,estado")] Requerimiento requerimiento)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(requerimiento).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ViewBag.cedulaResponsable_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", requerimiento.cedulaResponsable_FK);
                ViewBag.nombreProyecto_FK = new SelectList(db.Modulo, "NombreProy", "Nombre", requerimiento.nombreProyecto_FK);
                return View(requerimiento);
            }
        }

        // GET: Requerimiento/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(id);
                if (requerimiento == null)
                {
                    return HttpNotFound();
                }
                return View(requerimiento);
            }
        }

        // POST: Requerimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                Requerimiento requerimiento = await db.Requerimiento.FindAsync(id);
                db.Requerimiento.Remove(requerimiento);
                await db.SaveChangesAsync();
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





        public SelectList getProyectos(string rol, string cedula)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {
                return this.proyController.getProyectos(rol, cedula);
            }
        }


        public  SelectList getModulos(string nombreproyecto)
        {
            using (Gr02Proy3Entities db = new Gr02Proy3Entities())
            {

                return this.moduloController.getModulos(nombreproyecto);
            }
        }


    }
}
