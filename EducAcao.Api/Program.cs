using System.Security.Claims;
using System.Text;
using EducAcao.Api.Dados;
using EducAcao.Api.DTOs;
using EducAcao.Api.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbContexto>(stringConexao, ServerVersion.AutoDetect(stringConexao));

//Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();
builder.Services.AddScoped<TokenServico>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var chaveJwt = Encoding.ASCII.GetBytes(TokenServico.Chave);
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = TokenServico.Emissor,
            ValidAudience = TokenServico.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(chaveJwt)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

//cria usuario
app.MapPost("/usuario", async (CadastroUsuarioDto body, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var novoUsuario = new Usuario(body.Nome, body.Email, body.Senha, body.DataNascimento);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        return Results.Created();
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});

//login
app.MapPost("/login", async (LoginUsuarioDto body, UsuarioServicos usuarioServicos, TokenServico tokenServico) =>
{
    try
    {
        var usuarioEncontrado = await usuarioServicos.BuscarPorEmailAsync(body.Email);

        if (usuarioEncontrado.Senha != body.Senha) return Results.BadRequest("Usuario ou senha estão errados!");

        if (usuarioEncontrado.Senha != body.Senha)
            return Results.BadRequest("Usuário ou senha estão errados!");

        var token = tokenServico.GerarToken(
            usuarioEncontrado.Id.ToString(),
            usuarioEncontrado.Email,
            usuarioEncontrado.Nome);

        return Results.Ok(new { Token = token });
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensagem = ex.Message });
    }
    catch
    {
        return Results.Problem(
            "Ocorreu um erro desconhecido, tente novamente mais tarde.");
    }
});

app.MapGet("/perfil", (ClaimsPrincipal usuario) =>
    {
        var userId = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = usuario.FindFirst(ClaimTypes.Email)?.Value;
        var nome = usuario.Identity?.Name;

        return Results.Ok(new
        {
            Mensagem = "Você acessou a rota protegida com sucesso!",
            UsuarioId = userId,
            Email = email,
            Nome = nome
        });
    })
    .RequireAuthorization();

app.Run();