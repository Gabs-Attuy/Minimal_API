using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Infraestrutura.Db;

namespace Minimal_API.Dominio.Services;

public class AdministradorService : IAdministradorService
{
    private readonly DbContexto _contexto;

    public AdministradorService(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? Login(LoginDTO login)
    {
        return _contexto.Administradores.FirstOrDefault(a => a.Email == login.Email && a.Senha == login.Senha);
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
            query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        
        return query.ToList();
    }

    public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administradores.FirstOrDefault(a => a.Id == id);
    }
}