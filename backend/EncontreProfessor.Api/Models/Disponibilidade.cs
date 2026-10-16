using System.Text.Json.Serialization;

namespace EncontreProfessor.Api.Models;

public class Disponibilidade
{
    [JsonPropertyName("dia")]
    public string Dia  { get; set; } = string.Empty;
    [JsonPropertyName("inicio")]
    public string InicioFaixa { get; set; } = string.Empty;
    [JsonPropertyName("fim")]
    public string FimFaixa { get; set; } = string.Empty;
}