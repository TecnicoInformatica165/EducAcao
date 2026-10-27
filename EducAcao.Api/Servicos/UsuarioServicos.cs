using EducAcao.Api.Dados;
using EducAcao.Core.Entidades;
using EducAcao.Core.Exceptions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace EducAcao.Api.Servicos;

public class UsuarioServicos (DbContexto contexto)
{
    public async Task AdicionaAsync(Usuario modelo)
    {
        await contexto.Usuarios.AddAsync(modelo);
        await contexto.SaveChangesAsync();
       
    }
    
    // Busca o usuário pelo email
    public async Task<Usuario> BuscarPorEmailAsync(string email)
    {
        var usuarioEncontrado = await contexto.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == email);

        if (usuarioEncontrado is null)
            throw new UsuarioNaoEncontradoException("O usuário não existe!");

        return usuarioEncontrado;
    }
}