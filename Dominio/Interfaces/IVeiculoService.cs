using Minimal_API.Dominio.Entidades;

namespace Minimal_API.Dominio.Interfaces;

public interface IVeiculoService
{
    List<Veiculo>? Todos(int? pagina = 1,  string? nome = null, string? marca = null);

    Veiculo? BuscarPorId(int id);

    void Incluir(Veiculo veiculo);

    void Atualizar(Veiculo veiculo);

    void Apagar(Veiculo veiculo);
}