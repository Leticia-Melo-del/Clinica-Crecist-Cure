namespace Clinica_Crescit.Cura.UI
{
    public class MenuMedico
    {

        // Menu específico para médicos, com opções de visualização de agenda, atendimento de pacientes e visualização de dados pessoais
        private readonly IConsultaRepository _consultaRepo;
        private readonly ICadastroRepository _cadastroRepo;
        public MenuMedico(IConsultaRepository consultaRepo, ICadastroRepository cadastroRepo)
        {
            _consultaRepo = consultaRepo;
            _cadastroRepo = cadastroRepo;
        }

        public void Exibir(Medico medicoLogado)
        {
            while (true)
            {
                ConsoleHelper.LimparTela();
                Console.WriteLine("--- Área do Médico ---");
                Console.WriteLine($"Dr(a). {medicoLogado.NomeCompleto} | {medicoLogado.Especialidade}");
                Console.WriteLine("1 - Minha Agenda Profissional");
                Console.WriteLine("2 - Preencher Prontuários");
                Console.WriteLine("3 - Meus Dados");
                Console.WriteLine("0 - Sair da Conta");

                string opcao = Console.ReadLine()?.Trim() ?? "";

                switch (opcao)
                {
                    case "1":
                        ExibirAgenda(medicoLogado);
                        break;
                    case "2":
                        AtenderPaciente(medicoLogado);
                        break;
                    case "3":
                        ExibirDados(medicoLogado);
                        break;
                    case "0":
                        Console.WriteLine("Logout realizado.");
                        ConsoleHelper.AguardarInteracao();
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        ConsoleHelper.AguardarInteracao();
                        break;
                }
            }
        }

        private void ExibirAgenda(Medico medicoLogado)
        {
            while (true) // Mantém o médico nesta tela até ele escolher "0"
            {
                ConsoleHelper.LimparTela();
                Console.WriteLine("-- Minha Agenda Profissional --");

                var consultas = _consultaRepo.ObterPorMedico(medicoLogado.Id)
                                     .OrderBy(c => c.DataHora).ToList();

                if (consultas.Count == 0)
                    {
                        Console.WriteLine("Nenhuma consulta agendada.");
                        ConsoleHelper.AguardarInteracao();
                        return; 
                    }

                // Exibe a lista com números para seleção
                for (int i = 0; i < consultas.Count; i++)
                {
                    var consulta = consultas[i];
                    var paciente = ObterPacientePorId(consulta.PacienteId);
                    string nomePaciente = paciente != null ? paciente.NomeCompleto : "Desconhecido";
            
                    Console.WriteLine($"{i + 1} - [{consulta.DataHora:dd/MM/yyyy HH:mm}] Paciente: {nomePaciente} | Status: {consulta.Status}");
                }

                Console.WriteLine("\n0 - Voltar");
                Console.Write("\nSelecione o número da consulta para ver os detalhes do paciente: ");

                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index == 0) return; // Sai do loop e volta ao menu principal

                    if (index > 0 && index <= consultas.Count)
                    {
                        // Chama o novo método passando a consulta escolhida
                        ExibirDetalhesConsulta(consultas[index - 1]);
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida!");
                        ConsoleHelper.AguardarInteracao();
                    }
                }
            }
        }

        private void AtenderPaciente(Medico medicoLogado)
        {
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Prontuários Pendentes --");

            // Busca apenas consultas que ainda não foram realizadas ou canceladas
            var consultasPendentes = _consultaRepo.ObterPorMedico(medicoLogado.Id)
                .Where(c => c.Status == "Agendada" || c.Status == "Confirmada")
                .OrderBy(c => c.DataHora).ToList();

            if (consultasPendentes.Count == 0)
            {
                Console.WriteLine("Você não possui prontuários pendentes para atender no momento.");
                ConsoleHelper.AguardarInteracao();
                return;
            }

            for (int i = 0; i < consultasPendentes.Count; i++)
            {
                var c = consultasPendentes[i];
                string nomePac = ObterNomePaciente(c.PacienteId);
                Console.WriteLine($"{i + 1} - [{c.DataHora:dd/MM/yyyy HH:mm}] Paciente: {nomePac}");
            }

            Console.WriteLine("\n0 - Voltar");
            Console.Write("Selecione o prontuário que deseja realizar: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= consultasPendentes.Count)
            {
                var consulta = consultasPendentes[index - 1];
                string nomePaciente = ObterNomePaciente(consulta.PacienteId);

                ConsoleHelper.LimparTela();
                Console.WriteLine($"--- Atendimento: {nomePaciente} ---");
                Console.WriteLine("Motivo / Observações do Responsável:");
                Console.WriteLine(string.IsNullOrWhiteSpace(consulta.Observacoes) ? "Nenhuma observação informada." : consulta.Observacoes);
                
                Console.WriteLine("\n------------------------------------------------");
                Console.WriteLine("Escreva o Diagnóstico e as Recomendações:");
                string prontuario = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(prontuario))
                {
                    consulta.Diagnostico = prontuario;
                    consulta.Status = "Realizada"; // Muda o status!
                    
                    _consultaRepo.Atualizar(consulta); // Salva no JSON
                    
                    Console.WriteLine("\nConsulta finalizada! Prontuário salvo e disponível para o responsável.");
                }
                else
                {
                    Console.WriteLine("\nOperação cancelada. O prontuário não pode ficar vazio.");
                }
                ConsoleHelper.AguardarInteracao();
            }
        }

        private void ExibirDados(Medico medicoLogado)
        {
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Meus Dados --");
            medicoLogado.ExibirResumo();
            Console.WriteLine($"CPF: {medicoLogado.CPF}");
            ConsoleHelper.AguardarInteracao();
        }

        private string ObterNomePaciente(int pacienteId)
        {
            var todosCadastros = _cadastroRepo.ObterTodos(); // Se precisar, crie esse método no JsonCadastroRepository
            foreach (var cadastro in todosCadastros)
            {
                var paciente = cadastro.Pacientes.FirstOrDefault(p => p.Id == pacienteId);
                if (paciente != null)
                {
                    return paciente.NomeCompleto;
                }
            }
            return "Desconhecido";
        }
    
        private void ExibirDetalhesConsulta(Consulta consulta)
        {
            ConsoleHelper.LimparTela();
            // CLÁUSULA DE GUARDA: Bloqueia o acesso se estiver cancelada
            if (consulta.Status.Equals("Cancelada", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("--- Acesso Bloqueado ---");
                Console.WriteLine($"A consulta de {consulta.DataHora:dd/MM/yyyy ás HH:mm} foi CANCELADA.");
                Console.WriteLine("Por motivos de privacidade, os dados do paciente não estão disponíveis.");
                ConsoleHelper.AguardarInteracao();
                return; // O 'return' faz com que o método pare aqui e volte para a agenda
            }

            var paciente = ObterPacientePorId(consulta.PacienteId);

            if (paciente == null)
            {
                Console.WriteLine("Erro: Dados do paciente não encontrados no sistema.");
                ConsoleHelper.AguardarInteracao();
                return;
            }

            Console.WriteLine("--- Detalhes do Atendimento ---");
            Console.WriteLine($"Data/Hora: {consulta.DataHora:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Status: {consulta.Status}");
    
            Console.WriteLine("\n-- Perfil do Paciente --");
            Console.WriteLine($"Nome: {paciente.NomeCompleto}");
            // Utiliza a nova propriedade Idade encapsulada na entidade Paciente
            Console.WriteLine($"Idade: {paciente.Idade} anos (Nasc: {paciente.DataNascimento:dd/MM/yyyy})");
            Console.WriteLine($"Gênero: {paciente.Genero}");

            // Mostra dados adicionais apenas se o responsável os tiver preenchido
            if (!string.IsNullOrWhiteSpace(paciente.TipoSanguineo)) 
            {
                Console.WriteLine($"Tipo Sanguíneo: {paciente.TipoSanguineo}");
            }
            // Mostra dados adicionais apenas se o responsável os tiver preenchido
            if (!string.IsNullOrWhiteSpace(paciente.Alergias)) 
            {
                Console.WriteLine($"Alergias: {paciente.Alergias}");
            }
            if (!string.IsNullOrWhiteSpace(paciente.DoencasPreExistentes)) 
            {
                Console.WriteLine($"Condições Pré-existentes: {paciente.DoencasPreExistentes}");
            }
            if (!string.IsNullOrWhiteSpace(paciente.Altura.ToString()) && !string.IsNullOrWhiteSpace(paciente.Peso.ToString())) 
            {
                Console.WriteLine($"Peso: {paciente.Peso} kg | Altura: {paciente.Altura} m");
            }

            Console.WriteLine("\n-- Observações Adicionais (Responsável) --");
            Console.WriteLine(string.IsNullOrWhiteSpace(consulta.Observacoes) ? "Nenhuma observação ou motivo relatado no agendamento." : consulta.Observacoes);

            Console.WriteLine("\n------------------------------------------------");
            ConsoleHelper.AguardarInteracao();
        }
        
        private PacienteRegistro? ObterPacientePorId(int pacienteId)
        {
            var todosCadastros = _cadastroRepo.ObterTodos();
            return todosCadastros
            .SelectMany(c => c.Pacientes)
            .FirstOrDefault(p => p.Id == pacienteId);
        }
    
    }
}