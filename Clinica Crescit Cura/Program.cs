using Clinica_Crescit.Cura.UI;

namespace Clinica_Crescit.Cura.UI
{
                            // Ponto de entrada da aplicação, onde as dependências são configuradas e a aplicação é iniciada
    internal class Program
    {
        static void Main(string[] args)
        {
            ICadastroRepository cadastroRepository = new JsonCadastroRepository();
            IMedicoRepository medicoRepository = new JsonMedicoRepository();
            IAdministradorRepository administradorRepository = new JsonAdministradorRepository();
            IConsultaRepository consultaRepository = new JsonConsultaRepository();

            AutenticacaoService autenticacaoService = new AutenticacaoService(
                cadastroRepository,
                medicoRepository,
                administradorRepository);

            CadastroConsoleService cadastroConsoleService = new CadastroConsoleService();
            UsuarioConsoleService usuarioConsoleService = new UsuarioConsoleService(
                cadastroRepository,
                medicoRepository,
                administradorRepository);

            ClinicaApp app = new ClinicaApp(
                cadastroRepository,
                medicoRepository,
                administradorRepository,
                consultaRepository,
                autenticacaoService,
                cadastroConsoleService,
                usuarioConsoleService);

            app.Executar();
        }
    }
}
