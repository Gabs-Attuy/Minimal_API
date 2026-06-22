using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Infraestrutura.Db;

namespace Minimal_API.Dominio.Services;

public class AdministradorService : IAdministradorService
{
    private readonly DbContexto _contexto;
    private readonly ISenhaHasher _senhaHasher;
    public AdministradorService(DbContexto contexto, ISenhaHasher senhaHasher)
    {
        _contexto = contexto;
        _senhaHasher = senhaHasher;
    }

    public Administrador? Login(LoginDTO login)
    {
        var administrador = _contexto.Administradores.FirstOrDefault(a => a.Email == login.Email);
        if (administrador != null && _senhaHasher.VerificarSenha(login.Senha, administrador.Senha))
        {
            return administrador;
        }

        return null;
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Senha = _senhaHasher.HashSenha(administrador.Senha);

        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
        {
            query = query
                .Skip(((int)pagina - 1) * itensPorPagina)
                .Take(itensPorPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administradores.FirstOrDefault(a => a.Id == id);
    }
}