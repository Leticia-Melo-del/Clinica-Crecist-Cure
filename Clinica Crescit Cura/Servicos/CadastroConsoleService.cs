namespace Clinica_Crescit.Cura
{
    public class CadastroConsoleService
    {
        // Método para ler os dados de um paciente a partir do console, garantindo que o CPF seja válido e não conflite com o CPF do responsável ou outros pacientes já vinculados
        public Paciente LerDadosPaciente(string cpfResponsavel,IEnumerable<string>? cpfsPacientesExistentes = null)
        {
            string nomePac = UI.ConsoleHelper.LerTextoObrigatorio("Nome completo: ");
            string cpfPac = LerCpfPacienteValido(cpfResponsavel, cpfsPacientesExistentes);
            DateTime dataNascimento = UI.ConsoleHelper.LerDataNascimentoValida();
            string genero = UI.ConsoleHelper.LerGeneroValido();

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

        // Método para criar um novo cadastro a partir dos dados do responsável e da lista de pacientes
        public CadastroRegistro CriarCadastroAtualizado(
            CadastroRegistro original,
            ResponsavelRegistro? responsavel = null,
            List<PacienteRegistro>? pacientes = null)
        {
            return new CadastroRegistro
            {
                Id = original.Id,
                DataCadastroUtc = original.DataCadastroUtc,
                Responsavel = responsavel ?? CriarCopiaResponsavel(original.Responsavel),
                Pacientes = pacientes ?? original.Pacientes.Select(CriarCopiaPaciente).ToList()
            };
        }

        // Método para criar um registro de paciente a partir de um objeto Paciente
        public PacienteRegistro CriarPacienteRegistro(Paciente paciente)
        {
            return new PacienteRegistro
            {
                NomeCompleto = paciente.NomeCompleto,
                Cpf = paciente.CPF,
                CpfNumerico = UI.ConsoleHelper.LimparDigitos(paciente.CPF),
                Genero = paciente.Genero,
                DataNascimento = paciente.DataNascimento,
                TipoSanguineo = paciente.TipoSanguineo,
                Alergias = paciente.Alergias,
                DoencasPreExistentes = paciente.DoencasPreExistentes,
                Peso = paciente.Peso,
                Altura = paciente.Altura
            };
        }

        // Método para criar uma cópia de um registro de responsável, garantindo que as informações sejam preservadas
        public ResponsavelRegistro CriarCopiaResponsavel(ResponsavelRegistro responsavel)
        {
            return new ResponsavelRegistro
            {
                Id = responsavel.Id,
                NomeCompleto = responsavel.NomeCompleto,
                Cpf = responsavel.Cpf,
                CpfNumerico = responsavel.CpfNumerico,
                Telefone = responsavel.Telefone,
                TelefoneNumerico = responsavel.TelefoneNumerico,
                Email = responsavel.Email,
                Endereco = responsavel.Endereco,
                SenhaHash = responsavel.SenhaHash
            };
        }

        // Método para criar uma cópia de um registro de paciente, garantindo que as informações sejam preservadas
        public PacienteRegistro CriarCopiaPaciente(PacienteRegistro paciente)
        {
            return new PacienteRegistro
            {
                Id = paciente.Id,
                ResponsavelId = paciente.ResponsavelId,
                NomeCompleto = paciente.NomeCompleto,
                Cpf = paciente.Cpf,
                CpfNumerico = paciente.CpfNumerico,
                Genero = paciente.Genero,
                DataNascimento = paciente.DataNascimento,
                TipoSanguineo = paciente.TipoSanguineo,
                Alergias = paciente.Alergias,
                DoencasPreExistentes = paciente.DoencasPreExistentes,
                Peso = paciente.Peso,
                Altura = paciente.Altura
            };
        }

        // Método para ler um CPF de paciente válido, garantindo que ele seja diferente do CPF do responsável e que não esteja vinculado a outro paciente já existente
        private static string LerCpfPacienteValido(string cpfResponsavel,
            IEnumerable<string>? cpfsPacientesExistentes = null)
        {
            string cpfResponsavelLimpo = UI.ConsoleHelper.LimparDigitos(cpfResponsavel);
            HashSet<string> cpfsJaVinculados = (cpfsPacientesExistentes ?? Enumerable.Empty<string>())
                .Select(UI.ConsoleHelper.LimparDigitos)
                .Where(cpf => !string.IsNullOrWhiteSpace(cpf))
                .ToHashSet();

            while (true)
            {
                string cpf = UI.ConsoleHelper.LerCpfValido();
                string cpfLimpo = UI.ConsoleHelper.LimparDigitos(cpf);

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
    }
}
