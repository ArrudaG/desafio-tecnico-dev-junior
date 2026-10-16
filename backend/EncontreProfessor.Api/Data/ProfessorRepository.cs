using System.Text.Json.Serialization;

namespace EncontreProfessor.Api.Data;

using System.Text.Json;
using Models;

public class ProfessorWrapper
{
    [JsonPropertyName("professores")]
    public List<Professor> Professores { get; set; } = [];
}

public class ProfessorRepository : IProfessorRepository
{
    private readonly List<Professor> _professores = [];

    public ProfessorRepository()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "professores.json");

        if (File.Exists(jsonPath))
        {
            var json = File.ReadAllText(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Desserializa no objeto wrapper que contém a propriedade "professores"
            var wrapper = JsonSerializer.Deserialize<ProfessorWrapper>(json, options);
            _professores = wrapper?.Professores ?? [];
        }
    }

    public IEnumerable<Professor> ObterTodos() => _professores;
}