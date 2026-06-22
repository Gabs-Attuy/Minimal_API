using System.ComponentModel.DataAnnotations;
using Minimal_API.Dominio.Enuns;

namespace Minimal_API.Dominio.DTOs;

public record AdministradorDTO(
    [Required(ErrorMessage = "O campo Email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
    string Email,

    [Required(ErrorMessage = "O campo Senha é obrigatório.")]
    [MinLength(6, ErrorMessage = "O campo Senha deve conter no mínimo 6 caracteres.")]
    string Senha,
    
    Perfil? Perfil
);