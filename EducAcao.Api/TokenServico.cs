using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EducAcao.Api;

public class TokenServico(IConfiguration configuration)
{
    public string GerarToken(int id, string email, string nome)
    {
        var chave = configuration["CHAVE_JWT"]!;
        var emissor = configuration["EMISSOR_JWT"]!;
        var audiencia = configuration["AUDIENCIA_JWT"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nome)
        };

        var chaveSegura = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(chave));

        var credenciais = new SigningCredentials(
            chaveSegura,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}