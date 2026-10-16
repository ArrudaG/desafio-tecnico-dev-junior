using System.Text.Json.Serialization;

namespace EncontreProfessor.Api.Models;

public class Professor
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
    [JsonPropertyName("idiomas")]
    public List<string> Idiomas { get; set; } = [];
    [JsonPropertyName("agendaBloqueada")]
    public bool AgendaBloqueada { get; set; }
    [JsonPropertyName("disponibilidade")]
    public List<Disponibilidade> Disponibilidade { get; set; } = [];
    [JsonPropertyName("aulasMarcadas")]
    public List<AulaMarcada> AulasMarcadas { get; set; } = [];
}