namespace EducAcao.Api.Servicos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenServico
{
    public string GerarToken(string idUsuario, string emailUsuario, string nomeUsuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("CHAVE_JWT"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
           //claims da manu
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "987"),
                new Claim(ClaimTypes.Email, "maria@email.com"),
                new Claim(ClaimTypes.Name, "Maria Souza"),
            }),
            
            //tempo
            Expires = DateTime.UtcNow.AddHours(2),
            
            // Emissor e Audiência q veio das configuraçoes
            Issuer = Environment.GetEnvironmentVariable("EMISSOR_JWT"),
            Audience = Environment.GetEnvironmentVariable("AUDIENCIA_JWT"),
            
            // Credenciais de Assinatura (Chave Secreta + Algoritmo HmacSha256)
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        // 3. Cria e escreve o token em formato de string
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}