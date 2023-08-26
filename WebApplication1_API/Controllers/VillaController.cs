using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using WebApplication1_API.Datos;
using WebApplication1_API.Modelos;
using WebApplication1_API.Modelos.Dto;
using WebApplication1_API.Repositorio.IRepositorio;

namespace WebApplication1_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger; //nos muestra informacion del proceso en consola
        //private readonly AppDbContext _appDbContext; //realizamos CRUD con la bd
        private readonly IVillaRepositorio _villaRepositorio;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepositorio, IMapper mapper)
        {
            _logger = logger;
            //_appDbContext = appDbContext;
            _villaRepositorio = villaRepositorio;
            _mapper = mapper;
            _apiResponse = new();
        }

        
        [HttpGet] //nos devuelve TODA la lista
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            /*return new List<VillaDto>
            {
                new VillaDto {Id=1, Nombre="Vista a la Piscina"},
                new VillaDto {Id=2, Nombre="Vista a la Playa"}
            };*/

            try
            {
                _logger.LogInformation("Obtener las Villas");

                //IEnumerable<Villa> villaList = await _appDbContext.Villas.ToListAsync();
                IEnumerable<Villa> villaList = await _villaRepositorio.ObtenerTodos();

                _apiResponse.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _apiResponse.StatusCode = HttpStatusCode.OK;

                //return Ok(VillaStore.villaList);
                //return Ok(await _appDbContext.Villas.ToListAsync());
                //return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _apiResponse;
        }

        
        [HttpGet("id:int", Name="GetVilla")] //nos devuelve UN(1) elemento de la lista por Id
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Villa con Id: " + id);
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsExitoso = false;
                    return BadRequest(_apiResponse);
                }

                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                //var villa = await _appDbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                var villa = await _villaRepositorio.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsExitoso = false;
                    return NotFound(_apiResponse);
                }

                _apiResponse.Resultado = _mapper.Map<VillaDto>(villa);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                
                //return Ok(villa);
                //return Ok(_mapper.Map<VillaDto>(villa));
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _apiResponse;
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[FromBody] indica que vamos a recibir datos
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); //verifica que se cumplan las validaciones de campos
                }

                /*if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre ya existe!");
                    return BadRequest(ModelState);
                }*/

                /*if (await _appDbContext.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre ya existe!");
                    return BadRequest(ModelState);
                }*/

                if (await _villaRepositorio.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La Villa con ese Nombre ya existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                /*if (villaDto.Id > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }*/

                //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
                //VillaStore.villaList.Add(villaDto);

                Villa modelo = _mapper.Map<Villa>(createDto);

                /*Villa modelo = new()
                {
                    //Id = villaDto.Id, //aca el id se genera automaticamente
                    Nombre = createDto.Nombre,
                    Detalle = createDto.Detalle,
                    ImagenUrl = createDto.ImagenUrl,
                    Ocupantes = createDto.Ocupantes,
                    Tarifa = createDto.Tarifa,
                    MetrosCuadrados = createDto.MetrosCuadrados,
                    Amenidad = createDto.Amenidad
                };*/

                //await _appDbContext.Villas.AddAsync(modelo);
                //await _appDbContext.SaveChangesAsync();

                modelo.FechaCreacion = DateTime.Now;
                //modelo.FechaActualizacion = DateTime.Now;
                await _villaRepositorio.Crear(modelo);
                _apiResponse.Resultado = modelo;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                //return Ok(villaDto);
                //return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto);
                //return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _apiResponse;
        }

        
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) //IAction porque retorna NoContent y no necesita mapeo
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                //var villa = await _appDbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);
                var villa = await _villaRepositorio.Obtener(v => v.Id == id);

                if (villa == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                //VillaStore.villaList.Remove(villa);

                //_appDbContext.Villas.Remove(villa); //remove no es metodo asincrono
                //await _appDbContext.SaveChangesAsync();

                await _villaRepositorio.Remover(villa);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;

                //return NoContent();
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return BadRequest(_apiResponse);
        }

        
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            /*villa.Nombre = villaDto.Nombre;
            villa.Ocupantes = villaDto.Ocupantes;
            villa.MetrosCuadrados = villaDto.MetrosCuadrados;*/

            Villa modelo = _mapper.Map<Villa>(updateDto);

            /*Villa modelo = new()
            {
                Id = updateDto.Id,
                Nombre = updateDto.Nombre,
                Detalle = updateDto.Detalle,
                ImagenUrl = updateDto.ImagenUrl,
                Ocupantes = updateDto.Ocupantes,
                Tarifa = updateDto.Tarifa,
                MetrosCuadrados = updateDto.MetrosCuadrados,
                Amenidad = updateDto.Amenidad
            };*/

            //_appDbContext.Villas.Update(modelo); //update no es metodo asincrono
            //await _appDbContext.SaveChangesAsync();

            await _villaRepositorio.Actualizar(modelo);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;

            //return NoContent();
            return Ok(_apiResponse);
        }

        
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //var villa = await _appDbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            var villa = await _villaRepositorio.Obtener(v => v.Id == id, tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            /*VillaUpdateDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };*/

            if (villa == null)
            {
                return BadRequest();
            }

            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            /*Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };*/

            //_appDbContext.Villas.Update(modelo); //no es asincrono
            //await _appDbContext.SaveChangesAsync();

            await _villaRepositorio.Actualizar(modelo);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;

            //return NoContent();
            return Ok(_apiResponse);
        }
    }
}
