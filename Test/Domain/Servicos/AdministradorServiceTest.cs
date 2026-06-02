using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Services;
using Minimal_API.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServiceTest
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
    public void TesteParaSalvarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorService(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count);
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorService = new AdministradorService(context);

        // Act
        administradorService.Incluir(adm);
        var admDoBanco = administradorService.BuscarPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }
}