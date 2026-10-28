using EducAcao.Api.Dados;
using EducAcao.Api.DTO;
using EducAcao.Api.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbiContexto>(stringConexao, ServerVersion.AutoDetect(stringConexao));


//cadastro de servicos costumizados
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

//rota de criacao usuario
app.MapPost("/usuario", async (CadastroUsuarioDto requisicao, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var novoUsuario = new Usuario(requisicao.Nome, requisicao.Email, requisicao.Senha, requisicao.Nascimento);
        await usuarioServicos.AdicionaAsync(novoUsuario);
        return Results.Created();
    }
    catch (CoreException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message });
    }
});


//rota de login
app.MapPost("/login", async (LoginUsuarioDto body, UsuarioServicos usuarioServicos) =>
{
    try
    {
        var usuarioEncontrado = await usuarioServicos
            .BuscarPorEmailAsync(body.Email);

        if (usuarioEncontrado.Senha != body.Senha)

            return Results.BadRequest("Usuario ou senha estão incorretos!");

        return Results.Ok();
    }
    catch (UsuarioNaoEncontradoException ex)
    {
        return Results.NotFound(new { Mensgem = ex.Message });
    }
    catch
    {
        return Results.Problem("Ocorreu um erro desconhecido! Tente novamente mais tarde.",
            "Ocorreu um erro no servidor.", 500);
    }
});

var builder = WebApplication.CreateBuilder(args);

// 1. Ler as configurações diretamente das Variáveis de Ambiente
var secretKey = Environment.GetEnvironmentVariable("CHAVE_JWT") 
                ?? throw new InvalidOperationException("A variável de ambiente 'CHAVE_JWT' não foi encontrada.");
var issuer = Environment.GetEnvironmentVariable("EMISSOR_JWT") ?? "MinhaApi";
var audience = Environment.GetEnvironmentVariable("AUDIENCIA_JWT") ?? "MinhaApiClientes";

var key = Encoding.UTF8.GetBytes(secretKey);

// 2. Adicionar serviços de Autenticação e JWT Bearer
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// 3. Ordem dos middlewares
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();  