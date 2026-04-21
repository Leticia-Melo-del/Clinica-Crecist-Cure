using static Clinica_Crescit.Cura.UI.ConsoleHelper;

namespace Clinica_Crescit.Cura.UI
{
    public class ClinicaApp // Classe principal da aplicação, responsável por gerenciar o fluxo geral do programa
    {
        private readonly ICadastroRepository _cadastroRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IAdministradorRepository _administradorRepository;
        private readonly IConsultaRepository _consultaRepository;
        private readonly AutenticacaoService _autenticacaoService;
        private readonly CadastroConsoleService _cadastroConsoleService;
        private readonly UsuarioConsoleService _usuarioConsoleService;

        public ClinicaApp(
            ICadastroRepository cadastroRepository,
            IMedicoRepository medicoRepository,
            IAdministradorRepository administradorRepository,
            IConsultaRepository consultaRepository,
            AutenticacaoService autenticacaoService,
            CadastroConsoleService cadastroConsoleService,
            UsuarioConsoleService usuarioConsoleService)
        {
            _cadastroRepository = cadastroRepository;
            _medicoRepository = medicoRepository;
            _administradorRepository = administradorRepository;
            _consultaRepository = consultaRepository;
            _autenticacaoService = autenticacaoService;
            _cadastroConsoleService = cadastroConsoleService;
            _usuarioConsoleService = usuarioConsoleService;
        }

        // Métodos para exibir menus, ler entradas do usuário, realizar cadastro, login e outras funcionalidades da aplicação
        public void Executar()
        {
            _medicoRepository.GarantirCargaInicial();
            _administradorRepository.GarantirCargaInicial();

            while (true)
            {
                Console.WriteLine("--- Clinica Crescit Cura ---");
                Console.WriteLine("1 - Cadastrar");
                Console.WriteLine("2 - Fazer Login");
                Console.Write("Escolha uma opção: ");

                string opcao = (Console.ReadLine() ?? string.Empty).Trim();
                bool pausarAoFinal = true;
                LimparTela();

                switch (opcao)
                {
                    case "1":
                        RealizarCadastro();
                        break;
                    case "2":
                        SessaoUsuario? sessao = RealizarLogin();

                        if (sessao is not null)
                        {
                            ExibirMenuDaSessao(sessao);
                            pausarAoFinal = false;
                        }
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Digite 1 ou 2.");
                        break;
                }

                if (pausarAoFinal)
                {
                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                    AguardarInteracao();
                    LimparTela();
                }
            }
        }

        private void RealizarCadastro()
        {
            Console.WriteLine("-- Dados do Responsável Legal --");

            string nomeResp = LerTextoObrigatorio("Nome completo: ");
            string cpfResp = LerCpfValido();
            string telefone = LerTelefoneValido();
            string email = _usuarioConsoleService.LerEmailDisponivelParaResponsavel();
            string endereco = LerEnderecoValido();

            Responsavel responsavel = new Responsavel(nomeResp, cpfResp, telefone, email, endereco);

            Console.WriteLine(" Informações da Criança/Adolescente ");

            Paciente paciente = _cadastroConsoleService.LerDadosPaciente(cpfResp);
            responsavel.Pacientes.Add(paciente);

            Console.WriteLine("Aperte qualquer tecla para ver o resumo do cadastro...");
            AguardarInteracao();
            LimparTela();

            responsavel.ExibirResumo();
            foreach (Paciente pac in responsavel.Pacientes)
            {
                pac.ExibirResumo();
            }

            string senha = LerSenhaValida();
            responsavel.CadastrarSenha(senha);

            CadastroRegistro cadastro = CadastroRegistro.CriarDe(responsavel, senha);

            try
            {
                _cadastroRepository.Salvar(cadastro);
                Console.WriteLine("\nCadastro finalizado com sucesso!");
                Console.WriteLine($"Dados salvos em: {_cadastroRepository.Destino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nCadastro concluído, mas houve um erro ao salvar os dados.");
                Console.WriteLine(ex.Message);
            }
        }

        private SessaoUsuario? RealizarLogin()
        {
            Console.WriteLine("-- Login --");

            string email = LerEmailValido();
            Console.Write("Senha: ");
            string senha = Console.ReadLine() ?? string.Empty;

            LoginResultado resultado = _autenticacaoService.Autenticar(email, senha);

            if (!resultado.Sucesso || resultado.Sessao is null)
            {
                Console.WriteLine(resultado.Mensagem);
                return null;
            }

            ExibirResumoAposLogin(resultado.Sessao);
            return resultado.Sessao;
        }

        private static void ExibirResumoAposLogin(SessaoUsuario sessao)
        {
            switch (sessao.Tipo)
            {
                case TipoUsuario.Responsavel when sessao.Responsavel is not null:
                    ExibirResumoResponsavel(sessao.Responsavel);
                    break;
                case TipoUsuario.Medico when sessao.Medico is not null:
                    ExibirResumoMedico(sessao.Medico);
                    break;
                case TipoUsuario.Administrador when sessao.Administrador is not null:
                    ExibirResumoAdministrador(sessao.Administrador);
                    break;
                default:
                    Console.WriteLine("Não foi possível identificar o perfil autenticado.");
                    break;
            }
        }

        private static void ExibirResumoResponsavel(CadastroRegistro cadastro)
        {
            Console.WriteLine($"\nLogin realizado com sucesso. Bem-vindo(a), {cadastro.Responsavel.NomeCompleto}!");
            Console.WriteLine("Perfil: Responsável");
            Console.WriteLine($"E-mail: {cadastro.Responsavel.Email}");
            Console.WriteLine($"Telefone: {cadastro.Responsavel.Telefone}");
            Console.WriteLine($"Pacientes vinculados: {cadastro.Pacientes.Count}");

            foreach (PacienteRegistro paciente in cadastro.Pacientes)
            {
                Console.WriteLine($"\nPaciente: {paciente.NomeCompleto}");
                Console.WriteLine($"CPF: {paciente.Cpf}");
                Console.WriteLine($"Data de nascimento: {paciente.DataNascimento:dd/MM/yyyy}");
                Console.WriteLine($"Gênero: {paciente.Genero}");
            }
        }

        private static void ExibirResumoMedico(Medico medico)
        {
            Console.WriteLine($"\nLogin realizado com sucesso. Bem-vindo(a), {medico.NomeCompleto}!");
            Console.WriteLine("Perfil: Médico");
            Console.WriteLine($"CRM: {medico.Crm}");
            Console.WriteLine($"Especialidade: {medico.Especialidade}");
            Console.WriteLine($"E-mail: {medico.Email}");
        }

        private static void ExibirResumoAdministrador(AdministradorRegistro administrador)
        {
            Console.WriteLine($"\nLogin realizado com sucesso. Bem-vindo(a), {administrador.NomeCompleto}!");
            Console.WriteLine("Perfil: Administrador");
            Console.WriteLine($"E-mail: {administrador.Email}");
        }

        private void ExibirMenuDaSessao(SessaoUsuario sessao)
        {
            switch (sessao.Tipo)
            {
                case TipoUsuario.Responsavel when sessao.Responsavel is not null:
                    ExibirMenuResponsavel(sessao.Responsavel);
                    break;
                case TipoUsuario.Medico when sessao.Medico is not null:
                    ExibirMenuMedico(sessao.Medico);
                    break;
                case TipoUsuario.Administrador when sessao.Administrador is not null:
                    ExibirMenuAdministrador(sessao.Administrador);
                    break;
                default:
                    Console.WriteLine("Sessão inválida.");
                    break;
            }
        }

        private void ExibirMenuResponsavel(CadastroRegistro usuarioLogado)
        {
            MenuResponsavel menu = new MenuResponsavel(
                _cadastroRepository,
                _medicoRepository,
                _consultaRepository,
                _cadastroConsoleService,
                _usuarioConsoleService);

            menu.Exibir(usuarioLogado);
        }

        private void ExibirMenuMedico(Medico medicoLogado)
        {
            MenuMedico menu = new MenuMedico(_consultaRepository, _cadastroRepository);
            menu.Exibir(medicoLogado);
        }

        private void ExibirMenuAdministrador(AdministradorRegistro administradorLogado)
        {
            MenuAdm menu = new MenuAdm(
                _cadastroRepository,
                _medicoRepository,
                _administradorRepository,
                _consultaRepository,
                _usuarioConsoleService);

            menu.ExibirMenuAdministrador(administradorLogado);
        }
    }
}
