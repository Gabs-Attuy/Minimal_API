namespace Minimal_API.Dominio.Interfaces;

public interface ISenhaHasher
{
    string HashSenha(string senha);
    bool VerificarSenha(string senha, string senhaHash);
}