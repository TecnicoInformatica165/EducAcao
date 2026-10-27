using EducAcao.Api.Dados;
using EducAcao.Api.DTO;
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

// Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();

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
        Status = "Online",
    });
});

app.MapPost("/usuario", async (CadastroUsuarioDto body, UsuarioServicos  usuarioServicos) =>
{
    try
    {
        var novoUsuario = new Usuario(body.Nome, body.Email, body.Senha, body.Nascimento);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        return Results.Created();
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});
    

// Rota de Login
    app.MapPost("/Login", async (LoginUsuarioDto body, UsuarioServicos usuarioServicos) =>
    {
        try
        {
            // Toda a trtativa
            var usuarioEncontrado = await usuarioServicos.BuscarPorEmailAsync(body.Email);

            if (usuarioEncontrado.Senha != body.Senha)
                return Results.BadRequest("usuario ou senha estão errados!");

            return Results.Ok();
        }

        catch (UsuarioNaoEncontradoException ex)
        {
            return Results.BadRequest(new { Mensagem = ex.Message });
        }
        catch
        {
            return Results.Problem("Ocorreu um erro desconhecido, tente novamente mais tarde.", "Erro no servidor!",
                500);
        }
    });

    app.Run();





