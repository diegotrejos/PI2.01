using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class ClienteController : Controller         //Controlador del Módelo Cliente
    {

        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        // GET: Cliente
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            ViewBag.user = usuario;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            if (usuario != "Jefe")
            {
                if (usuario == "Cliente")
                {
                    var obj = from a in db.Cliente
                              where a.cedula == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
                else
                {
                    var obj = from a in db.Cliente
                              from b in db.Proyecto
                              from c in db.Equipo
                              from d in db.EmpleadoDesarrollador
                              where a.cedula == b.cedulaCliente
                              where b.nombre == c.nombreProy_FK
                              where c.cedulaEM_FK == d.cedulaED
                              where d.cedulaED == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
            }
            else
            {
                return View(db.Cliente.ToList());
            }
               
        }

        // GET: Cliente/Details/5
        //Método que devuelve los detalles de un cliente en específico
        public ActionResult Details(string id)  
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cedula,nombre,apellido1,apellido2,telefono,direccionExacta,distrito,canton,provincia")] Cliente cliente)
        {
            if (ModelState.IsValid) //Para validar la cédula de cada cliente
            {
                if (!db.Cliente.Any(model => model.cedula == cliente.cedula)) //Si es una cédula que no existe, lo guarda con normalidad
                {
                    db.Cliente.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else //Si ya esa cédula existe, muestra un mensaje de error
                    Response.Write("<script>alert('La cédula de este cliente ya existe. Intente con una nueva');</script>");
            }

            return View(cliente);
        }

        // GET: Cliente/Edit/5
        //Método encargado de buscar el Cliente y poder editar sus datos y guardarlos.
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cedula,nombre,apellido1,apellido2,telefono,direccionExacta,distrito,canton,provincia")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges(); //Guarda los cambios al modificar
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        // GET: Cliente/Delete/5
        //Método que busca el cliente para que sea eliminado
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        //Método que borra el cliente de la tabla
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Cliente cliente = db.Cliente.Find(id);
            db.Cliente.Remove(cliente);
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
