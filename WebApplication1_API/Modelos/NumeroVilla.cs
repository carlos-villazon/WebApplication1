using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1_API.Modelos
{
    public class NumeroVilla
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)] //NO genera el # automaticamente y no permite que se repita
        public int VillaNumero { get; set; }
        
        [Required]
        public int VillaId { get; set; }
        
        [ForeignKey("VillaId")]
        public Villa Villa { get; set; }
        public string DetalleEspecial { get; set; }
        public DateTime FechaCreacion { get; set; }
        //public DateTime FechaActualizacion { get; set; }
    }
}
