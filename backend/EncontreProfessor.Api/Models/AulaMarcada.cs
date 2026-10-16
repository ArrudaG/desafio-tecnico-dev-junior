namespace EncontreProfessor.Api.Models;

using System.Text.Json.Serialization;

public class AulaMarcada
{
    [JsonPropertyName("dia")]
    public string Dia { get; set; } = string.Empty;
    [JsonPropertyName("inicio")]
    public string Inicio { get; set; } = string.Empty;
    [JsonPropertyName("duracaoMinutos")]
    public int DuracaoMinutos { get; set; }
}