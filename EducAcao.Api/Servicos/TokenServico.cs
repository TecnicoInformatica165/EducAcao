using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EducAcao.Core.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Educacao.Api.Servicos
{
    public class TokenServico
    {
        public static string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var chave = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("CHAVE_JWT"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Name, usuario.Nome)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = Environment.GetEnvironmentVariable("EMISSOR_JWT"),
                Audience = Environment.GetEnvironmentVariable("AUDIENCIA_JWT"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chave), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}