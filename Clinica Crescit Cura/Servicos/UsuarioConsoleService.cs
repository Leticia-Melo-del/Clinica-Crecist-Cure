namespace Clinica_Crescit.Cura
{
    public class UsuarioConsoleService
    {
        // Repositórios para acesso aos dados de cadastro, médicos e administradores, utilizados para validação de informações como CPF, CRM e e-mail durante a criação e edição de usuários
        private readonly ICadastroRepository _cadastroRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IAdministradorRepository _administradorRepository;

        // Construtor para injeção de dependências dos repositórios necessários para as operações de leitura e validação de dados de usuários
        public UsuarioConsoleService(
            ICadastroRepository cadastroRepository,
            IMedicoRepository medicoRepository,
            IAdministradorRepository administradorRepository)
        {
            _cadastroRepository = cadastroRepository;
            _medicoRepository = medicoRepository;
            _administradorRepository = administradorRepository;
        }

        // Métodos para ler informações específicas de usuários, garantindo que os dados sejam válidos e não entrem em conflito com registros existentes, como CPF de pacientes, CPF e CRM de médicos, e e-mails para responsáveis e médicos, com mensagens de feedback para o usuário em caso de erros ou conflitos
        public string LerCpfMedicoDisponivel()
        {
            while (true)
            {
                string cpf = UI.ConsoleHelper.LerCpfValido();

                if (!_medicoRepository.CpfJaCadastrado(cpf))
                {
                    return cpf;
                }

                Console.WriteLine("Já existe um médico cadastrado com esse CPF.");
            }
        }

        public string LerEmailDisponivelParaResponsavel()
        {
            return LerEmailDisponivelParaNovoUsuario();
        }

        public string LerEmailDisponivelParaMedico()
        {
            return LerEmailDisponivelParaNovoUsuario();
        }

        public string LerCrmDisponivel()
        {
            while (true)
            {
                Console.Write("CRM: ");
                string crm = (Console.ReadLine() ?? string.Empty).Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(crm))
                {
                    Console.WriteLine("O CRM é obrigatório.");
                    continue;
                }

                if (_medicoRepository.CrmJaCadastrado(crm))
                {
                    Console.WriteLine("Já existe um médico cadastrado com esse CRM.");
                    continue;
                }

                return crm;
            }
        }

        public string LerEmailComValorAtual(CadastroRegistro cadastro)
        {
            while (true)
            {
                Console.Write($"E-mail ({cadastro.Responsavel.Email}): ");
                string entrada = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    return cadastro.Responsavel.Email;
                }

                if (!UI.ConsoleHelper.EhEmailValido(entrada))
                {
                    Console.WriteLine("E-mail inválido. Digite novamente.");
                    continue;
                }

                bool emailEmUsoPorOutroResponsavel =
                    _cadastroRepository.EmailJaCadastradoPorOutroCadastro(entrada, cadastro.Id);

                bool emailEmUsoPorOutroPerfil =
                    _medicoRepository.EmailJaCadastrado(entrada)
                    || _administradorRepository.EmailJaCadastrado(entrada);

                if (emailEmUsoPorOutroResponsavel || emailEmUsoPorOutroPerfil)
                {
                    Console.WriteLine("Já existe um cadastro com esse e-mail.");
                    continue;
                }

                return entrada;
            }
        }

        public bool EmailDisponivelParaNovoUsuario(string email)
        {
            return !_cadastroRepository.EmailJaCadastrado(email)
                && !_medicoRepository.EmailJaCadastrado(email)
                && !_administradorRepository.EmailJaCadastrado(email);
        }

        private string LerEmailDisponivelParaNovoUsuario()
        {
            while (true)
            {
                string email = UI.ConsoleHelper.LerEmailValido();

                if (EmailDisponivelParaNovoUsuario(email))
                {
                    return email;
                }

                Console.WriteLine("Já existe um cadastro com esse e-mail.");
            }
        }
    }
}
