using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Services;

namespace Test.Mocks;

public class AdministradorServiceMock : IAdministradorService
{
    private readonly ISenhaHasher _senhaHasher;
    private readonly List<Administrador> administradores;

    public AdministradorServiceMock()
    {
        _senhaHasher = new SenhaHasherService();

        administradores =
        [
            new() {
                Id = 1,
                Email = "adm@teste.com",
                Senha = _senhaHasher.HashSenha("123456"),
                Perfil = "Adm"
            },
            new() {
                Id = 2,
                Email = "editor@teste.com",
                Senha = _senhaHasher.HashSenha("123456"),
                Perfil = "Editor"
            }
        ];
    }

    public Administrador? BuscarPorId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count + 1;
        administradores.Add(administrador);

        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var administrador = administradores
            .FirstOrDefault(a => a.Email == loginDTO.Email);

        if (administrador == null)
            return null;

        return _senhaHasher.VerificarSenha(
            loginDTO.Senha,
            administrador.Senha)
            ? administrador
            : null;
    }

    public List<Administrador> Todos(int? pagina)
    {
        return administradores;
    }
}