using System.ComponentModel.DataAnnotations;

namespace WebApplication1_API.Modelos.Dto
{
    public class NumeroVillaCreateDto
    {
        [Required]
        public int VillaNumero { get; set; }
        
        [Required]
        public int VillaId { get; set; }
        public string DetalleEspecial { get; set; }
    }
}
