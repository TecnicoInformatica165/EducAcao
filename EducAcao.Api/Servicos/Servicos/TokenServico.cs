using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EducAcao.Core.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace EducAcao.Api.Servicos.Servicos
{
    public class TokenServico
    {
        public static string GerarToken(Usuario usuario)
        {
            var chave = Environment.GetEnvironmentVariable("CHAVE_JWT");
            var emissor = Environment.GetEnvironmentVariable("EMISSOR_JWT");
            var audiencia = Environment.GetEnvironmentVariable("AUDIENCIA_JWT");

            if (string.IsNullOrWhiteSpace(chave) ||
                string.IsNullOrWhiteSpace(emissor) ||
                string.IsNullOrWhiteSpace(audiencia))
            {
                throw new InvalidOperationException(
                    "As variáveis de ambiente do JWT não foram configuradas.");
            }

            var chaveBytes = Encoding.UTF8.GetBytes(chave);
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
                issuer: emissor,
                audience: audiencia,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}