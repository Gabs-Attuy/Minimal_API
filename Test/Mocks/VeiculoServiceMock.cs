using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Interfaces;

namespace Test.Mocks;

public class VeiculoServiceMock : IVeiculoService
{
    private readonly List<Veiculo> veiculos;

    public VeiculoServiceMock()
    {
        veiculos = new List<Veiculo>();
    }

    public Veiculo? BuscarPorId(int id)
    {
        return veiculos.Find(v => v.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count + 1;
        veiculos.Add(veiculo);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var index = veiculos.FindIndex(v => v.Id == veiculo.Id);
        if (index != -1)
        {
            veiculos[index] = veiculo;
        }
    }

    public void Apagar(Veiculo veiculo)
    {
        veiculos.Remove(veiculo);
    }

    public List<Veiculo>? Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => v.Nome.Contains(nome));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => v.Marca.Contains(marca));
        }

        int itensPorPagina = 10;

        if (pagina != null)
            query = query
            .Skip(((int)pagina - 1) * itensPorPagina)
            .Take(itensPorPagina);

        return query.ToList();
    }
}