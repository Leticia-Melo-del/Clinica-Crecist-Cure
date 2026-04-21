
namespace Clinica_Crescit.Cura.UI
{

    // Menu específico para responsáveis, com opções de marcação de consultas, visualização de consultas marcadas, dados pessoais e gerenciamento de pacientes vinculados
    public class MenuResponsavel
    {
        private readonly ICadastroRepository _cadastroRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly IConsultaRepository _consultaRepo;
        private readonly CadastroConsoleService _cadastroConsoleService;
        private readonly UsuarioConsoleService _usuarioConsoleService;

        public MenuResponsavel(
            ICadastroRepository cadastroRepo,
            IMedicoRepository medicoRepo,
            IConsultaRepository consultaRepo,
            CadastroConsoleService cadastroConsoleService,
            UsuarioConsoleService usuarioConsoleService)
        {
            _cadastroRepo = cadastroRepo;
            _medicoRepo = medicoRepo;
            _consultaRepo = consultaRepo;
            _cadastroConsoleService = cadastroConsoleService;
            _usuarioConsoleService = usuarioConsoleService;
        }

        public void Exibir(CadastroRegistro usuarioLogado)
        {
            while (true)
            {
                ConsoleHelper.LimparTela();
                Console.WriteLine("--- Área do Responsável ---");
                Console.WriteLine($"Responsável: {usuarioLogado.Responsavel.NomeCompleto}");
                Console.WriteLine("1 - Marcar Consulta");
                Console.WriteLine("2 - Ver Consultas Marcadas");
                Console.WriteLine("3 - Diagnósticos de Consultas");
                Console.WriteLine("4 - Área de Dados");
                Console.WriteLine("5 - Adicionar Filho(a)");
                Console.WriteLine("6 - Apagar Conta"); 
                Console.WriteLine("0 - Sair da conta");
                
                string opcao = Console.ReadLine()?.Trim() ?? "";
                
                switch (opcao)
                {
                    case "1":
                        MarcarConsulta(usuarioLogado);
                        break;
                    case "2":
                        VerConsultasMarcadas(usuarioLogado);
                        break;
                    case "3":
                        VerResultadosConsultas(usuarioLogado);
                        break;
                    case "4":
                        usuarioLogado = ExibirAreaDeDados(usuarioLogado);
                        break;
                    case "5":
                        usuarioLogado = AdicionarPacienteAoCadastro(usuarioLogado);
                        break;
                    case "6":
                        bool contaApagada = ApagarConta(usuarioLogado);
                        if (contaApagada) return; // Sai do menu pois a conta não existe mais
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

        private void MarcarConsulta(CadastroRegistro usuarioLogado)
        {
            // 1. SELEÇÃO DO PACIENTE 
            PacienteRegistro? pacienteSelecionado = null;
            while (pacienteSelecionado == null)
            {
                Console.WriteLine("\nSelecione o Paciente:");
            
            for (int i = 0; i < usuarioLogado.Pacientes.Count; i++)
            {
            Console.WriteLine($"{i + 1} - {usuarioLogado.Pacientes[i].NomeCompleto}");
            }

            if (int.TryParse(Console.ReadLine(), out int pIndex) && pIndex >= 1 && pIndex <= usuarioLogado.Pacientes.Count)
            {
                pacienteSelecionado = usuarioLogado.Pacientes[pIndex - 1];
            }
            else
            {
                Console.WriteLine("Seleção inválida. Escolha um número da lista.");
            }
        
            }
    
            // 2. SELEÇÃO DO MÉDICO
            var medicos = _medicoRepo.ObterTodos();
            Medico? medicoSelecionado = null;
            while (medicoSelecionado == null)
            {
                Console.WriteLine("\nSelecione o Médico:");

                for (int i = 0; i < medicos.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {medicos[i].NomeCompleto} ({medicos[i].Especialidade})");
                }

                Console.Write("Digite o número correspondente: ");
                if (int.TryParse(Console.ReadLine(), out int medicoIndex) && medicoIndex >= 1 && medicoIndex <= medicos.Count)
                {
                    medicoSelecionado = medicos[medicoIndex - 1];
                }
                else
                {
                    Console.WriteLine("Opção inválida. Tente novamente.");
                }
            }

            // 3. SELEÇÃO DA DATA E HORA
            DateTime dataFinal;
            while (true)

            {
                DateTime dataApenas;
                while (true)
                {
                    Console.Write("\nDigite a data da consulta (dd/MM/yyyy): ");
                    string dataInput = Console.ReadLine() ?? string.Empty;

                    if (DateTime.TryParseExact(dataInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataApenas))
                    {
                        if (dataApenas.Date < DateTime.Now.Date)
                        {
                            Console.WriteLine("Não é possível marcar para uma data que já passou.");
                            continue;
                        }

                        break;
                    }

                    Console.WriteLine("Formato de data inválido. Use o padrão: 25/12/2026");
                }

                TimeSpan horaApenas;
                while (true)
                {
                    Console.Write("Digite o horário (HH:mm): ");
                    string horaInput = Console.ReadLine() ?? string.Empty;

                    if (TimeSpan.TryParseExact(horaInput, @"hh\:mm", null, out horaApenas))
                    {
                        break;
                    }

                    Console.WriteLine("Formato de hora inválido. Use o padrão: 14:30");
                }

                dataFinal = dataApenas.Date.Add(horaApenas);

                if (dataFinal < DateTime.Now)
                {
                    Console.WriteLine("Este horário já passou. Escolha um horário futuro.");
                    continue;
                }

                bool ocupado = _consultaRepo.ObterTodas()
                    .Any(c => c.MedicoId == medicoSelecionado.Id && c.DataHora == dataFinal);

                if (ocupado)
                {
                    Console.WriteLine($"O médico {medicoSelecionado.NomeCompleto} já tem consulta marcada às {dataFinal:HH:mm} no dia {dataFinal:dd/MM/yyyy}.");
                    continue;
                }

                break;
            }

            // 4. OBSERVAÇÕES (ADICIONAL)
            Console.WriteLine("\nDeseja adicionar alguma observação ou motivo da consulta para o médico? (Opcional)");
            Console.WriteLine("Apenas pressione Enter se não quiser adicionar nada.");
            Console.Write("Observações: ");
            string observacoes = Console.ReadLine()?.Trim() ?? string.Empty;

            // 5. FLUXO DE PAGAMENTO
            decimal valorConsulta = 250.00m; // Valor fixo simulado para clínica privada
            ConsoleHelper.LimparTela();
            Console.WriteLine("--- Pagamento ---");
            Console.WriteLine($"Consulta com: {medicoSelecionado.NomeCompleto}");
            Console.WriteLine($"Valor: {valorConsulta:C2}");
            Console.WriteLine("\nSelecione a forma de pagamento:");
            Console.WriteLine("1 - Cartão de Crédito/Débito");
            Console.WriteLine("2 - PIX");
            Console.Write("Opção: ");
            string formaPagamento = Console.ReadLine() == "2" ? "PIX" : "Cartão";

            Console.WriteLine($"\nProcessando pagamento via {formaPagamento}...");
            Thread.Sleep(1500); // Simulação de processamento
            Console.WriteLine("Pagamento confirmado com sucesso!");

            // 5. SALVAMENTO E COMPROVANTE
            var consulta = new Consulta
            {
                PacienteId = pacienteSelecionado.Id,
                MedicoId = medicoSelecionado.Id,
                DataHora = dataFinal,
                Valor = valorConsulta, // Agora armazena o valor na consulta
                Status = "Confirmada",
                Observacoes = observacoes
            };

            _consultaRepo.Salvar(consulta);

            // Gerar Comprovante no Console
            ExibirComprovante(usuarioLogado.Responsavel.NomeCompleto, pacienteSelecionado.NomeCompleto, medicoSelecionado, dataFinal, valorConsulta, formaPagamento);

            ConsoleHelper.AguardarInteracao();
        }

        private void ExibirComprovante(string responsavel, string paciente, Medico medico, DateTime data, decimal valor, string forma)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("--COMPROVANTE DE AGENDAMENTO--");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"Emissão: {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Responsável: {responsavel}");
            Console.WriteLine($"Paciente:    {paciente}");
            Console.WriteLine($"Médico:      {medico.NomeCompleto}");
            Console.WriteLine($"Especialidade: {medico.Especialidade}");
            Console.WriteLine($"Data/Hora:   {data:dd/MM/yyyy} às {data:HH:mm}");
            Console.WriteLine($"Valor Pago:  {valor:C2} ({forma})");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine(" -- Clínica Crescit Cura Agradece! -- ");
            Console.WriteLine(new string('=', 40));
        }

        private void VerConsultasMarcadas(CadastroRegistro usuarioLogado)
        {
            
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Consultas Marcadas --");

            var consultas = new List<Consulta>();
            foreach (var paciente in usuarioLogado.Pacientes)
            {
                consultas.AddRange(_consultaRepo.ObterPorPaciente(paciente.Id));
            }

            if (consultas.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta marcada.");
                ConsoleHelper.AguardarInteracao();
                return;
            }

            while (true)
            {
                ConsoleHelper.LimparTela();
                Console.WriteLine("-- Suas Consultas --\n");

                // Mantemos a ordenação aqui dentro para atualizar caso algo seja cancelado
                var consultasOrdenadas = consultas.OrderBy(c => c.DataHora).ToList();

                for (int i = 0; i < consultasOrdenadas.Count; i++)
                {
                    var consulta = consultasOrdenadas[i];
                    var medico = _medicoRepo.ObterTodos().FirstOrDefault(m => m.Id == consulta.MedicoId);
                    var paciente = usuarioLogado.Pacientes.FirstOrDefault(p => p.Id == consulta.PacienteId);
                    Console.WriteLine($"{i + 1} - Paciente: {paciente?.NomeCompleto} | Médico: {medico?.NomeCompleto}");
                    Console.WriteLine($"   Data: {consulta.DataHora:dd/MM/yyyy}  Hora: {consulta.DataHora:HH:mm} | Valor: {consulta.Valor:C2} | Status: {consulta.Status}");
                    Console.WriteLine();
                }

                Console.WriteLine("0 - Voltar");
                Console.Write("Digite o número da consulta para selecionar (ou 0 para voltar): ");
                
                if (int.TryParse(Console.ReadLine(), out int opcao))
                {
                    if (opcao == 0)
                    {
                        break;
                    }

                    if (opcao > 0 && opcao <= consultasOrdenadas.Count)
                    {
                        var consultaSelecionada = consultasOrdenadas[opcao - 1];
                        var medicoSelecionado = _medicoRepo.ObterTodos().FirstOrDefault(m => m.Id == consultaSelecionada.MedicoId);
                        var pacienteSelecionado = usuarioLogado.Pacientes.FirstOrDefault(p => p.Id == consultaSelecionada.PacienteId);

                        if (medicoSelecionado != null)
                        {
                            // --- NOVO SUB-MENU DA CONSULTA ---
                            ConsoleHelper.LimparTela();
                            Console.WriteLine($"--- Opções da Consulta (Status: {consultaSelecionada.Status}) ---");
                            Console.WriteLine("1 - Ver Comprovante");
                            
                            // Só exibe a opção de desmarcar se a consulta não estiver cancelada
                            bool podeDesmarcar = consultaSelecionada.Status != "Cancelada";
                            if (podeDesmarcar)
                            {
                                Console.WriteLine("2 - Desmarcar Consulta");
                            }
                            Console.WriteLine("0 - Voltar");
                            Console.Write("Escolha uma opção: ");
                            
                            string subOpcao = Console.ReadLine()?.Trim() ?? "";

                            if (subOpcao == "1")
                            {
                                ExibirComprovante(usuarioLogado.Responsavel.NomeCompleto, pacienteSelecionado?.NomeCompleto ?? "Desconhecido", medicoSelecionado, consultaSelecionada.DataHora, consultaSelecionada.Valor, "Pago");
                                ConsoleHelper.AguardarInteracao();
                            }
                            else if (subOpcao == "2" && podeDesmarcar)
                            {
                                // --- NOVA VALIDAÇÃO DE SENHA ---
                                Console.WriteLine("\nPara sua segurança, confirme sua identidade.");
                                Console.Write("Digite sua senha: ");
                                string senhaConfirma = Console.ReadLine() ?? "";

                                if (SenhaService.VerificarSenha(senhaConfirma, usuarioLogado.Responsavel.SenhaHash))
                                {
                                    Console.Write("\nTem certeza que deseja desmarcar esta consulta? (s/n): ");
                                    if (Console.ReadLine()?.ToUpper() == "S")
                                    {
                                        consultaSelecionada.Status = "Cancelada";
                                        
                                        // Salva a alteração no arquivo JSON
                                        _consultaRepo.Atualizar(consultaSelecionada); 
                                        
                                        Console.WriteLine("\nConsulta desmarcada com sucesso!");
                                    }
                                    else 
                                    {
                                        Console.WriteLine("\nOperação cancelada. A consulta foi mantida.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nSenha incorreta. Acesso negado.");
                                }
                                ConsoleHelper.AguardarInteracao();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Erro: Médico não encontrado para esta consulta.");
                            ConsoleHelper.AguardarInteracao();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida!");
                        ConsoleHelper.AguardarInteracao();
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida!");
                    ConsoleHelper.AguardarInteracao();
                }
            }
        }

        private CadastroRegistro AdicionarPacienteAoCadastro(CadastroRegistro usuarioLogado)
        {
            Console.WriteLine("-- Adicionar Paciente --");
            Paciente paciente = _cadastroConsoleService.LerDadosPaciente(
                usuarioLogado.Responsavel.Cpf,
                usuarioLogado.Pacientes.Select(item => item.Cpf));

            List<PacienteRegistro> pacientesAtualizados = usuarioLogado.Pacientes
                .Select(_cadastroConsoleService.CriarCopiaPaciente)
                .ToList();

            pacientesAtualizados.Add(_cadastroConsoleService.CriarPacienteRegistro(paciente));

            CadastroRegistro cadastroAtualizado = _cadastroConsoleService.CriarCadastroAtualizado(
                usuarioLogado,
                pacientes: pacientesAtualizados);

            try
            {
                _cadastroRepo.Atualizar(cadastroAtualizado);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível adicionar o paciente no momento.");
                Console.WriteLine(ex.Message);
                ConsoleHelper.AguardarInteracao();
                return usuarioLogado;
            }

            Console.WriteLine($"Paciente {paciente.NomeCompleto} adicionado com sucesso!");
            Console.WriteLine($"Total de pacientes vinculados: {cadastroAtualizado.Pacientes.Count}");
            ConsoleHelper.AguardarInteracao();
            return cadastroAtualizado;
        }

        private CadastroRegistro AlterarDadosResponsavel(CadastroRegistro usuarioLogado)
        {
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Alterar Dados --");
            Console.WriteLine("Pressione Enter para manter a mesma informação.");

            string nomeAtualizado = ConsoleHelper.LerTextoComValorAtual(
                "Nome completo",
                usuarioLogado.Responsavel.NomeCompleto);

            string telefoneAtualizado = ConsoleHelper.LerTelefoneComValorAtual(usuarioLogado.Responsavel.Telefone);
            string emailAtualizado = _usuarioConsoleService.LerEmailComValorAtual(usuarioLogado);
            string enderecoAtualizado = ConsoleHelper.LerEnderecoComValorAtual(usuarioLogado.Responsavel.Endereco);

            ResponsavelRegistro responsavelAtualizado = new ResponsavelRegistro
            {
                Id = usuarioLogado.Responsavel.Id,
                NomeCompleto = nomeAtualizado,
                Cpf = usuarioLogado.Responsavel.Cpf,
                CpfNumerico = usuarioLogado.Responsavel.CpfNumerico,
                Telefone = telefoneAtualizado,
                TelefoneNumerico = ConsoleHelper.LimparDigitos(telefoneAtualizado),
                Email = emailAtualizado,
                Endereco = enderecoAtualizado,
                SenhaHash = usuarioLogado.Responsavel.SenhaHash
            };

            CadastroRegistro cadastroAtualizado = _cadastroConsoleService.CriarCadastroAtualizado(
                usuarioLogado,
                responsavel: responsavelAtualizado);

            try
            {
                _cadastroRepo.Atualizar(cadastroAtualizado);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível atualizar os dados no momento.");
                Console.WriteLine(ex.Message);
                ConsoleHelper.AguardarInteracao();
                return usuarioLogado;
            }

            Console.WriteLine("Dados atualizados com sucesso!");
            Console.WriteLine($"Novo e-mail: {cadastroAtualizado.Responsavel.Email}");
            Console.WriteLine($"Novo telefone: {cadastroAtualizado.Responsavel.Telefone}");
            ConsoleHelper.AguardarInteracao();
            return cadastroAtualizado;
        }

        private string LerEmailComValorAtual(CadastroRegistro cadastro)
        {
            while (true)
            {
                Console.Write($"E-mail ({cadastro.Responsavel.Email}): ");
                string entrada = (Console.ReadLine() ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    return cadastro.Responsavel.Email;
                }

                if (!ConsoleHelper.EhEmailValido(entrada))
                {
                    Console.WriteLine("E-mail inválido. Digite novamente.");
                    continue;
                }

                bool emailEmUsoPorOutroResponsavel =
                    _cadastroRepo.EmailJaCadastradoPorOutroCadastro(entrada, cadastro.Id);

                bool emailEmUsoPorOutroPerfil =
                    _medicoRepo.EmailJaCadastrado(entrada);

                if (emailEmUsoPorOutroResponsavel || emailEmUsoPorOutroPerfil)
                {
                    Console.WriteLine("Já existe um cadastro com esse e-mail.");
                    continue;
                }

                return entrada;
            }
        }

        private static Paciente LerDadosPaciente(string cpfResponsavel, IEnumerable<string>? cpfsPacientesExistentes = null)
        {
            string nomePac = ConsoleHelper.LerTextoObrigatorio("Nome completo: ");
            string cpfPac = LerCpfPacienteValido(cpfResponsavel, cpfsPacientesExistentes);
            DateTime dataNascimento = ConsoleHelper.LerDataNascimentoValida();
            string genero = ConsoleHelper.LerGeneroValido();

            Paciente paciente = new Paciente(nomePac, cpfPac, genero, dataNascimento);

            Console.Write("Tipo Sanguíneo (opcional): ");
            paciente.TipoSanguineo = Console.ReadLine() ?? string.Empty;

            Console.Write("Possui Alergias? (opcional): ");
            paciente.Alergias = Console.ReadLine() ?? string.Empty;

            Console.Write("Possui Doenças Pre-existentes? (opcional): ");
            paciente.DoencasPreExistentes = Console.ReadLine() ?? string.Empty;

            Console.Write("Peso (kg) (opcional): ");
            paciente.Peso = double.TryParse(Console.ReadLine(), out double peso) ? peso : 0;

            Console.Write("Altura (m) (opcional): ");
            paciente.Altura = double.TryParse(Console.ReadLine(), out double altura) ? altura : 0;

            return paciente;
        }

        private static string LerCpfPacienteValido(
            string cpfResponsavel,
            IEnumerable<string>? cpfsPacientesExistentes = null)
        {
            string cpfResponsavelLimpo = ConsoleHelper.LimparDigitos(cpfResponsavel);
            HashSet<string> cpfsJaVinculados = (cpfsPacientesExistentes ?? Enumerable.Empty<string>())
                .Select(ConsoleHelper.LimparDigitos)
                .Where(cpf => !string.IsNullOrWhiteSpace(cpf))
                .ToHashSet();

            while (true)
            {
                string cpf = ConsoleHelper.LerCpfValido();
                string cpfLimpo = ConsoleHelper.LimparDigitos(cpf);

                if (cpfLimpo == cpfResponsavelLimpo)
                {
                    Console.WriteLine("O CPF do paciente deve ser diferente do CPF do responsável.");
                    continue;
                }

                if (cpfsJaVinculados.Contains(cpfLimpo))
                {
                    Console.WriteLine("Esse paciente já está vinculado ao responsável.");
                    continue;
                }

                return cpf;
            }
        }

        private static CadastroRegistro CriarCadastroAtualizado(
            CadastroRegistro original,
            ResponsavelRegistro? responsavel = null,
            List<PacienteRegistro>? pacientes = null)
        {
            return new CadastroRegistro
            {
                Id = original.Id,
                Responsavel = responsavel ?? original.Responsavel,
                Pacientes = pacientes ?? original.Pacientes
            };
        }

        private static PacienteRegistro CriarPacienteRegistro(Paciente paciente)
        {
            return new PacienteRegistro
            {
                NomeCompleto = paciente.NomeCompleto,
                Cpf = paciente.CPF,
                DataNascimento = paciente.DataNascimento,
                Genero = paciente.Genero,
                TipoSanguineo = paciente.TipoSanguineo,
                Alergias = paciente.Alergias,
                DoencasPreExistentes = paciente.DoencasPreExistentes,
                Peso = paciente.Peso,
                Altura = paciente.Altura
            };
        }

        private static PacienteRegistro CriarCopiaPaciente(PacienteRegistro paciente)

        {
            return new PacienteRegistro
            {
                Id = paciente.Id,
                NomeCompleto = paciente.NomeCompleto,
                Cpf = paciente.Cpf,
                DataNascimento = paciente.DataNascimento,
                Genero = paciente.Genero,
                TipoSanguineo = paciente.TipoSanguineo,
                Alergias = paciente.Alergias,
                DoencasPreExistentes = paciente.DoencasPreExistentes,
                Peso = paciente.Peso,
                Altura = paciente.Altura
            };
        }
    
        private bool ApagarConta(CadastroRegistro usuarioLogado)
        {
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Apagar Conta --");

            // 1. Verificar se há consultas marcadas para qualquer paciente deste cadastro
            var temConsultasPendente = usuarioLogado.Pacientes.Any(
                p => _consultaRepo.ObterPorPaciente(p.Id)
                    .Any(c => c.Status == "Agendada" || c.Status == "Confirmada" || c.Status == "Confirmada (Paga)"));

            if (temConsultasPendente)
            {
                Console.WriteLine("Não é possível apagar a conta pois existem consultas agendadas.");
                ConsoleHelper.AguardarInteracao();
            return false;
            }

            // 2. Pedir confirmação de segurança
            Console.WriteLine("ATENÇÃO: Esta ação é irreversível e apagará todos os seus dados.");
            Console.WriteLine("Para confirmar, forneça suas credenciais:");

            Console.Write("E-mail: ");
            string emailConfirma = Console.ReadLine() ?? "";
    
            Console.Write("Senha: ");
            string senhaConfirma = Console.ReadLine() ?? "";

            // 3. Validar credenciais (usando a mesma lógica de login)
            // Nota: Como usuarioLogado.Responsavel.Email e SenhaHash já estão em memória, comparamos aqui
            bool emailValido = emailConfirma.Trim().ToLower() == usuarioLogado.Responsavel.Email.ToLower();
            bool senhaValida = SenhaService.VerificarSenha(senhaConfirma, usuarioLogado.Responsavel.SenhaHash);

            if (emailValido && senhaValida)
            {
                Console.Write("\nTem certeza que deseja apagar permanentemente? (S/N): ");
                if (Console.ReadLine()?.ToUpper() == "S")
                {
                    try
                    {
                        // Remove o cadastro do repositório
                        _cadastroRepo.Excluir(usuarioLogado.Id);
                        Console.WriteLine("\nSua conta e todos os dados vinculados foram apagados com sucesso.");
                        ConsoleHelper.AguardarInteracao();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao apagar conta: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Credenciais incorretas. Operação cancelada.");
            }
            ConsoleHelper.AguardarInteracao();
            return false;
        }
    
        private CadastroRegistro ExibirAreaDeDados(CadastroRegistro usuarioLogado)
        {
            while (true)
            {
                ConsoleHelper.LimparTela();
                Console.WriteLine("--- Área de Dados ---");

                // 1. Visualização dos Dados do Responsável
                Console.WriteLine("\n[ Seus Dados ]");
                Console.WriteLine($"Nome:     {usuarioLogado.Responsavel.NomeCompleto}");
                Console.WriteLine($"CPF:      {usuarioLogado.Responsavel.Cpf}");
                Console.WriteLine($"E-mail:   {usuarioLogado.Responsavel.Email}");
                Console.WriteLine($"Telefone: {usuarioLogado.Responsavel.Telefone}");
                Console.WriteLine($"Endereço: {usuarioLogado.Responsavel.Endereco}");

                // 2. Visualização dos Dados dos Pacientes
                Console.WriteLine("\n[ Pacientes Vinculados ]");
                
                foreach (var pac in usuarioLogado.Pacientes)
                {
                    Console.WriteLine($"- {pac.NomeCompleto.PadRight(20)} | CPF: {pac.Cpf} | Nasc: {pac.DataNascimento:dd/MM/yyyy}");
                if (!string.IsNullOrWhiteSpace(pac.Alergias)) Console.WriteLine($"  Alergias: {pac.Alergias}");
                }

                // 3. Sub-menu de Ações
                Console.WriteLine("\n1 - Alterar Meus Dados");
                Console.WriteLine("2 - Alterar Dados de um Filho(a)");
                Console.WriteLine("0 - Voltar");
                Console.Write("\nEscolha uma opção: ");

                string opcao = Console.ReadLine()?.Trim() ?? "";

                switch (opcao)
                    {
                        case "1":
                            usuarioLogado = AlterarDadosResponsavel(usuarioLogado);
                            break;
                        case "2":
                            usuarioLogado = AlterarDadosPaciente(usuarioLogado);
                            break;
                        case "0":
                            return usuarioLogado;
                        default:
                            Console.WriteLine("Opção inválida!");
                            ConsoleHelper.AguardarInteracao();
                            break;
                    }
            }
        }

        private CadastroRegistro AlterarDadosPaciente(CadastroRegistro usuarioLogado)
        {
            Console.WriteLine("\n--- Editar Dados do Paciente ---");
    
            // Seleção do paciente
            for (int i = 0; i < usuarioLogado.Pacientes.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {usuarioLogado.Pacientes[i].NomeCompleto}");
            }

            Console.Write("Selecione o número do paciente para editar: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > usuarioLogado.Pacientes.Count)
            {
                Console.WriteLine("Paciente inválido.");
                ConsoleHelper.AguardarInteracao();
            return usuarioLogado;
            }

            var pacOriginal = usuarioLogado.Pacientes[index - 1];
            ConsoleHelper.LimparTela();
            Console.WriteLine($"Editando: {pacOriginal.NomeCompleto}");
            Console.WriteLine("Pressione Enter para manter a mesma informação.");

            // Coleta dos dados (usando helpers para manter o padrão)
            string novoNome = ConsoleHelper.LerTextoComValorAtual("Nome Completo", pacOriginal.NomeCompleto);
            string novoGenero = ConsoleHelper.LerTextoComValorAtual("Gênero", pacOriginal.Genero);
    
            // Para campos opcionais
            Console.Write($"Alergias ({pacOriginal.Alergias}): ");
            string novasAlergias = Console.ReadLine()?.Trim() ?? "";
            Console.Write($"Tipo Sanguíneo ({pacOriginal.TipoSanguineo}): ");
            string novoTipoSanguineo = Console.ReadLine()?.Trim() ?? "";
            Console.Write($"Peso ({pacOriginal.Peso} kg): ");
            string novoPeso = Console.ReadLine()?.Trim() ?? "";
            Console.Write($"Altura ({pacOriginal.Altura} m): ");
            string novaAltura = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(novasAlergias)) novasAlergias = pacOriginal.Alergias;
            if (string.IsNullOrEmpty(novoTipoSanguineo)) novoTipoSanguineo = pacOriginal.TipoSanguineo;
            if (string.IsNullOrEmpty(novoPeso)) novoPeso = pacOriginal.Peso.ToString();
            if (string.IsNullOrEmpty(novaAltura)) novaAltura = pacOriginal.Altura.ToString();

            // Criando a versão atualizada do paciente
            var pacAtualizado = new PacienteRegistro
            {
                Id = pacOriginal.Id,
                ResponsavelId = pacOriginal.ResponsavelId,
                NomeCompleto = novoNome,
                Cpf = pacOriginal.Cpf,
                CpfNumerico = pacOriginal.CpfNumerico,
                Genero = novoGenero,
                DataNascimento = pacOriginal.DataNascimento,
                TipoSanguineo = pacOriginal.TipoSanguineo,
                Alergias = novasAlergias,
                DoencasPreExistentes = pacOriginal.DoencasPreExistentes,
                Peso = pacOriginal.Peso,
                Altura = pacOriginal.Altura
            };

            // Atualiza a lista de pacientes no cadastro
            List<PacienteRegistro> pacientesNovos = usuarioLogado.Pacientes
                .Select(p => p.Id == pacOriginal.Id ? pacAtualizado : _cadastroConsoleService.CriarCopiaPaciente(p))
                .ToList();

            CadastroRegistro cadastroAtualizado = _cadastroConsoleService.CriarCadastroAtualizado(
                usuarioLogado,
                pacientes: pacientesNovos);

            try
            {
                _cadastroRepo.Atualizar(cadastroAtualizado);
                Console.WriteLine("\nDados do paciente atualizados com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
            }

            ConsoleHelper.AguardarInteracao();
            return cadastroAtualizado;
        
        }
    
        private void VerResultadosConsultas(CadastroRegistro usuarioLogado)
        {
            ConsoleHelper.LimparTela();
            Console.WriteLine("-- Histórico e Resultados de Consultas --");

            // 1. Buscar todas as consultas dos pacientes vinculados que já foram realizadas
            var consultasRealizadas = new List<Consulta>();
            foreach (var paciente in usuarioLogado.Pacientes)
            {
            var historico = _consultaRepo.ObterPorPaciente(paciente.Id)
                    .Where(c => c.Status == "Realizada")
                    .ToList();
                consultasRealizadas.AddRange(historico);
            }

            if (!consultasRealizadas.Any())
            {
                Console.WriteLine("Nenhum resultado de consulta disponível no momento.");
                ConsoleHelper.AguardarInteracao();
            return;
            }

            // 2. Listar as consultas para seleção
            var listaOrdenada = consultasRealizadas.OrderByDescending(c => c.DataHora).ToList();

            for (int i = 0; i < listaOrdenada.Count; i++)
            {
                var c = listaOrdenada[i];
                var pac = usuarioLogado.Pacientes.First(p => p.Id == c.PacienteId);
                var med = _medicoRepo.ObterTodos().FirstOrDefault(m => m.Id == c.MedicoId);
        
                Console.WriteLine($"{i + 1} - [{c.DataHora:dd/MM/yyyy}] Paciente: {pac.NomeCompleto} | Médico: {med?.NomeCompleto}");
            }

            Console.WriteLine("\n0 - Voltar");
            Console.Write("Selecione uma consulta para ver o prontuário: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= listaOrdenada.Count)
            {
                var selecionada = listaOrdenada[index - 1];
                ExibirProntuarioDetalhado(selecionada, usuarioLogado);
            }
        }

        private void ExibirProntuarioDetalhado(Consulta consulta, CadastroRegistro usuarioLogado)
        {
            var paciente = usuarioLogado.Pacientes.First(p => p.Id == consulta.PacienteId);
            var medico = _medicoRepo.ObterTodos().FirstOrDefault(m => m.Id == consulta.MedicoId);

            ConsoleHelper.LimparTela();
            Console.WriteLine("==========================================");
            Console.WriteLine("           PRONTUÁRIO MÉDICO              ");
            Console.WriteLine("==========================================");
            Console.WriteLine($"Paciente: {paciente.NomeCompleto}");
            Console.WriteLine($"Data:    {consulta.DataHora:dd/MM/yyyy}");
            Console.WriteLine($"Médico:  {medico?.NomeCompleto} ({medico?.Especialidade})");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("MOTIVO DA CONSULTA (Responsável):");
            Console.WriteLine(string.IsNullOrWhiteSpace(consulta.Observacoes) ? "Nenhuma observação informada." : consulta.Observacoes);
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("RESULTADO E RECOMENDAÇÕES (Médico):");
    
            if (string.IsNullOrWhiteSpace(consulta.Diagnostico))
            {
                Console.WriteLine("O médico ainda não registrou o feedback desta consulta.");
            }
            else
            {
                Console.WriteLine(consulta.Diagnostico);
            }
    
            Console.WriteLine("==========================================");
            ConsoleHelper.AguardarInteracao();
        } 
    
    }
}
