namespace EncontreProfessor.Api.Dtos;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class BuscaProfessoresRequest : IValidatableObject
{
    [Required(ErrorMessage = "O idioma é obrigatório.")]
    public string Idioma { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pelo menos um dia deve ser informado.")]
    [MinLength(1, ErrorMessage = "Pelo menos um dia deve ser informado.")]
    public List<string> Dias { get; set; } = [];
    
    [Required(ErrorMessage = "O início é obrigatório.")]
    public string InicioFaixa { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "O fim é obrigatório.")]
    public string FimFaixa { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "A duração da aula é obrigatório.")]
    public int DuracaoMinutos { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Idioma))
        {
            yield return new ValidationResult("O idioma não pode estar vazio.", [nameof(Idioma)]);
        }
        
        var diasValidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "segunda", "terca", "quarta", "quinta", "sexta", "sabado", "domingo"
        };

        foreach (var dia in Dias)
        {
            if (!diasValidos.Contains(dia))
            {
                yield return new ValidationResult($"Dia da semana inválido: '{dia}'.", [nameof(Dias)]);
            }
        }
        
        var regexHorario = new Regex(@"^(?:[01]\d|2[0-3]):[0-5]\d$");

        if (!regexHorario.IsMatch(InicioFaixa))
        {
            yield return new ValidationResult("O início deve estar no formato HH:mm.", [nameof(InicioFaixa)]);
        }
        
        if (!regexHorario.IsMatch(FimFaixa))
        {
            yield return new ValidationResult("O fim deve estar no formato HH:mm.", [nameof(FimFaixa)]);
        }

        if (TimeSpan.TryParse(InicioFaixa, out var inicio) && TimeSpan.TryParse(FimFaixa, out var fim))
        {
            if (fim <= inicio)
            {
                yield return new ValidationResult("O fim deve ser posterior ao início.", [nameof(FimFaixa)]);
            }
        }

        int[] duracoesPermitidas = [30, 60, 90];
        if (!duracoesPermitidas.Contains(DuracaoMinutos))
        {
            yield return new ValidationResult("A duração da aula deve ser 30, 60 ou 90 minutos", [nameof(DuracaoMinutos)]);
        }
    }
}