using EducAcao.Api.Dados;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;

namespace EducAcao.Api.Servicos;

public class UsuarioServicos (DbContexto contexto)
{
    public async Task AdicionaAsync(Usuario modelo)
    {
        //adiciona usuario no banco de dados
        await contexto.Usuarios.AddAsync(modelo);
        await contexto.SaveChangesAsync();
    }
    
    //busca usuario pelo email
    public async Task<Usuario> BuscarPorEmailAsync(string email)
    {
        var usuarioEncontrado = contexto.Usuarios.FirstOrDefault(Usuario => Usuario.Email == email);

        if (usuarioEncontrado is null)
            throw new UsuarioNaoEncontradoException("Usuário não existe!");

        return usuarioEncontrado;
    }
}