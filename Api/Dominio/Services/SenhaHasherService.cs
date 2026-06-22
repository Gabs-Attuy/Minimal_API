using Microsoft.AspNetCore.Identity;
using Minimal_API.Dominio.Interfaces;

namespace Minimal_API.Dominio.Services;

public class SenhaHasherService : ISenhaHasher
{
    private readonly PasswordHasher<string> _hasher = new();

    public string HashSenha(string senha)
    {
        return _hasher.HashPassword(null!, senha);
    }

    public bool VerificarSenha(string senha, string senhaHash)
    {
        var resultado = _hasher.VerifyHashedPassword(
            null!,
            senhaHash,
            senha
        );

        return resultado == PasswordVerificationResult.Success;
    }
}