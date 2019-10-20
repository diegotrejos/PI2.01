using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;
using System.Data.SqlClient;
using System.Windows;

namespace Proyecto.Controllers
{
    public class ProyectoController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: Proyecto

        /* Metodo que devuelve la lista de proyectos para la vista index
         * @return lista de proyectos
         */
        public ActionResult Index()
        {
            var proyecto = db.Proyecto.Include(p => p.Cliente);
            return View(proyecto.ToList());
        }

        // GET: Proyecto/Details/5
        /*Metodo que devuelve los detalles de un proyecto en específico
         * @param id : la llave del proyecto específico
         * @return proyecto
         */
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
        /*Metodo para la vista de la cedula de cliente
         * @return vista de cliente
         */
        public ActionResult Create()
        {
            ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre");
            return View();
        }

        // POST: Proyecto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*Método para crear un proyecto
  * @param datos necesarios para la creacion de un proyecto
  * @return vista del nuevo proyecto
  */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombre,duracionEstimada,costoTrabajo,costoEstimado,objetivo,fechaFinalizacion,fechaInicio,cedulaCliente")] Proyecto.Models.Proyecto proyecto)
        {
                  


                if (ModelState.IsValid)
                {
                    if (!db.Proyecto.Any(model => model.nombre == proyecto.nombre))
                    {

                        SqlConnection con = new SqlConnection("data source=172.16.202.23;user id=Gr02Proy3;password=Orion24!!!;MultipleActiveResultSets=True;App=EntityFramework");
                        SqlCommand cmd = new SqlCommand("SELECT * FROM Proyecto WHERE nombre=@nombre AND objetivo=@objetivo", con);
                        /* Convertimos en literal estos parámetros, por lo que no podrán hacer la inyección */
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 15).Value = proyecto.nombre;
                        cmd.Parameters.Add("@objetivo", SqlDbType.VarChar, 256).Value = proyecto.objetivo;

                        db.Proyecto.Add(proyecto);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                        Response.Write("<script>alert('El nombre del proyecto ya existe. Intente con uno nuevo');</script>");
                }


                ViewBag.cedulaCliente = new SelectList(db.Cliente, "cedula", "nombre", proyecto.cedulaCliente);
                return View(proyecto);
                //registro no existe
        }


        /*Método para la vista  editar un proyecto
         * @param id:llave del proyecto
         * @return vista del proyecto específico
         */
        // GET: Proyecto/Edit/5C:\Users\Katherine\Desktop\Proyecto\Proyecto\Controllers\ProyectoController.cs
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


        /*Metodo para editar un proyecto
         * @param datos necesarios para editar un proyecto
         * @return vista al inicio con los cambios del proyecto
         */
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
        /*Vista para eliminar un proyecto
         * @param id: llave del proyecto específico
         * @return vista del proyecto específico
         */
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
        /*Método para borrar un proyecto
         *@param id: llave específica del proyecto
         * @return vista al inicio de proyecto borrado
         */
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

        /*Método para obtener una SelectLista de proyectos
         * @return lista de proyectos
         */
        public SelectList getProyectos()
        {
            var query = from proy in db.Proyecto
                        select proy.nombre;
            return new SelectList(query);

        }

         /*Método para obtener una lista de proyectos
         * @return lista de proyectos
         */
        public List<Proyecto.Models.Proyecto> gettProyectos()
        {
            var query = from proy in db.Proyecto
                        select proy;
            return new List<Proyecto.Models.Proyecto>(query);

        }
    }
}
