namespace EncontreProfessor.Tests;

using Moq;
using Xunit;
using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;

public class BuscadorProfessorServiceTests
{
    private readonly Mock<IProfessorRepository> _repositoryMock;
    private readonly BuscadorProfessorService _service;

    public BuscadorProfessorServiceTests()
    {
        _repositoryMock = new Mock<IProfessorRepository>();
        _service = new BuscadorProfessorService(_repositoryMock.Object);
    }

    [Fact]
    public void DeveRetornarNaoElegivel_QuandoProfessorNaoEnsinaOIdioma()
    {
        // Arrange
        var professores = new List<Professor>
        {
            new() { Id = 1, Nome = "Ana", Idiomas = ["Inglês"], AgendaBloqueada = false }
        };
        _repositoryMock.Setup(r => r.ObterTodos()).Returns(professores);

        var request = new BuscaProfessoresRequest
        {
            Idioma = "Espanhol",
            Dias = ["segunda"],
            InicioFaixa = "18:00",
            FimFaixa = "20:00",
            DuracaoMinutos = 60
        };

        // Act
        var resultado = _service.Buscar(request);

        // Assert
        Assert.Empty(resultado.Professores);
        Assert.Single(resultado.NaoElegiveis);
        Assert.Equal("Não ensina o idioma.", resultado.NaoElegiveis[0].Motivo);
    }

    [Fact]
    public void DeveRetornarNaoElegivel_QuandoAgendaDoProfessorEstaBloqueada()
    {
        // Arrange
        var professores = new List<Professor>
        {
            new() { Id = 1, Nome = "Bruno", Idiomas = ["Inglês"], AgendaBloqueada = true }
        };
        _repositoryMock.Setup(r => r.ObterTodos()).Returns(professores);

        var request = new BuscaProfessoresRequest
        {
            Idioma = "Inglês",
            Dias = ["segunda"],
            InicioFaixa = "18:00",
            FimFaixa = "20:00",
            DuracaoMinutos = 60
        };

        // Act
        var resultado = _service.Buscar(request);

        // Assert
        Assert.Empty(resultado.Professores);
        Assert.Single(resultado.NaoElegiveis);
        Assert.Equal("Agenda bloqueada.", resultado.NaoElegiveis[0].Motivo);
    }

    [Fact]
    public void DeveRetornarNaoElegivel_QuandoNaoHaHorarioEmComumEmTodosOsDiasPedidos()
    {
        // Arrange
        // Professor só tem disponibilidade de 18:00 às 19:00 na segunda, mas o aluno pediu segunda E terca
        var professores = new List<Professor>
        {
            new()
            {
                Id = 1,
                Nome = "Carlos",
                Idiomas = ["Espanhol"],
                AgendaBloqueada = false,
                Disponibilidade =
                [
                    new() { Dia = "segunda", InicioFaixa = "18:00", FimFaixa = "20:00" } // Faltou terça
                ]
            }
        };
        _repositoryMock.Setup(r => r.ObterTodos()).Returns(professores);

        var request = new BuscaProfessoresRequest
        {
            Idioma = "Espanhol",
            Dias = ["segunda", "terca"],
            InicioFaixa = "18:00",
            FimFaixa = "20:00",
            DuracaoMinutos = 60
        };

        // Act
        var resultado = _service.Buscar(request);

        // Assert
        Assert.Empty(resultado.Professores);
        Assert.Single(resultado.NaoElegiveis);
        Assert.Equal("Sem horário em comum nos dias pedidos.", resultado.NaoElegiveis[0].Motivo);
    }

    [Fact]
    public void DevePermitirHorarioEncostadoAOutraAula_SemConsiderarSobreposicao()
    {
        // Arrange
        // Professor tem aula marcada das 18:00 às 19:00. O aluno quer aula das 19:00 às 20:00.
        // Encostar no horário de término (19:00) deve ser PERMITIDO.
        var professores = new List<Professor>
        {
            new()
            {
                Id = 1,
                Nome = "Diego",
                Idiomas = ["Inglês"],
                AgendaBloqueada = false,
                Disponibilidade = [new() { Dia = "segunda", InicioFaixa = "18:00", FimFaixa = "21:00" }],
                AulasMarcadas = [new() { Dia = "segunda", Inicio = "18:00", DuracaoMinutos = 60 }]
            }
        };
        _repositoryMock.Setup(r => r.ObterTodos()).Returns(professores);

        var request = new BuscaProfessoresRequest
        {
            Idioma = "Inglês",
            Dias = ["segunda"],
            InicioFaixa = "19:00",
            FimFaixa = "20:00",
            DuracaoMinutos = 60
        };

        // Act
        var resultado = _service.Buscar(request);

        // Assert
        Assert.Single(resultado.Professores);
        Assert.Contains("19:00", resultado.Professores[0].Horarios);
    }

    [Fact]
    public void DeveOrdenarProfessores_PorMenosAulasNaSemanaEOrdemAlfabetica()
    {
        // Arrange
        // Prof A: 2 aulas marcadas, nome "Zélia"
        // Prof B: 1 aula marcada, nome "Beatriz"
        // Prof C: 1 aula marcada, nome "Ana"
        // Ordem esperada no resultado: Ana (1 aula), Beatriz (1 aula), Zélia (2 aulas)
        var professores = new List<Professor>
        {
            new()
            {
                Id = 1, Nome = "Zélia", Idiomas = ["Inglês"], AgendaBloqueada = false,
                Disponibilidade = [new() { Dia = "segunda", InicioFaixa = "18:00", FimFaixa = "20:00" }],
                AulasMarcadas = [new() { Dia = "terca", Inicio = "10:00", DuracaoMinutos = 60 }, new() { Dia = "quarta", Inicio = "10:00", DuracaoMinutos = 60 }]
            },
            new()
            {
                Id = 2, Nome = "Beatriz", Idiomas = ["Inglês"], AgendaBloqueada = false,
                Disponibilidade = [new() { Dia = "segunda", InicioFaixa = "18:00", FimFaixa = "20:00" }],
                AulasMarcadas = [new() { Dia = "terca", Inicio = "10:00", DuracaoMinutos = 60 }]
            },
            new()
            {
                Id = 3, Nome = "Ana", Idiomas = ["Inglês"], AgendaBloqueada = false,
                Disponibilidade = [new() { Dia = "segunda", InicioFaixa = "18:00", FimFaixa = "20:00" }],
                AulasMarcadas = [new() { Dia = "terca", Inicio = "10:00", DuracaoMinutos = 60 }]
            }
        };
        _repositoryMock.Setup(r => r.ObterTodos()).Returns(professores);

        var request = new BuscaProfessoresRequest
        {
            Idioma = "Inglês",
            Dias = ["segunda"],
            InicioFaixa = "18:00",
            FimFaixa = "19:00",
            DuracaoMinutos = 60
        };

        // Act
        var resultado = _service.Buscar(request);

        // Assert
        Assert.Equal(3, resultado.Professores.Count);
        Assert.Equal("Ana", resultado.Professores[0].Nome);
        Assert.Equal("Beatriz", resultado.Professores[1].Nome);
        Assert.Equal("Zélia", resultado.Professores[2].Nome);
    }
}