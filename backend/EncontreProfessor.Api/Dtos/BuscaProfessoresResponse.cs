namespace EncontreProfessor.Api.Dtos;

public class BuscaProfessoresResponse
{
    public List<ProfessorElegivelDto> Professores { get; set; } = [];
    public List<ProfessorNaoElegivelDto> NaoElegiveis { get; set; } = [];
}

public class ProfessorElegivelDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int AulasNaSemana  { get; set; }
    public List<string> Horarios { get; set; } = [];
}

public class ProfessorNaoElegivelDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
}