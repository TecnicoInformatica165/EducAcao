using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EducAcao.Api.Servicos;

public class TokenServico
{
    public string GerarToken(string id, string email, string nome)
    {
        var chaveJwt = Environment.GetEnvironmentVariable("CHAVE_JWT");
        var emissor = Environment.GetEnvironmentVariable("EMISSOR_JWT");
        var audiencia = Environment.GetEnvironmentVariable("AUDIENCIA_JWT");

        if (string.IsNullOrWhiteSpace(chaveJwt) ||
            string.IsNullOrWhiteSpace(emissor) ||
            string.IsNullOrWhiteSpace(audiencia))
        {
            throw new InvalidOperationException(
                "Configure as variáveis de ambiente do JWT.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nome)
        };

        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(chaveJwt));

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    }

