using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Proyecto.Models
{
    public class AutenticarController : Controller
    {
        private Gr02Proy3Entities db = new Gr02Proy3Entities();

        string user = "";
        string pass = "";
        public ActionResult Index()
        {
            var user = from a in db.Autenticar
                       where a.flag == true
                       select a;
            if (user.FirstOrDefault() != null)
            {
                user.FirstOrDefault().flag = false;
                db.SaveChanges();
            }
            return View();
        }
        // POST: Autenticar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Index(string id, string password)
        {
           
            if (id == null)
            {
                Response.Write("<script>alert('Usuario no digitado');</script>");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autenticar autenticar = db.Autenticar.Find(id, password);
            if (autenticar == null)
            {
                Response.Write("<script>alert('Datos Incorrectos');</script>");
                return View();
            }
            else if (autenticar.contrasena == password)
            {
               
                autenticar.flag = true;
                db.SaveChanges();
                return RedirectToAction("Index", "Proyecto");
            }
            else {
                Response.Write("<script>alert('Datos Incorrectos');</script>");
            }
            return View();
        }

        public string getUsuario() {
            var usuario = from a in db.Autenticar
                          where a.flag == true
                          select a;
            return usuario.FirstOrDefault().usuario;
        }

        // GET: Autenticar/Delete/5



    }
}
