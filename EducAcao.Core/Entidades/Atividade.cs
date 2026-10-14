namespace EducAcao.Core.Entidades;

public class Atividade
{
    public int Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public int VagasTotais { get; private set; }
    public int IdadeMinima { get; private set; }
    public Usuario Provedor { get; private set; } = null!;
    public Usuario Admin { get; private set; } = null!;

    protected Atividade()
    {
        
    }
    
    public Atividade(string titulo, int vagasTotais, int idadeMinima, Usuario provedor, Usuario admin)
    {
        ValidarTitulo(titulo);
        ValidarVagasTotais(vagasTotais);
        ValidarIdadeMinima(idadeMinima);
        Provedor = provedor;
        Admin = admin;
    }

    private void ValidarIdadeMinima(int idadeMinima)
    {
        IdadeMinima = idadeMinima;
    }

    private void ValidarVagasTotais(int vagasTotais)
    {
        VagasTotais = vagasTotais;
    }

    private void ValidarTitulo(string titulo)
    {
        Titulo = titulo;
    }
}