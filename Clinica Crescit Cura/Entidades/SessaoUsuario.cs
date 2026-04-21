using System;
namespace Clinica_Crescit.Cura
{
    public enum TipoUsuario // Enumeração para representar os tipos de usuários do sistema: Responsável, Médico e Administrador
    {
        Responsavel,
        Medico,
        Administrador
    }

    // Representa a sessão de um usuário autenticado, contendo informações sobre o tipo de usuário e seus dados relevantes
    public class SessaoUsuario 
    {
        private SessaoUsuario(
            TipoUsuario tipo,
            string nomeExibicao,
            CadastroRegistro? responsavel = null,
            Medico? medico = null,
            AdministradorRegistro? administrador = null)
        {
            Tipo = tipo;
            NomeExibicao = nomeExibicao;
            Responsavel = responsavel;
            Medico = medico;
            Administrador = administrador;
        }

        // Representa a sessão de um usuário autenticado, contendo informações sobre o tipo de usuário e seus dados relevantes
        public TipoUsuario Tipo { get; }
        public string NomeExibicao { get; }
        public CadastroRegistro? Responsavel { get; }
        public Medico? Medico { get; }
        public AdministradorRegistro? Administrador { get; }

        // Métodos de fábrica para criar sessões de diferentes tipos de usuários, garantindo que as informações relevantes sejam associadas corretamente
        public static SessaoUsuario ParaResponsavel(CadastroRegistro cadastro)
        {
            return new SessaoUsuario(
                TipoUsuario.Responsavel,
                cadastro.Responsavel.NomeCompleto,
                responsavel: cadastro);
        }

        public static SessaoUsuario ParaMedico(Medico medico)
        {
            return new SessaoUsuario(
                TipoUsuario.Medico,
                medico.NomeCompleto,
                medico: medico);
        }

        public static SessaoUsuario ParaAdministrador(AdministradorRegistro administrador)
        {
            return new SessaoUsuario(
                TipoUsuario.Administrador,
                administrador.NomeCompleto,
                administrador: administrador);
        }
    }
}
