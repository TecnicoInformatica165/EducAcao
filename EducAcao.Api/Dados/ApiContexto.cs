using EducAcao.Core.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EducAcao.Api.Dados;

public class ApiContexto(DbContextOptions<ApiContexto> opcoesContexto) : DbContext(opcoesContexto)
{
    public DbSet<Usuario>  Usuarios { get; set; }
}