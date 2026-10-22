namespace EducAcao.Api.DTOs;

public class CadastroUsuarioDto
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public DateOnly Nascimento { get; set; }
}