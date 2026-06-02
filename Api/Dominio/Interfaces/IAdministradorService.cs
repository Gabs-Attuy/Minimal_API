using Minimal_API.Dominio.DTOs;
using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Dominio.Interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDTO login);

    Administrador Incluir(Administrador administrador);

    Administrador? BuscarPorId(int id);
    List<Administrador> Todos(int? pagina);
}