using EncontreProfessor.Api.Data;

namespace EncontreProfessor.Api.Services;

using Dtos;
using Models;

public class BuscadorProfessorService : IBuscadorProfessorService
{
    private readonly IProfessorRepository _repository;

    public BuscadorProfessorService(IProfessorRepository repository)
    {
        _repository = repository;
    }

    public BuscaProfessoresResponse Buscar(BuscaProfessoresRequest request)
    {
        var professores = _repository.ObterTodos();
        
        return ExecutarBuscar(request, professores);
    }
    
    public BuscaProfessoresResponse ExecutarBuscar(BuscaProfessoresRequest request,  IEnumerable<Professor> professores)
    {
        var response = new BuscaProfessoresResponse();
        var inicio = TimeSpan.Parse(request.InicioFaixa);
        var fim = TimeSpan.Parse(request.FimFaixa);
        var duracao = TimeSpan.FromMinutes(request.DuracaoMinutos);
        var horariosCandidatos = GerarHorariosCandidatos(inicio, fim, duracao);

        foreach (var prof in professores)
        {
            bool ensinaIdioma = prof.Idiomas.Any(i => i.Equals(request.Idioma, StringComparison.OrdinalIgnoreCase));
            if (!ensinaIdioma)
            {
                response.NaoElegiveis.Add(new ProfessorNaoElegivelDto
                {
                    Id = prof.Id,
                    Nome = prof.Nome,
                    Motivo = "Não ensina o idioma."
                });
                continue;
            }

            if (prof.AgendaBloqueada)
            {
                response.NaoElegiveis.Add(new ProfessorNaoElegivelDto
                {
                    Id = prof.Id,
                    Nome = prof.Nome,
                    Motivo = "Agenda bloqueada."
                });
                continue;
            }
            
            var horariosValidos = new List<TimeSpan>();

            foreach (var inicioAula in horariosCandidatos)
            {
                var fimAula = inicioAula + duracao;
                bool livreEmTodosOsDias = request.Dias.All(dia =>
                    EstaLivreNoDia(prof, dia, inicioAula, fimAula));

                if (livreEmTodosOsDias)
                {
                    horariosValidos.Add(inicioAula);
                }
            }

            if (horariosValidos.Count == 0)
            {
                response.NaoElegiveis.Add(new ProfessorNaoElegivelDto
                {
                    Id = prof.Id,
                    Nome = prof.Nome,
                    Motivo = "Sem horário em comum nos dias pedidos."
                });
                continue;
            }
            
            response.Professores.Add(new ProfessorElegivelDto
            {
                Id = prof.Id,
                Nome = prof.Nome,
                AulasNaSemana = prof.AulasMarcadas.Count,
                Horarios = horariosValidos
                    .OrderBy(h => h)
                    .Select(h => h.ToString(@"hh\:mm"))
                    .ToList()
            });
        }
        
        response.Professores = response.Professores
            .OrderBy(p => p.AulasNaSemana)
            .ThenBy(p => p.Nome, StringComparer.OrdinalIgnoreCase)
            .ToList();
        
        return response;
    }

    private static List<TimeSpan> GerarHorariosCandidatos(TimeSpan inicio, TimeSpan fim, TimeSpan duracao)
    {
        var candidatos = new List<TimeSpan>();
        var atual = inicio;

        if (atual.Minutes % 30 != 0)
        {
            atual = atual.Add(TimeSpan.FromMinutes(30 - (atual.Minutes % 30)));
        }

        while (atual + duracao <= fim)
        {
            candidatos.Add(atual);
            atual = atual.Add(TimeSpan.FromMinutes(30));
        }

        return candidatos;
    }

    private static bool EstaLivreNoDia(Professor prof, string dia, TimeSpan inicioAula, TimeSpan fimAula)
    {
        bool cabeNaDisponibilidade = prof.Disponibilidade.Any(d =>
            d.Dia.Equals(dia, StringComparison.OrdinalIgnoreCase) &&
            TimeSpan.TryParse(d.InicioFaixa, out var inicioDisp) &&
            TimeSpan.TryParse(d.FimFaixa, out var fimDisp) &&
            inicioDisp <= inicioAula &&
            fimDisp >= fimAula);

        if (!cabeNaDisponibilidade)
            return false;

        bool temConflitoDeAula = prof.AulasMarcadas.Any(a =>
        {
            if (!a.Dia.Equals(dia, StringComparison.OrdinalIgnoreCase))
                return false;
            
            if (!TimeSpan.TryParse(a.Inicio, out var inicioMarcada))
                return false;
            
            var fimMarcada = inicioMarcada + TimeSpan.FromMinutes(a.DuracaoMinutos);

            return inicioAula < fimMarcada && fimAula > inicioMarcada;
        });
        
        return !temConflitoDeAula;
    }
}