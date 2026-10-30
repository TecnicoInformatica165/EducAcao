using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EducAcao.Api.Servicos;

public class TokenServico
{
    public static string Chave { get; } = Environment.GetEnvironmentVariable("CHAVE_JWT") ?? string.Empty;
    public static string Emissor { get; } = Environment.GetEnvironmentVariable("EMISSOR_JWT")?? string.Empty;
    public static string Audiencia { get; } = Environment.GetEnvironmentVariable("AUDIENCIA_JWT")?? string.Empty;
    
    public string GerarToken(string id, string email, string nome)
    {
        if (string.IsNullOrWhiteSpace(Chave) ||
            string.IsNullOrWhiteSpace(Emissor) ||
            string.IsNullOrWhiteSpace(Audiencia))
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
            Encoding.UTF8.GetBytes(Chave));

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256);

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

