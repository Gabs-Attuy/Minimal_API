using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimal_API.Dominio.DTOs;
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
        var senhaHasher = new SenhaHasherService();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        var administradorServico = new AdministradorService(context, senhaHasher);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count);
    }

    [TestMethod]
    public void TestandoBuscaPorIdExistente()
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

        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        administradorService.Incluir(adm);

        // Act
        var admDoBanco = administradorService.BuscarPorId(adm.Id);

        // Assert
        Assert.AreEqual(1, admDoBanco?.Id);
    }

    [TestMethod]
    public void TestandoBuscaPorIdInexistente()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        // Act
        var admDoBanco = administradorService.BuscarPorId(999);

        // Assert
        Assert.IsNull(admDoBanco);
    }

    [TestMethod]
    public void TestandoLoginComSucesso()
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

        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        var login = new LoginDTO
        (
            Email: adm.Email,
            Senha: adm.Senha
        );

        administradorService.Incluir(adm);

        // Act
        var loginResult = administradorService.Login(login);

        // Assert
        Assert.IsNotNull(loginResult);
    }

    [TestMethod]
    public void TestandoLoginSemSucesso()
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

        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        var login = new LoginDTO
        (
            Email: adm.Email,
            Senha: "senhaErrada"
        );

        administradorService.Incluir(adm);

        // Act
        var loginResult = administradorService.Login(login);

        // Assert
        Assert.IsNull(loginResult);
    }

    [TestMethod]
    public void TestandoListagemComPaginacao()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        for (int i = 1; i <= 11; i++)
        {
            var adm = new Administrador
            {
                Email = $"teste{i}@teste.com",
                Senha = "teste",
                Perfil = "Adm"
            };
            administradorService.Incluir(adm);
        }

        // Act
        var administradores = administradorService.Todos(1);

        // Assert
        Assert.IsNotNull(administradores);
        Assert.AreEqual(10, administradores.Count);
    }

    [TestMethod]
    public void TestandoSegundaPaginaNaListagemComPaginacao()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        for (int i = 1; i <= 11; i++)
        {
            var adm = new Administrador
            {
                Email = $"teste{i}@teste.com",
                Senha = "teste",
                Perfil = "Adm"
            };
            administradorService.Incluir(adm);
        }

        // Act
        var administradores = administradorService.Todos(2);

        // Assert
        Assert.IsNotNull(administradores);
        Assert.AreEqual(1, administradores.Count);
    }

    [TestMethod]
    public void TestandoSenhaHashadaAoIncluirAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var senhaSemHash = "teste";

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = senhaSemHash,
            Perfil = "Adm"
        };

        var senhaHasher = new SenhaHasherService();
        var administradorService = new AdministradorService(context, senhaHasher);

        // Act
        administradorService.Incluir(adm);
        var admDoBanco = administradorService.BuscarPorId(adm.Id);

        // Assert
        Assert.AreNotEqual(senhaSemHash, admDoBanco?.Senha);
    }
}