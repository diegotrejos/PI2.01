//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Equipo
    {
        [Key] 
        [Display(Name = "Cedula de empleado")]
        public string cedulaEM_FK { get; set; } //metodo que asigna y devuelve la cedula del empleado en el proyecto

        [Key]
        [Display(Name = "Nombre de proyecto")]
        public string nombreProy_FK { get; set; } ////metodo que asigna y devuelve el nombre del Proyecto en el que esta el equipo

        [Display(Name = "Lider")]
        public bool rol { get; set; } //rol que cumple el empleado en el proyecto

        public virtual EmpleadoDesarrollador EmpleadoDesarrollador { get; set; }
        public virtual Proyecto Proyecto { get; set; }
    }
}
