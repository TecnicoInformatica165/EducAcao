using EducAcao.Api.Dados;
using EducAcao.Api.DTOs;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<ApiContexto>
    (stringConexao, ServerVersion.AutoDetect
    (stringConexao));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/status", () =>
{
    return Results.Ok(new
    {
        Estado = "Online",
        Codigo = "1234"
    });
});

app.MapPost("/usuario", (CadastroUsuarioDto requisicao) =>
{
    try
    {
        var novoUsuario = new Usuario(requisicao.Nome, requisicao.Email, requisicao.Senha, requisicao.DataNascimento);
        return Results.Created(string.Empty,
            new
            {
                Usuario = requisicao
            });
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});
app.Run();
