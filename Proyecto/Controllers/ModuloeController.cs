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
        /*Variables utilizadas para obtener la información del usuario en la sesión*/
        public string usuario = "";
        public string cedula = "";
        public string proy = "";

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        //incluyo la tabla de proyectos
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
       

        // GET: Moduloe
        public ActionResult Index()
        {
            //Inicializacion de variables de sesion 
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            var modulo = db.Modulo.Include(m => m.Proyecto);

            //variable para la vista
            ViewBag.user = usuario;

            if (usuario != "Jefe")
            {
                if (usuario == "Cliente") { //si soy cliente puedo solamente ver los modulos de mis proyectos
                    var obj = from a in db.Modulo
                              from b in db.Proyecto
                              from c in db.Cliente
                              where a.NombreProy == b.nombre
                              where b.cedulaCliente == c.cedula
                              where c.cedula == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
                else //si soy desarrollador puedo ver los módulos de mi proyecto 
                {
                    var obj = from a in db.Modulo
                              from b in db.Proyecto
                              from c in db.Equipo
                              from d in db.EmpleadoDesarrollador
                              where a.NombreProy == b.nombre
                              where b.nombre == c.nombreProy_FK
                              where c.cedulaEM_FK == d.cedulaED
                              where d.cedulaED == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
            }
            else //como jefe puedo ver todos los modulos 
            {
                return View(modulo.ToList());
            }
        }



        // POST: Moduloe
        [HttpPost]
        public ActionResult Index(string filtro)//filtro es el nombre del dropdown que me da el nombre de proyecto
        {
            //consulto para obtener solo los modulos de un proyecto



            //inicialización de variables de sesion
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            
            //variable de vista
            ViewBag.user = usuario;

            var modulo = db.Modulo.Include(m => m.Proyecto);

            if (usuario != "Jefe")
            {
                if (usuario == "Cliente") //si soy cliente puedo ver los módulos de mi proyecto
                {
                    var obj = from a in db.Modulo
                              from b in db.Proyecto
                              from c in db.Cliente
                              where a.NombreProy == filtro
                              where b.cedulaCliente == c.cedula
                              where c.cedula == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
                else //si soy desarrollador puedo ver los módulos de mi proyecto
                {
                    var obj = from a in db.Modulo
                              from b in db.Proyecto
                              from c in db.Equipo
                              from d in db.EmpleadoDesarrollador
                              where a.NombreProy == b.nombre
                              where b.nombre == c.nombreProy_FK
                              where b.nombre == filtro
                              where c.cedulaEM_FK == d.cedulaED
                              where d.cedulaED == cedula
                              select a;

                    return View(obj.Distinct().ToList());
                }
            }
            else //si soy jefe puedo ver todos los módulos
            {
                var query = from a in db.Modulo
                            where ((a.NombreProy.Equals(filtro)))
                            select a;
                return View(query.ToList());
            }



            
        }



        // GET: Moduloe/Details/5
        public ActionResult Details(int id, string nombreProy)
        {

         
                //Busca modulo con sus dos llaves
                var query = from a in db.Modulo
                            where (a.Id == id && a.NombreProy == nombreProy)
                            select a;

                var item = query.FirstOrDefault();

                if (item != null)
                    return View(item);
                else
                    return View("NotFound");

        }

        // GET: Moduloe/Create
        public ActionResult Create()
        {
          
                ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo");
                return View();
            
    
        }

        // POST: Moduloe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken] //no me valida si existen en el mismo proyecto de ves en cuando
        public ActionResult Create(Modulo modulo)
        {
           
                if (ModelState.IsValid)//Valida si Id y Proyecto esten correctos
                {


                   

                    if (db.Proyecto.Any(model => model.nombre == modulo.NombreProy))//Reviso que el proyecto seleccionado sea valido
                    {

                       
                        var queryCheck =
                       from a in db.Modulo
                       where (a.NombreProy  == modulo.NombreProy  && a.Nombre == modulo.Nombre)
                       select a.Nombre;
                       

                        if (!(queryCheck.Any()))
                                   
                        {
                        
                        string input = modulo.Nombre;
                        string output = input.Replace("or 1=1", " ");
                             output = output.Replace("modulo", "_");
                            modulo.Nombre = output;
                            

                            db.Modulo.Add(modulo);
                                db.SaveChanges();
                                return RedirectToAction("index");
                       
                        }
                        else
                        {

                            Response.Write("<script>alert('El nombre de este modulo ya existe en este proyecto. Intente con uno nuevo');</script>");

                        }

                    }
                    else//Si la cédula ya existe, muestra mensaje de error
                        Response.Write("<script>alert('Este proyecto no existe. Intente con otro');</script>");
                }
                ViewBag.NombreProy = new SelectList(db.Proyecto, "nombre", "objetivo", modulo.NombreProy);
            
            return View(modulo);
        }











        // GET: Moduloe/Edit/5
        public ActionResult Edit(int id, string nombreProy) // comunica con el modelo
        {
           



            var query = from a in db.Modulo
                           where  (a.Id == id && a.NombreProy == nombreProy  )   
                           select a;

     
            var item = query.FirstOrDefault();

            if (item != null)
                return View(item);
            else
                return View("NotFound");



        }



        // POST: Moduloe/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Modulo modulo)
        {
        
                if (ModelState.IsValid)
                {

                string input = modulo.Nombre;
                string output = input.Replace("or 1=1", " ");
                output = output.Replace("modulo", "_");
                modulo.Nombre = output;

                db.Entry(modulo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(modulo);
            
        }

    







        // GET: Moduloe/Delete/5
        public ActionResult Delete(int id, string nombreProy) // comunica con el modelo
        {

          

            var query = from a in db.Modulo
                        where (a.Id == id && a.NombreProy == nombreProy)
                        select a;


            var item = query.FirstOrDefault();

                if (item != null)
                    return View(item);
                else
                    return View("NotFound");

            

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "NombreProy,Id")] Modulo modulo)
        {
          
                db.Entry(modulo).State = EntityState.Deleted;
                db.Modulo.Remove(modulo);
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
        //METODO Q OBTIENE LISTA DE CONTROLLADOR DE PROYECTO
        public SelectList getProyectos(String rol, String cedula)
        {
            return this.proyController.getProyectos(rol, cedula);
        }

        public SelectList getProyectos()
        {
            return this.proyController.getProyectos();
        }

        public SelectList getModulos(string nombreProy)
        {

          

                var query =  from Modulo in db.Modulo
                              where Modulo.NombreProy == nombreProy
                              select Modulo.Nombre;
                        
                        
                return new SelectList(query);
            
        }

      




    }
}
