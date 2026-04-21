using System;
using System.Collections.Generic;


namespace Clinica_Crescit.Cura.UI
{

    // Menu específico para administradores, com opções de gerenciamento de usuários, consultas, pagamentos e corpo clínico
    public class MenuAdm
    {
        private readonly ICadastroRepository cadastroRepository;
        private readonly IMedicoRepository medicoRepository;
        private readonly IAdministradorRepository administradorRepository;
        private readonly IConsultaRepository consultaRepository;
        private readonly UsuarioConsoleService usuarioConsoleService;
    

        public MenuAdm(
                ICadastroRepository cadastroRepository,
                IMedicoRepository medicoRepository,
                IAdministradorRepository administradorRepository,
                IConsultaRepository consultaRepository,
                UsuarioConsoleService usuarioConsoleService)
            
            {
                this.cadastroRepository = cadastroRepository;
                this.medicoRepository = medicoRepository;
                this.administradorRepository = administradorRepository;
                this.consultaRepository = consultaRepository;
                this.usuarioConsoleService = usuarioConsoleService;
            }
       
        public void ExibirMenuAdministrador(AdministradorRegistro administradorLogado)
        {
           while (true)
            {
                Console.WriteLine("\n--- Área Administrativa ---");
                Console.WriteLine("1 - Usuários Cadastrados");
                Console.WriteLine("2 - Consultas Agendadas");
                Console.WriteLine("3 - Gerenciar Pagamentos");
                Console.WriteLine("4 - Corpo Clínico");
                Console.WriteLine("5 - Cadastrar Médico");
                Console.WriteLine("0 - Sair da conta");
                Console.Write("Escolha uma opção: ");
            
                string opcao = (Console.ReadLine() ?? string.Empty).Trim();
                ConsoleHelper.LimparTela();

                switch (opcao)
                {
                    case "1":
                        ExibirUsuariosCadastrados();
                        break;
                    case "2":
                        ExibirGerenciamentoConsultas();
                        break;
                    case "3":
                        ExibirGerenciamentoPagamentos();
                        break;
                    case "4":
                        ExibirMedicos();
                        break;
                    case "5":
                        CadastrarMedicoPeloAdministrador();
                        break;
                    case "0":
                        Console.WriteLine("Logout realizado com sucesso.");
                        ConsoleHelper.AguardarInteracao();
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Digite um número de 0 a 5.");
                        break;
                }

                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                ConsoleHelper.AguardarInteracao();
            }
        }

        private void ExibirUsuariosCadastrados()
        {
            Console.WriteLine("-- Usuários Cadastrados --\n");
            
            // Obtém todos os cadastros através do repositório
            List<CadastroRegistro> cadastros = cadastroRepository.ObterTodos();

            if (cadastros == null || cadastros.Count == 0)
            {
                Console.WriteLine("Nenhum usuário cadastrado no sistema.");
                return;
            }

            Console.WriteLine($"Total de usuários cadastrados: {cadastros.Count}\n");

            foreach (var cadastro in cadastros)
            {
                // Verifica se a propriedade Responsavel existe na sua classe CadastroRegistro
                // Caso o nome da propriedade seja diferente, ajuste aqui.
                Console.WriteLine($"Usuário: {cadastro.Responsavel?.NomeCompleto ?? "Nome não informado"} | CPF: {cadastro.Responsavel?.Cpf}");
                Console.WriteLine($"E-mail: {cadastro.Responsavel?.Email}");
                
                // Exibindo os pacientes vinculados (depende da estrutura de CadastroRegistro ter uma lista de pacientes)
                if (cadastro.Pacientes != null && cadastro.Pacientes.Count > 0)
                {
                    Console.WriteLine("Pacientes Vinculados:");
                    foreach (var paciente in cadastro.Pacientes)
                    {
                        Console.WriteLine($"- {paciente.NomeCompleto} | Data de Nasc.: {paciente.DataNascimento:dd/MM/yyyy}");
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum paciente vinculado a este responsável.");
                }
                Console.WriteLine(new string('-', 40));
            }
        }

        private void ExibirGerenciamentoConsultas()
        {
            Console.WriteLine("-- Gerenciamento de Consultas --\n");
            Console.WriteLine("1 - Listar todas as consultas agendadas");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha uma opção: ");
            
            string opcao = (Console.ReadLine() ?? string.Empty).Trim();
            
            switch(opcao)
            {
                case "1":
                   ExibirListaConsultas();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            ConsoleHelper.AguardarInteracao();
            ConsoleHelper.LimparTela();
        
        }

        private void ExibirListaConsultas()
        {
            // Obtém a lista de consultas do repositório
            var consultas = consultaRepository.ObterTodas();

            Console.WriteLine("-- Lista de Consultas Agendadas --\n");

            if (consultas == null || !consultas.Any())
            {
                Console.WriteLine("Não há consultas agendadas no momento.");
                return;
            }

            Console.WriteLine($"Total de agendamentos: {consultas.Count}\n");
    
            // Cabeçalho da tabela para melhor organização visual
            Console.WriteLine("{0,-18} | {1,-18} | {2,-12} | {3,-8} | {4,-10}", "Paciente", "Médico", "Data", "Horário", "Status");
            Console.WriteLine(new string('-', 85));

            foreach (var consulta in consultas)
            {
                // Buscar nome do paciente
                string pacienteNome = "Desconhecido";
                var paciente = ObterPacientePorId(consulta.PacienteId);
                if (paciente != null)
                {
                    pacienteNome = paciente.NomeCompleto;
                }

                // Buscar nome do médico
                string medicoNome = "Desconhecido";
                var medico = medicoRepository.ObterTodos().FirstOrDefault(m => m.Id == consulta.MedicoId);
                if (medico != null)
                {
                    medicoNome = medico.NomeCompleto;
                }

                // Exibe os detalhes da entidade Consulta
                Console.WriteLine("{0,-18} | {1,-18} | {2,-12} | {3,-8} | {4,-10}", 
                pacienteNome.Length > 16 ? pacienteNome.Substring(0, 15) + "..." : pacienteNome,
                medicoNome.Length > 16 ? medicoNome.Substring(0, 15) + "..." : medicoNome,
                consulta.DataHora.ToString("dd/MM/yyyy"),
                consulta.DataHora.ToString("HH:mm"),
                consulta.Status);
            }
        }

        private void ExibirGerenciamentoPagamentos()
        {
            while (true)
            {
                Console.WriteLine("-- Gestão de Pagamentos --\n");
                Console.WriteLine("1 - Lista de Pagamentos pendentes");
                Console.WriteLine("2 - Lista  de Pagamentos realizados");
                Console.WriteLine("3 - Registrar Pagamento (Dar Baixa)");
                Console.WriteLine("0 - Voltar");
                Console.Write("Escolha uma opção: ");

                string opcao = (Console.ReadLine() ?? string.Empty).Trim();
                ConsoleHelper.LimparTela();

                switch (opcao)
                {
                    case "1":
                        ListarPagamentos(false); // false = pendente
                        break;
                    case "2":
                        ListarPagamentos(true);  // true = pago
                        break;
                    case "3":
                        RegistrarPagamento();
                        break;
                    case "0":
                    return;
                    default:
                    Console.WriteLine("Opção inválida.");
                    break;
                }

                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                ConsoleHelper.AguardarInteracao();
                ConsoleHelper.LimparTela();
            }
        }
        
        // Método auxiliar para listar pendentes ou pagos
        private void ListarPagamentos(bool statusPago)
        {
            var todasConsultas = consultaRepository.ObterTodas();
    
            // Filtra as consultas baseadas no status (FoiPago)
            var consultasFiltradas = todasConsultas.Where(c => c.FoiPago == statusPago).ToList();

            string statusTexto = statusPago ? "Realizados" : "Pendentes";
            Console.WriteLine($"-- Pagamentos {statusTexto} --\n");

            if (!consultasFiltradas.Any())
            {
                Console.WriteLine($"Nenhum pagamento {statusTexto.ToLower()} encontrado.");
                return;
            }

            Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-10}", "ID", "Paciente", "Data", "Valor (R$)");
            Console.WriteLine(new string('-', 60));

            foreach (var c in consultasFiltradas)
            {
                // Buscar nome do paciente
                string pacienteNome = "Desconhecido";
                var paciente = ObterPacientePorId(c.PacienteId);
                if (paciente != null)
                {
                    pacienteNome = paciente.NomeCompleto;
                }

                Console.WriteLine("{0,-5} | {1,-20} | {2,-15} | {3,-10}", 
                c.Id, 
                pacienteNome.Length > 18 ? pacienteNome.Substring(0, 17) + "..." : pacienteNome,
                c.DataHora.ToString("dd/MM/yyyy"), 
                c.Valor.ToString("F2"));
            }
        }

        // Método para dar baixa no pagamento
        private void RegistrarPagamento()
        {
            Console.WriteLine("-- Registrar Pagamento --\n");
    
            // Mostra os pendentes para facilitar a visualização do ADM
            ListarPagamentos(false); 

            Console.Write("\nDigite o ID da consulta para dar baixa (ou 0 para cancelar): ");
    
            if (int.TryParse(Console.ReadLine(), out int idConsulta) && idConsulta != 0)
            {
                // Busca a consulta específica. (Certifique-se de ter o método ObterPorId no repositório)
                var consulta = consultaRepository.ObterPorId(idConsulta);

                if (consulta != null && !consulta.FoiPago)
                {
                    consulta.FoiPago = true;
                    consultaRepository.Atualizar(consulta); // Salva a alteração no arquivo JSON
            
                    Console.WriteLine("\nSucesso! Pagamento registrado e recibo digital anexado ao perfil do paciente.");
                }
                else if (consulta != null && consulta.FoiPago)
                {
                    Console.WriteLine("\nEsta consulta já consta como paga no sistema.");
                }
            }
            else
            {
                Console.WriteLine("\nID de consulta não encontrado.");
            }
        }
    
        private void ExibirMedicos()
        {
            IReadOnlyList<Medico> medicos = this.medicoRepository.ObterTodos();

            Console.WriteLine("-- Corpo Clínico --");
            Console.WriteLine($"Total de médicos cadastrados: {medicos.Count}\n");

            foreach (Medico medico in medicos)
            {
                medico.ExibirResumo();
            }

        }

        private void CadastrarMedicoPeloAdministrador()
        {
            Console.WriteLine("-- Cadastrar Médico --");

            string nome = ConsoleHelper.LerTextoObrigatorio("Nome completo: ");
            string cpf = usuarioConsoleService.LerCpfMedicoDisponivel();
            string crm = usuarioConsoleService.LerCrmDisponivel();
            string especialidade = ConsoleHelper.LerTextoObrigatorio("Especialidade: ");
            string email = usuarioConsoleService.LerEmailDisponivelParaMedico();
            string senha = ConsoleHelper.LerSenhaValida();

            Medico medico = new Medico(nome, cpf, crm, especialidade, email, senha);

            try
            {
                medicoRepository.Salvar(medico);
                Console.WriteLine("\nMédico cadastrado com sucesso!");
                Console.WriteLine($"CRM: {medico.Crm}");
                Console.WriteLine($"E-mail de acesso: {medico.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível cadastrar o médico no momento.");
                Console.WriteLine(ex.Message);
            }
        }
    
        private PacienteRegistro? ObterPacientePorId(int pacienteId)
        {
            var todosCadastros = cadastroRepository.ObterTodos();
            return todosCadastros
                .SelectMany(c => c.Pacientes)
                .FirstOrDefault(p => p.Id == pacienteId);
        }
   
    }

}
