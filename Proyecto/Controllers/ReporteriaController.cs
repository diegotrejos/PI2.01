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
    public class ReporteriaController : Controller
    {
        public string usuario = "";
        public string cedula = "";
        public string proy = "";
        public List<Proyecto.Models.ViewModels.TotalReqPorCliente> lista_totalReq = new List<Proyecto.Models.ViewModels.TotalReqPorCliente>();

        private Gr02Proy3Entities db = new Gr02Proy3Entities();
        Proyecto.Controllers.ProyectoController proyController = new Proyecto.Controllers.ProyectoController();
        // GET: Reporteria
        public ActionResult Index()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;
            return View();
        }

        public ActionResult DesarrolladoresProyDis()
        {
            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            List<EmpleadoDesarrollador> empleados = new EmpleadoDesarrolladorController().getEmpleados();
            
            TempData["empleadosDisponibles"] = empleados;
            List<Proyecto.Models.ViewModels.infoEmpleados> lista_datos = new List<Proyecto.Models.ViewModels.infoEmpleados>();

            int longitudArray = new EmpleadoDesarrolladorController().getNoDisponibles();


            //info_empleados[] arrayInfo = new info_empleados[longitudArray];

            //arrayInfo[1].longitud = longitudArray;

            new EquipoController().llenarArray(lista_datos);

            new RequerimientoController().llenarArray(lista_datos); //importante aqui la orden, primero se tiene que que llenar la array en equipo controller

            calculoDurEstimada(lista_datos);

            ordenarListaEmpleados(lista_datos);

            //ordenamientoElka(lista_datos,longitudArray);

            TempData["datos"] = lista_datos;
            return View();
        }

        public ActionResult TotalReqTerminadosEnEjecucion()
        {

            string usuario = System.Web.HttpContext.Current.Session["rol"] as string;
            string proy = System.Web.HttpContext.Current.Session["proyecto"] as string;
            string cedula = System.Web.HttpContext.Current.Session["cedula"] as string;

            //List<Proyecto.Models.ViewModels.TotalReqPorCliente> lista_totalReq = new List<Proyecto.Models.ViewModels.TotalReqPorCliente>();


            List<Proyecto.Models.Proyecto> proyectos = new ProyectoController().GetProyectosDeCliente(cedula);

            int index = 0;

            foreach (var item in proyectos)
            {
                lista_totalReq.Add(new Proyecto.Models.ViewModels.TotalReqPorCliente());
                lista_totalReq.Add(new Proyecto.Models.ViewModels.TotalReqPorCliente());
                //para los requerimientos en ejecucion
                lista_totalReq[index].nombreProy = item.nombre;
                lista_totalReq[index].durEstimada = (DateTime)item.fechaInicio;
                lista_totalReq[index].nombreCliente = item.Cliente.nombre;
                lista_totalReq[index].apellidoCliente = item.Cliente.apellido1;
                //para los requerimientos finalizados
                lista_totalReq[index + 1].nombreProy = item.nombre;
                lista_totalReq[index + 1].durEstimada = (DateTime)item.fechaInicio;
                lista_totalReq[index + 1].nombreCliente = item.Cliente.nombre;
                lista_totalReq[index + 1].apellidoCliente = item.Cliente.apellido1;
                new RequerimientoController().llenarListaReq(lista_totalReq, item.nombre,index);
                index += 2;
            }

            TempData["Lista"] = lista_totalReq;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalReqTerminadosEnEjecucion(string filtro)
        {
            int index = 0;
            List<Proyecto.Models.ViewModels.TotalReqPorCliente> lista_totalReqFiltrada = new List<Proyecto.Models.ViewModels.TotalReqPorCliente>();
            foreach (var item in lista_totalReq)
            {
                if (item.nombreProy == filtro)
                {
                    lista_totalReqFiltrada.Add(new Models.ViewModels.TotalReqPorCliente());
                    lista_totalReqFiltrada[index++] = item;
                }
            }
            TempData["Lista"] = lista_totalReqFiltrada;
            return View();
        }

            private void calculoDurEstimada(List<Proyecto.Models.ViewModels.infoEmpleados> info)
        {
            foreach (var item1 in info)
            {
                double totalHoras = 0;
                foreach (var item in item1.requerimientos)
                {
                    totalHoras += item.duracionEstimada;
                }
                totalHoras = totalHoras / 8;
                item1.durEstimada = (DateTime)item1.equipo.Proyecto.fechaInicio;
                item1.durEstimada = item1.durEstimada.AddDays(totalHoras);
              
            }
        }

        private void ordenarListaEmpleados(List<Proyecto.Models.ViewModels.infoEmpleados> info)
        {
            int elementos = 0; 
            foreach (var item in info)
            {
                ++elementos;
            }
            int marca = 0;
            int comparador = 0;
            Proyecto.Models.ViewModels.infoEmpleados aux;
            int k; 
            for (int i = 0; i < elementos; ++i)
            {
                for (k = elementos - 1; k > marca; --k)
                {
                    comparador = DateTime.Compare(info[k - 1].durEstimada, info[k].durEstimada);
                    if (comparador > 0)
                    {
                        aux = info[k - 1];
                        info[k - 1] = info[k];
                        info[k] = aux;
                    }
                }
                ++marca;
            }
        }
        public SelectList getProyectos(String rol, String cedula)
        {
            return this.proyController.getProyectos(rol, cedula);
        }
    }
}
