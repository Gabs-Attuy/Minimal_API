using Minimal_API.Dominio.Enuns;

namespace Minimal_API.Dominio.DTOs;

public record AdministradorDTO(
    string Email,
    string Senha,
    Perfil? Perfil
);