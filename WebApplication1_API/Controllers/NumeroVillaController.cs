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
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger; //nos muestra informacion del proceso en consola
        private readonly IVillaRepositorio _villaRepositorio;
        private readonly INumeroVillaRepositorio _numeroVillaRepositorio;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepositorio, 
            INumeroVillaRepositorio numeroVillaRepositorio, IMapper mapper)
        {
            _logger = logger;
            _villaRepositorio = villaRepositorio;
            _numeroVillaRepositorio = numeroVillaRepositorio;
            _mapper = mapper;
            _apiResponse = new();
        }

        [HttpGet] //nos devuelve TODA la lista
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener Numeros Villas");

                IEnumerable<NumeroVilla> numeroVillaList = await _numeroVillaRepositorio.ObtenerTodos();

                _apiResponse.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numeroVillaList);
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _apiResponse;
        }

        
        [HttpGet("id:int", Name = "GetNumeroVilla")] //nos de vuelve UN(1) elemento de la lista por Id
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer Numero Villa con Id " + id);
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsExitoso = false;
                    return BadRequest(_apiResponse);
                }

                var numeroVilla = await _numeroVillaRepositorio.Obtener(v => v.VillaNumero == id);

                if (numeroVilla == null)
                {
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.IsExitoso = false;
                    return NotFound(_apiResponse);
                }

                _apiResponse.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                
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
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); //verifica que se cumplan las validaciones de campos
                }

                if (await _numeroVillaRepositorio.Obtener(v => v.VillaNumero == createDto.VillaNumero) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El numero de Villa ya existe!");
                    return BadRequest(ModelState);
                }

                if (await _villaRepositorio.Obtener(v => v.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El Id de la Villa No existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);

                modelo.FechaCreacion = DateTime.Now;
                //modelo.FechaActualizacion = DateTime.Now;
                await _numeroVillaRepositorio.Crear(modelo);
                _apiResponse.Resultado = modelo;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNumero }, _apiResponse);
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
        public async Task<IActionResult> DeleteNumeroVilla(int id) //IAction porque retorna NoContent y no necesita mapeo
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var numeroVilla = await _numeroVillaRepositorio.Obtener(v => v.VillaNumero == id);

                if (numeroVilla == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                await _numeroVillaRepositorio.Remover(numeroVilla);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;

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
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.VillaNumero)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            if (await _villaRepositorio.Obtener(v => v.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ClaveForanea", "El Id de la Villa No Existe!");
                return BadRequest(ModelState);
            }

            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);

            await _numeroVillaRepositorio.Actualizar(modelo);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;

            return Ok(_apiResponse);
        }
    }
}
