using EducAcao.Core.Exceptions;

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
        if (idadeMinima < 6)
            throw new CoreException("A idade mínima não pode ser inferior a 6 anos!");
        
        IdadeMinima = idadeMinima;
    }

    private void ValidarVagasTotais(int vagasTotais)
    {
        if (vagasTotais < 1)
            throw new CoreException("O número de vagas não pode ser inferior a 1!");
        
        VagasTotais = vagasTotais;
    }

    private void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new CoreException("O título da atividade não pode ficar em branco ou conter apenas espaços!");

        titulo = titulo.Trim();

        if (titulo.Length < TamanhoMinimoCaracteresTitulo)
            throw new CoreException($"O título da atividade deve conter no mínimo {TamanhoMinimoCaracteresTitulo} caracteres!");
        
        Titulo = titulo;
    }

    private const int TamanhoMinimoCaracteresTitulo = 10;
}