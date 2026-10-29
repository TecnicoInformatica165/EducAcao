using System.Security.Claims;
using System.Text;
using EducAcao.Api.Dados;
using EducAcao.Api.DTOs;
using EducAcao.Api.Servicos;
using EducAcao.Api.Servicos.Servicos;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TokenServico>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var stringConexao = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddMySql<DbContexto>
    (stringConexao, ServerVersion.AutoDetect
    (stringConexao));

if (string.IsNullOrWhiteSpace(TokenServico.Chave) ||
    string.IsNullOrWhiteSpace(TokenServico.Emissor) ||
    string.IsNullOrWhiteSpace(TokenServico.Audiencia))
{
    throw new InvalidOperationException(
        "As variáveis de ambiente do JWT não foram configuradas.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(TokenServico.Chave);
    
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer =  TokenServico.Emissor,
        ValidateAudience = true,
        ValidAudience =  TokenServico.Audiencia,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
    
//Cadastro de Servicos Customizados
builder.Services.AddScoped<UsuarioServicos>();
builder.Services.AddAuthorization(options =>
{
    // Exemplo: Criando uma regra/permissão chamada "ApenasAdmin"
    // Ela exige que o usuário tenha um carimbo (Claim) do tipo Role com o valor "Admin"
    options.AddPolicy("ApenasAdmin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
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
        
        var token = TokenServico.GerarToken(usuarioEncontrado);
        return Results.Ok(new { Token = token });
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

//validar token
app.MapGet("/perfil", (ClaimsPrincipal Usuario) =>
    {

        // 2. Buscar o ID do usuário (NameIdentifier)
        string usuarioId = Usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 3. Buscar o e-mail do usuário
        string email = Usuario.FindFirst(ClaimTypes.Email)?.Value;

        // 4. Buscar uma Claim personalizada (ex: "Idade")
        string idade = Usuario.FindFirst("Idade")?.Value;

        // 5. Verificar se o usuário possui uma Role específica
        bool ehAdmin = Usuario.IsInRole("Admin");

        return Results.Ok(new { usuarioId, email, ehAdmin });
    })
    .RequireAuthorization();
app.Run();


