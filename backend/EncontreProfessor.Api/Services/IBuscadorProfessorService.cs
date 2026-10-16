namespace EncontreProfessor.Api.Services;

using Dtos;

public interface IBuscadorProfessorService
{
    BuscaProfessoresResponse Buscar(BuscaProfessoresRequest request);
}