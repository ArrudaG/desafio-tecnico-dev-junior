namespace EncontreProfessor.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Dtos;
using Services;

[ApiController]
[Route("api/[controller]")]
public class ProfessoresController : ControllerBase
{
    private readonly IBuscadorProfessorService _buscadorService;

    public ProfessoresController(IBuscadorProfessorService buscadorService)
    {
        _buscadorService = buscadorService;
    }

    [HttpPost("busca")]
    [ProducesResponseType(typeof(BuscaProfessoresResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult BuscarProfessores([FromBody] BuscaProfessoresRequest request)
    {
        var resultado = _buscadorService.Buscar(request);
        return Ok(resultado);
    }
}