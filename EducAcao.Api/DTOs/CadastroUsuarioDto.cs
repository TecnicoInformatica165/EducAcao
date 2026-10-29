namespace EducAcao.Api.DTOs;

public class CadastroUsuarioDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public DateOnly DataNascimento { get; set; }
}