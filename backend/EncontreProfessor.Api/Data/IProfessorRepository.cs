namespace EncontreProfessor.Api.Data;

using Models;

public interface IProfessorRepository
{
    IEnumerable<Professor> ObterTodos();
}