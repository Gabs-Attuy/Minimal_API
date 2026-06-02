using Minimal_API.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TesteGetSetDePropriedades()
    {
        // Arrange
        var administrador = new Administrador();
        var id = 1;
        var email = "administrador@teste.com";
        var senha = "123456";
        var perfil = "Adm";

        // Act
        administrador.Id = id;
        administrador.Email = email;
        administrador.Senha = senha;
        administrador.Perfil = perfil;

        // Assert
        Assert.AreEqual(id, administrador.Id);
        Assert.AreEqual(email, administrador.Email);
        Assert.AreEqual(senha, administrador.Senha);
        Assert.AreEqual(perfil, administrador.Perfil);
    }
}