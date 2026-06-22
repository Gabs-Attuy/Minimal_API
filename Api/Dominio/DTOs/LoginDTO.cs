using System.ComponentModel.DataAnnotations;

namespace Minimal_API.Dominio.DTOs;
public record LoginDTO(
    [Required(ErrorMessage = "O campo Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
    string Email,

    [Required(ErrorMessage = "O campo Senha é obrigatório.")]
    [MinLength(6, ErrorMessage = "O campo Senha deve conter no mínimo 6 caracteres.")]
    string Senha
);