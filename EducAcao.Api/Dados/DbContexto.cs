using EducAcao.Core.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EducAcao.Api.Dados;

public class DbContexto(DbContextOptions<DbContexto> opcoesContexto) : DbContext(opcoesContexto)
{
    public DbSet<Usuario>  Usuarios { get; set; }
}