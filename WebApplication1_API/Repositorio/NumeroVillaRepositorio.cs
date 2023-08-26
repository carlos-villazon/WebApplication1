using WebApplication1_API.Datos;
using WebApplication1_API.Modelos;
using WebApplication1_API.Repositorio.IRepositorio;

namespace WebApplication1_API.Repositorio
{
    public class NumeroVillaRepositorio : Repositorio<NumeroVilla>, INumeroVillaRepositorio
    {
        private readonly AppDbContext _appDbContext;

        public NumeroVillaRepositorio(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<NumeroVilla> Actualizar(NumeroVilla entidad)
        {
            //entidad.FechaActualizacion = DateTime.Now;
            entidad.FechaCreacion = DateTime.Now;
            _appDbContext.NumeroVillas.Update(entidad);
            await _appDbContext.SaveChangesAsync();
            return entidad;
        }
    }
}
