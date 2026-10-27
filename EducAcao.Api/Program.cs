using EducAcao.Api.Dados;
using EducAcao.Api.DTOs;
using EducAcao.Api.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbContexto>
    (stringConexao, ServerVersion.AutoDetect
    (stringConexao));

//Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//cria usuario
app.MapPost("/usuario", async (CadastroUsuarioDto body,
    UsuarioServicos usuarioServicos) =>
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
app.MapPost("/login", async (LoginUsuarioDto body, UsuarioServicos UsuarioServicos) =>
{
    try
    {
        var usuarioEncontrado = await UsuarioServicos.BuscarPorEmailAsync(body.Email);
        if (usuarioEncontrado.Senha != body.Senha) return Results.BadRequest("Usuario ou senha estão errados!");
        return Results.Ok();
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensagem = ex.Message });
    }
    catch 
    {
        return Results.Problem("Ocorreu um erro desconhecido, tente novamete mais tarde.", "Erro no servidor!", 500);
    }
});
app.Run();


