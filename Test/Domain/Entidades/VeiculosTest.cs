using Minimal_API.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculosTest
{
    [TestMethod]
    public void TesteGetSetDePropriedades()
    {
        // Arrange
        var veiculo = new Veiculo();
        var id = 1;
        var nome = "Corolla";
        var marca = "Toyota";
        var ano = 2020;

        // Act
        veiculo.Id = id;
        veiculo.Nome = nome;
        veiculo.Marca = marca;
        veiculo.Ano = ano;

        // Assert
        Assert.AreEqual(id, veiculo.Id);
        Assert.AreEqual(nome, veiculo.Nome);
        Assert.AreEqual(marca, veiculo.Marca);
        Assert.AreEqual(ano, veiculo.Ano);
    }
}