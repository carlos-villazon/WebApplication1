using WebApplication1_API.Datos;
using WebApplication1_API.Modelos;
using WebApplication1_API.Repositorio.IRepositorio;

namespace WebApplication1_API.Repositorio
{
    public class VillaRepositorio : Repositorio<Villa>, IVillaRepositorio
    {
        private readonly AppDbContext _appDbContext;

        public VillaRepositorio(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Villa> Actualizar(Villa entidad)
        {
            //entidad.FechaActualizacion = DateTime.Now;
            entidad.FechaCreacion = DateTime.Now;
            _appDbContext.Villas.Update(entidad);
            await _appDbContext.SaveChangesAsync();
            return entidad;
        }
    }
}
