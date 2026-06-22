using System.Net;
using System.Text;
using System.Text.Json;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]
public class AdministradorRequestTest
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

    [TestMethod]
    public async Task DeveRetornarOkEAdministradorComTokenAoFazerLoginComCredenciaisValidas()
    {
        // Arrange
        var loginDTO = new LoginDTO
        (
            "adm@teste.com",
            "123456"
        );

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email ?? "");
        Assert.IsNotNull(admLogado?.Perfil ?? "");
        Assert.IsNotNull(admLogado?.Token ?? "");
    }

    [TestMethod]
    public async Task DeveRetornar401ComCredenciaisInvalidas()
    {
        // Arrange
        var loginDTO = new LoginDTO
        (
            "adm@teste.com",
            "12345"
        );

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/administradores/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar401SemToken()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/administradores");

        // Act
        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornarOkComTokenValido()
    {
        // Arrange
        var loginDTO = new LoginDTO
        (
            "adm@teste.com",
            "123456"
        );

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");
        var responseLogin = await Setup.client.PostAsync("/administradores/login", content);
        var token = JsonSerializer.Deserialize<AdministradorLogado>(await responseLogin.Content.ReadAsStringAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })?.Token;

        // Act
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/administradores"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornar403ParaUsuarioSemPermissao()
    {
        // Arrange
        var loginDTO = new LoginDTO
        (
            "editor@teste.com",
            "123456"
        );

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");
        var responseLogin = await Setup.client.PostAsync("/administradores/login", content);
        var token = JsonSerializer.Deserialize<AdministradorLogado>(await responseLogin.Content.ReadAsStringAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })?.Token;

        // Act
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                "/administradores"
            );

        request.Headers.Add(
            "Authorization",
            $"Bearer {token}"
        );

        var response = await Setup.client.SendAsync(request);

        // Assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}