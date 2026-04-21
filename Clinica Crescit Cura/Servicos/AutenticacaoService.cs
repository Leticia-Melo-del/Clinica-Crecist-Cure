namespace Clinica_Crescit.Cura
{
    public class AutenticacaoService
    {
        private readonly ICadastroRepository _cadastroRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IAdministradorRepository _administradorRepository;

        // Construtor para injeção de dependências
        public AutenticacaoService(
            ICadastroRepository cadastroRepository,
            IMedicoRepository medicoRepository,
            IAdministradorRepository administradorRepository)
        {
            _cadastroRepository = cadastroRepository;
            _medicoRepository = medicoRepository;
            _administradorRepository = administradorRepository;
        }

        // Método para autenticar um usuário com base no e-mail e senha fornecidos
        public LoginResultado Autenticar(string email, string senha)
        {
            AdministradorRegistro? administrador = _administradorRepository.ObterPorEmail(email);

            if (administrador is not null
                && SenhaService.VerificarSenha(senha, administrador.SenhaHash))
            {
                return LoginResultado.SucessoCom(SessaoUsuario.ParaAdministrador(administrador));
            }

            Medico? medico = _medicoRepository.ObterPorEmail(email);

            if (medico is not null && SenhaService.VerificarSenha(senha, medico.SenhaHash))
            {
                return LoginResultado.SucessoCom(SessaoUsuario.ParaMedico(medico));
            }

            CadastroRegistro? cadastro = _cadastroRepository.ObterCadastroPorEmail(email);

            if (cadastro is not null
                && SenhaService.VerificarSenha(senha, cadastro.Responsavel.SenhaHash))
            {
                return LoginResultado.SucessoCom(SessaoUsuario.ParaResponsavel(cadastro));
            }

            return LoginResultado.Falha("E-mail ou senha inválidos.");
        }
    }
}
