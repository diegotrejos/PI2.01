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
        //Variables para saber quien está logeado y a que proyecto pertenece
        public string usuario = "";//EL usuario que está en el sistema
        public string cedula = "";//La édula de quien esta en el sistema
        public string proy = "";//Proyecto al que pertenece quien esta en el sistema
        private Gr02Proy3Entities db = new Gr02Proy3Entities();


        // GET: Habilidades
        /* Vista de habilidades
         * @return lista de habilidades
         */
        public ActionResult Index(string id)//id para conectar habilidad con empleado
        {
            //Estas variables guardan los datos del usuario en sesion
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            Habilidades modelo = new Habilidades();//Modelo
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
        /*Metodo para consultas de habilidades
         * @param id : llave de habilidades
         * @return vista de habilidades
         */
        public ActionResult Details(string id)
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
        /*Vista de create
        * @return vista
        */
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
        /*Crea empleado
          * @param atributos para crear habilidad
          * @return vista de habilidades
          */
        public ActionResult Create([Bind(Include = "cedulaEmpleadoPK_FK,conocimientos")] Habilidades habilidades)
        {
            if (ModelState.IsValid)//Para válidar 
            {  
                        db.Habilidades.Add(habilidades);
                        db.SaveChanges();
                        return RedirectToAction("Index", new { id = habilidades.cedulaEmpleadoPK_FK });

            }

            ViewBag.cedulaEmpleadoPK_FK = new SelectList(db.EmpleadoDesarrollador, "cedulaED", "nombreED", habilidades.cedulaEmpleadoPK_FK);
            return View(habilidades);
        }

        // GET: Habilidades/Edit/5
        /*Vista para editar 
         * @param id: llave
         * @return vista de editar
         */
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
        /*Editar habilidad
          * @param atributos para editar 
          * @return vista 
          */
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
        /*Vista para borrar habilidad
          * @param llavedo
          * @return vista
          */
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
        /*Para confirmar si borrar la habilidad
          * @param llave
          * @return vista de la lista de habilidades
          */
        public ActionResult DeleteConfirmed(string id, string habilidad)//id para conectar habilidad con empleado y la habilidad
        {
            Habilidades habilidade = db.Habilidades.Find(id, habilidad);
            db.Habilidades.Remove(habilidade);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = habilidade.cedulaEmpleadoPK_FK });
        }

        /*
          * 
          * @param 
          * @return 
          */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public SelectList getHabilidades()
        {
            var query = from Habilidades in db.Habilidades
                        select Habilidades.conocimientos;
            return new SelectList(query.Distinct());
        }

    }
}
