namespace Clinica_Crescit.Cura
{
    public class LoginResultado // Representa o resultado de uma tentativa de login
    {
        private LoginResultado(bool sucesso, string mensagem, SessaoUsuario? sessao)// Construtor privado para garantir que o resultado seja criado apenas por meio dos métodos de fábrica
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Sessao = sessao;
        }

        public bool Sucesso { get; }
        public string Mensagem { get; }
        public SessaoUsuario? Sessao { get; }

        public static LoginResultado Falha(string mensagem)  // Cria um resultado de login com falha e uma mensagem de erro
        {
            return new LoginResultado(false, mensagem, null);
        }

        public static LoginResultado SucessoCom(SessaoUsuario sessao) // Cria um resultado de login bem-sucedido com base na sessão do usuário
        {
            string perfil = sessao.Tipo switch
            {
                TipoUsuario.Responsavel => "Responsável",
                TipoUsuario.Medico => "Médico",
                TipoUsuario.Administrador => "Administrador",
                _ => "Usuário"
            };

            return new LoginResultado(true, $"Login realizado com sucesso como {perfil}.", sessao);
        }
    }
}
