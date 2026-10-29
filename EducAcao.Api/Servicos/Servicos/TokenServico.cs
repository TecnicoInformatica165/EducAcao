using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EducAcao.Core.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace EducAcao.Api.Servicos.Servicos;

public class TokenServico
{
    public static string Chave { get; } = Environment.GetEnvironmentVariable("CHAVE_JWT") ?? string.Empty;
    public static string Emissor { get; } = Environment.GetEnvironmentVariable("EMISSOR_JWT") ?? string.Empty;
    public static string Audiencia { get; } = Environment.GetEnvironmentVariable("AUDIENCIA_JWT") ?? string.Empty;

    public static string GerarToken(Usuario usuario)
    {
        var chaveBytes = Encoding.UTF8.GetBytes(Chave);
        var chaveSegura = new SymmetricSecurityKey(chaveBytes);

        var credenciais = new SigningCredentials(
            chaveSegura,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
        };

        var token = new JwtSecurityToken(
            issuer: Emissor,
            audience: Audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}