using System.Net;
using System.Text;
using System.Text.Json;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class VeiculoRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    private readonly string adm = "adm@teste.com";
    private readonly string editor = "editor@teste.com";
    private readonly string senha = "123456";

    private static async Task<string?> ObterToken(string email, string senha)
    {
        var loginDTO = new LoginDTO(
            email,
            senha
        );

        var content = new StringContent(
            JsonSerializer.Serialize(loginDTO),
            Encoding.UTF8,
            "application/json"
        );

        var response = await Setup.client.PostAsync(
            "/administradores/login",
            content
        );

        var administrador =
            JsonSerializer.Deserialize<AdministradorLogado>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        return administrador?.Token;
    }

    [TestMethod]
    public async Task DeveRetornar401SemToken()
    {
        var response = await Setup.client.GetAsync("/veiculos");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar403ComTokenSemAutorizacao()
    {
        // Arrange
        var token = await ObterToken(editor, senha);
        Assert.IsNotNull(token, "Falha ao obter token de autenticação.");

        var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                "/veiculos/1"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar200ComTokenValido()
    {
        // Arrange
        var token = await ObterToken(adm, senha);
        Assert.IsNotNull(token, "Falha ao obter token de autenticação.");

        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/veiculos"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar404AoDeletarVeiculoInexistente()
    {
        // Arrange
        var token = await ObterToken(adm, senha);
        Assert.IsNotNull(token, "Falha ao obter token de autenticação.");

        var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                "/veiculos/9999"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveCadastrarVeiculoComSucesso()
    {
        // Arrange
        var token = await ObterToken(adm, senha);

        var veiculo = new VeiculoDTO
        (
            Nome: "Civic",
            Marca: "Honda",
            Ano: 2020
        );

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/veiculos"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(veiculo),
                Encoding.UTF8,
                "application/json"
            );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(
            HttpStatusCode.Created,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornarBadRequestAoCadastrarVeiculoComAnoMenorQue1950()
    {
        // Arrange
        var token = await ObterToken(adm, senha);

        var veiculo = new VeiculoDTO
        (
            Nome: "Fusca",
            Marca: "Volkswagen",
            Ano: 1940
        );

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/veiculos"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(veiculo),
                Encoding.UTF8,
                "application/json"
            );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornarBadRequestAoCadastrarVeiculoComNomeVazio()
    {
        // Arrange
        var token = await ObterToken(adm, senha);

        var veiculo = new VeiculoDTO
        (
            Nome: "",
            Marca: "Honda",
            Ano: 2020
        );

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/veiculos"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(veiculo),
                Encoding.UTF8,
                "application/json"
            );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }

    [TestMethod]
    public async Task DeveRetornarBadRequestAoCadastrarVeiculoComMarcaVazia()
    {
        // Arrange
        var token = await ObterToken(adm, senha);

        var veiculo = new VeiculoDTO
        (
            Nome: "Civic",
            Marca: "",
            Ano: 2020
        );

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "/veiculos"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        request.Content =
            new StringContent(
                JsonSerializer.Serialize(veiculo),
                Encoding.UTF8,
                "application/json"
            );

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(
            HttpStatusCode.BadRequest,
            response.StatusCode
        );
    }
}