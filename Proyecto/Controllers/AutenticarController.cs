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

        /*Metodo que muestra la vista inicial de autenticar*/
        public ActionResult Index()
        {
            return View();
        }
        // POST: Autenticar/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*Metodo index de autenticar, este recibe una cedula y una contraseña valida si son 
         * correctos de serlo inicia en la sesion correspondiente, ademas se asigna el rol correspondiente del empleado
         * @param cedula
         * @param password
         * @return vista correspondiente con rol
         */
        [HttpPost]
        public ActionResult Index(string cedula, string password)
        {
            if (cedula == null)
            {
                Response.Write("<script>alert('Datos Incorrectos');</script>");
                return View();
            }
            string id = "";
            var cliente = from a in db.Cliente
                       where a.cedula == cedula
                       select a;
            var desarrollador = from a in db.EmpleadoDesarrollador
                          where a.cedulaED == cedula
                          select a;
            if (cliente.FirstOrDefault() != null)
            {
                id = "Cliente";
                System.Web.HttpContext.Current.Session["rol"] = "Cliente";
                var proyecto = from a in db.Proyecto
                               where a.cedulaCliente == cedula
                               select a;
                var nombre = from a in db.Cliente
                             where a.cedula == cedula
                             select a;
                System.Web.HttpContext.Current.Session["nombreUsuario"] = nombre.FirstOrDefault().nombre +" "+ nombre.FirstOrDefault().apellido1 + " "+ nombre.FirstOrDefault().apellido2;
                System.Web.HttpContext.Current.Session["proyecto"] = proyecto.FirstOrDefault().nombre;

            }
            else if (desarrollador.FirstOrDefault() != null)
            {
                id = "Desarrollador";
                System.Web.HttpContext.Current.Session["rol"] = "Desarrollador";
                var proyecto = from a in db.Equipo
                               where a.cedulaEM_FK == cedula
                               select a;
                if (proyecto.FirstOrDefault()!= null)
                {
                    if (proyecto.FirstOrDefault().rol == true)
                    {
                        id = "Lider";
                        System.Web.HttpContext.Current.Session["rol"] = "Lider";
                    }
                }
                var nombre = from a in db.EmpleadoDesarrollador
                             where a.cedulaED == cedula
                             select a;
                System.Web.HttpContext.Current.Session["nombreUsuario"] = nombre.FirstOrDefault().nombreED + " " + nombre.FirstOrDefault().apellido1ED + " " + nombre.FirstOrDefault().apellido2ED;

                if (proyecto.FirstOrDefault() != null)
                    System.Web.HttpContext.Current.Session["proyecto"] = proyecto.FirstOrDefault().nombreProy_FK;
                else
                    System.Web.HttpContext.Current.Session["proyecto"] = "";
            }
            else if (cedula == "Jefe") {
                id = "Jefe";
                System.Web.HttpContext.Current.Session["nombreUsuario"] = "Administrador";
                System.Web.HttpContext.Current.Session["rol"] = "Jefe";
            }
            System.Web.HttpContext.Current.Session["cedula"] = cedula;
            Autenticar autenticar = db.Autenticar.Find(id, password);
            if (autenticar == null)
            {
                Response.Write("<script>alert('Datos Incorrectos');</script>");
                return View();
            }
            else if (autenticar.contrasena == password)
            {
                db.SaveChanges();
                return RedirectToAction("Index", "Proyecto");
            }
            else {
                Response.Write("<script>alert('Datos Incorrectos');</script>");
            }
            return View();
        }





    }
}
