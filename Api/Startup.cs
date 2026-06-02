using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_API.Dominio.Entidades;
using Minimal_API.Dominio.Enuns;
using Minimal_API.Dominio.Interfaces;
using Minimal_API.Dominio.ModelViews;
using Minimal_API.Dominio.Services;
using Minimal_API.Dominio.DTOs;
using Minimal_API.Infraestrutura.Db;

namespace Minimal_API;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? "";
    }

    private string key = "";
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorService, AdministradorService>();
        services.AddScoped<IVeiculoService, VeiculoService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion


            #region Administradores

            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new ("Email", administrador.Email),
                    new ("Perfil", administrador.Perfil),
                    new (ClaimTypes.Role, administrador.Perfil),
                };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }


            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) =>
            {
                var administrador = administradorService.Login(loginDTO);
                if (administrador != null)
                {
                    var token = GerarTokenJwt(administrador);
                    return Results.Ok(new AdministradorLogado(
                        administrador.Email,
                        administrador.Perfil,
                        token
                    ));
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Administradores");


            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorService administradorService) =>
            {
                var administradoresQuery = administradorService.Todos(pagina);

                var administradores = new List<AdministradorModelView>();
                foreach (var administrador in administradoresQuery)
                {
                    administradores.Add(new AdministradorModelView(
                        administrador.Id,
                        administrador.Email,
                        administrador.Perfil
                    ));
                }

                return Results.Ok(administradores);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");


            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorService administradorService) =>
            {
                var administradorQuery = administradorService.BuscarPorId(id);

                if (administradorQuery == null)
                    return Results.NotFound();

                var administrador = new AdministradorModelView(
                    administradorQuery.Id,
                    administradorQuery.Email,
                    administradorQuery.Perfil
                );

                return Results.Ok(administrador);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");


            endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorService administradorService) =>
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = []
                };

                if (string.IsNullOrEmpty(administradorDTO.Email))
                    validacao.Mensagens.Add("O email é obrigatório.");

                if (string.IsNullOrEmpty(administradorDTO.Senha))
                    validacao.Mensagens.Add("A senha é obrigatória.");

                if (administradorDTO.Perfil == null)
                    validacao.Mensagens.Add("O perfil é obrigatório.");

                if (validacao.Mensagens.Count != 0)
                    return Results.BadRequest(validacao);

                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };

                administradorService.Incluir(administrador);

                return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView(
                    administrador.Id,
                    administrador.Email,
                    administrador.Perfil
                ));
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Administradores");

            #endregion


            #region Veiculos

            ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrEmpty(veiculoDTO.Nome))
                    validacao.Mensagens.Add("O nome não pode ser vazio");

                if (string.IsNullOrEmpty(veiculoDTO.Marca))
                    validacao.Mensagens.Add("A Marca não pode ficar em branco");

                if (veiculoDTO.Ano < 1950)
                    validacao.Mensagens.Add("Veículo muito antigo, aceito somete anos superiores a 1950");

                return validacao;
            }


            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
            {
                var validacao = validaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };
                veiculoService.Incluir(veiculo);

                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");


            endpoints.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoService veiculoService) =>
            {
                var veiculos = veiculoService.Todos(pagina);

                return Results.Ok(veiculos);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
            {
                var veiculo = veiculoService.BuscarPorId(id);
                if (veiculo == null) return Results.NotFound();
                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
            .WithTags("Veiculos");


            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoService veiculoService) =>
            {
                var veiculo = veiculoService.BuscarPorId(id);
                if (veiculo == null) return Results.NotFound();

                var validacao = validaDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veiculoService.Atualizar(veiculo);

                return Results.Ok(veiculo);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");


            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoService veiculoService) =>
            {
                var veiculo = veiculoService.BuscarPorId(id);
                if (veiculo == null) return Results.NotFound();

                veiculoService.Apagar(veiculo);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
            .WithTags("Veiculos");

            #endregion
        });
    }
}