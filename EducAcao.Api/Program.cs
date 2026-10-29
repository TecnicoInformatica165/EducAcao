using EducAcao.Api;
using EducAcao.Api.Dados;
using EducAcao.Api.DTOs;
using EducAcao.Api.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbContexto>(
    stringConexao, 
    ServerVersion.AutoDetect(stringConexao)
);

// Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();
builder.Services.AddScoped<TokenServico>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

// IMPORTANTE: Adicione estes dois se for usar autenticação com Token
// app.UseAuthentication();
// app.UseAuthorization();

// Cria usuario
app.MapPost("/usuario", async (CadastroUsuarioDto body, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var novoUsuario = new Usuario(body.Nome, body.Email, body.Senha, body.DataNascimento);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        return Results.Created($"/usuario/{novoUsuario.Id}", new { Mensagem = "Usuário criado com sucesso!" });
    }
    catch (CoreException ex)    
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});

// Login
app.MapPost("/login", async (
    LoginUsuarioDto body,
    UsuarioServicos usuarioServicos,
    TokenServico tokenServico) =>
{
    try
    {
        var usuarioEncontrado = await usuarioServicos.BuscarPorEmailAsync(body.Email);

        if (usuarioEncontrado.Senha != body.Senha)
            return Results.BadRequest(new { Mensagem = "Usuário ou senha estão errados!" });

        var token = tokenServico.GerarToken(
            usuarioEncontrado.Id,
            usuarioEncontrado.Email,
            usuarioEncontrado.Nome
        );

        return Results.Ok(new { Token = token });
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensagem = ex.Message });
    }
    catch
    {
        return Results.Problem(
            detail: "Ocorreu um erro desconhecido, tente novamente mais tarde.",
            title: "Erro no servidor!",
            statusCode: 500
        );
    }
});
 
// O comando correto em C# é com ponto e parênteses:
app.Run();