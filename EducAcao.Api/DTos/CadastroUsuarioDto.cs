namespace EducAcao.Api.DTos;

public class CadastroUsuarioDto
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public DateOnly DataNascimento { get; set; }
}