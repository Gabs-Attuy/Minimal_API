using System.ComponentModel.DataAnnotations;

namespace Minimal_API.Dominio.DTOs;

public record VeiculoDTO(
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    string Nome,

    [Required(ErrorMessage = "O campo Marca é obrigatório.")]
    string Marca,

    [Required(ErrorMessage = "O campo Ano é obrigatório.")]
    int Ano
);