using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Services;
using Minimal_API.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class VeiculoServiceTest
{
    private static DbContexto CriarContextoDeTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TesteParaSalvarVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2020
        };

        var veiculoServico = new VeiculoService(context);

        // Act
        veiculoServico.Incluir(veiculo);

        // Assert
        Assert.AreEqual(1, veiculoServico.Todos(1)?.Count);
    }

    [TestMethod]
    public void TestandoBuscaPorIdExistente()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2020
        };

        var veiculoServico = new VeiculoService(context);
        veiculoServico.Incluir(veiculo);

        // Act
        var veiculoEncontrado = veiculoServico.BuscarPorId(veiculo.Id);

        // Assert
        Assert.IsNotNull(veiculoEncontrado);
        Assert.AreEqual(veiculo.Nome, veiculoEncontrado.Nome);
    }

    [TestMethod]
    public void TestandoBuscaPorIdInexistente()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculoServico = new VeiculoService(context);

        // Act
        var veiculoEncontrado = veiculoServico.BuscarPorId(999);

        // Assert
        Assert.IsNull(veiculoEncontrado);
    }

    [TestMethod]
    public void TestandoExclusaoDeVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2020
        };

        var veiculoServico = new VeiculoService(context);
        veiculoServico.Incluir(veiculo);

        // Act
        veiculoServico.Apagar(veiculo);

        // Assert
        Assert.AreEqual(0, veiculoServico.Todos(1)?.Count);
    }

    [TestMethod]
    public void TestandoAtualizacaoDeVeiculo()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2020
        };

        var veiculoServico = new VeiculoService(context);
        veiculoServico.Incluir(veiculo);

        // Act
        veiculo.Nome = "Civic";
        veiculo.Marca = "Honda";
        veiculoServico.Atualizar(veiculo);

        // Assert
        var veiculoAtualizado = veiculoServico.BuscarPorId(veiculo.Id);
        Assert.IsNotNull(veiculoAtualizado);
        Assert.AreEqual("Civic", veiculoAtualizado.Nome);
        Assert.AreEqual("Honda", veiculoAtualizado.Marca);
        Assert.AreEqual(2020, veiculoAtualizado.Ano);
    }

    [TestMethod]
    public void TestandoBuscaComFiltro()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        var veiculo1 = new Veiculo
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2020
        };
        var veiculo2 = new Veiculo
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2021
        };

        var veiculoServico = new VeiculoService(context);
        veiculoServico.Incluir(veiculo1);
        veiculoServico.Incluir(veiculo2);

        // Act
        var veiculosFiltrados = veiculoServico.Todos(1, nome: "Civic");

        // Assert
        Assert.IsNotNull(veiculosFiltrados);
        Assert.AreEqual(1, veiculosFiltrados.Count);
        Assert.AreEqual("Civic", veiculosFiltrados[0].Nome);
    }

    [TestMethod]
    public void TestandoListagemComPaginacao()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos");

        for (int i = 1; i <= 25; i++)
        {
            var veiculo = new Veiculo
            {
                Nome = $"Veiculo{i}",
                Marca = "Marca",
                Ano = 2020
            };
            var veiculoServico = new VeiculoService(context);
            veiculoServico.Incluir(veiculo);
        }

        // Act
        var primeiraPagina = new VeiculoService(context).Todos(1);
        var segundaPagina = new VeiculoService(context).Todos(2);
        var terceiraPagina = new VeiculoService(context).Todos(3);

        // Assert
        Assert.IsNotNull(primeiraPagina);
        Assert.IsNotNull(segundaPagina);
        Assert.IsNotNull(terceiraPagina);
        Assert.AreEqual(10, primeiraPagina.Count);
        Assert.AreEqual(10, segundaPagina.Count);
        Assert.AreEqual(5, terceiraPagina.Count);
    }
}