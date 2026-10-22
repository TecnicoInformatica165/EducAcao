using EducAcao.Api.DTOs;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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
        Status = "online"
        

    });
        });

app.MapPost("/usuario", (CadastroUsuarioDto requisicao) =>
{
    try
    {
        var novoUsuario = new Usuario(requisicao.Nome, email: requisicao.Email, requisicao.Senha,
            requisicao.DataNascimento);
        return Results.Created(string.Empty,
            new
            {
                Usuario = requisicao
            });
    } catch (CoreException ex)
    {
     return  Results.BadRequest(new { message = ex.Message });
    }
});
app.Run();
   