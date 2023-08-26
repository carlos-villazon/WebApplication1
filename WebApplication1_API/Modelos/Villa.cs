﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1_API.Modelos
{
    public class Villa
    {
        [Key] //indicamos la primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //genera el Id y lo aumenta de 1 en 1
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        
        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public int MetrosCuadrados { get; set; }
        public string ImagenUrl { get; set; }
        public string Amenidad { get; set; }
        public DateTime FechaCreacion { get; set; }
        //public DateTime FechaActualizacion { get; set; }
    }
}
