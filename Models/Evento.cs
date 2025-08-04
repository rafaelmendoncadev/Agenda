using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agenda.Models;

public class Evento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória")]
    [Display(Name = "Data de Início")]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória")]
    [Display(Name = "Data de Fim")]
    public DateTime DataFim { get; set; }

    [Display(Name = "Dia Inteiro")]
    public bool DiaInteiro { get; set; }

    // Relacionamento com o usuário
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public IdentityUser? User { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;
}