using System.Runtime.CompilerServices;
using EducAcao.Api.Dados;
using EducAcao.Api.DTos;
using EducAcao.Api.DTOs;
using Educacao.Api.Servicos;
using EducAcao.Api.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddMySql<DbContexto>(stringConexao, ServerVersion.AutoDetect(stringConexao));
 

//Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();

builder.Services.AddScoped<TokenServico>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/usuario", async (CadastroUsuarioDto requisicao, UsuarioServicos usuarioServicos) =>

{
    try
    {
        var novoUsuario = new Usuario(requisicao.Nome, email: requisicao.Email, requisicao.Senha,
            requisicao.DataNascimento);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        
        return Results.Created();
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});

// Login
app.MapPost("/login", async (LoginUsuarioDto body, UsuarioServicos usuarioServicos) =>

{

    try
    {
        var usuarioEncontrado = await usuarioServicos.BuscaPorEmailAsync(body.Email);
        if (usuarioEncontrado.Senha != body.Senha)
        {
            return Results.BadRequest("Usuario ou senha estão errados!");
        }

        var token = TokenServico.GerarToken(usuarioEncontrado);
        return Results.Ok(new { Token = token });
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensagem = ex.Message });
    }
    catch
    {
        return Results.Problem("Ocorreu um erro desconhecido, tente novamente mais tarde.", 
            "Erro no servidor!",500); 
    }
});

app.Run();