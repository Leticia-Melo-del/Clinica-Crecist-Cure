using System.Globalization;

namespace Clinica_Crescit.Cura.UI
{

    // Helper estático para leitura de dados do console, validações e outras funcionalidades relacionadas à interação com o usuário
    public static class ConsoleHelper
    {
        public static string LerTextoObrigatorio(string mensagem)
        {
            while (true)
            {
                Console.Write(mensagem);
                string texto = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto.Trim();
                }

                Console.WriteLine("Este campo é obrigatório. Digite novamente.");
            }
        }
        public static string LimparDigitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
        public static bool EhEmailValido(string email)
        {
            int posicaoArroba = email.IndexOf('@');
            int posicaoUltimoPonto = email.LastIndexOf('.');

            return !string.IsNullOrWhiteSpace(email)
                && posicaoArroba > 0
                && posicaoUltimoPonto > posicaoArroba + 1
                && posicaoUltimoPonto < email.Length - 1;
        }
        public static void LimparTela()
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
            }
        }
        public static void AguardarInteracao()
        {
            try
            {
                if (!Console.IsInputRedirected)
                {
                    Console.ReadKey();
                    return;
                }
            }
            catch (InvalidOperationException)
            {
            }

            if (!Console.IsInputRedirected)
            {
                return;
            }

            Console.Read();
        }
        public static string LerTextoComValorAtual(string campo, string valorAtual)
        {
            while (true)
            {
                Console.Write($"{campo} ({valorAtual}): ");
                string entrada = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    return valorAtual;
                }

                string valor = entrada.Trim();

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    return valor;
                }
            }
        }
        public static string LerCpfValido()

        {
            while (true)
            {
                Console.Write("CPF: ");
                string cpf = Console.ReadLine() ?? string.Empty;

                if (ValidadorCpf.EhValido(cpf))
                {
                    return cpf;
                }

                Console.WriteLine("CPF inválido. Digite novamente.");
            }
        }
        public static string LerTelefoneValido()
        {
            while (true)
            {
                Console.Write("Telefone: ");
                string telefone = Console.ReadLine() ?? string.Empty;
                string telefoneLimpo = LimparDigitos(telefone);

                if (telefoneLimpo.Length >= 10 && telefoneLimpo.Length <= 11)
                {
                    return telefone;
                }

                Console.WriteLine("Telefone inválido. Digite novamente com DDD.");
            }
        }
        public static string LerTelefoneComValorAtual(string valorAtual)
        {
            while (true)
            {
                Console.Write($"Telefone ({valorAtual}): ");
                string entrada = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    return valorAtual;
                }

                string telefone = entrada.Trim();
                string telefoneLimpo = LimparDigitos(telefone);

                if (telefoneLimpo.Length >= 10 && telefoneLimpo.Length <= 11)
                {
                    return telefone;
                }

                Console.WriteLine("Telefone inválido. Digite novamente com DDD.");
            }
        }
        public static string LerEmailValido()
        {
            while (true)
            {
                Console.Write("E-mail: ");
                string email = (Console.ReadLine() ?? string.Empty).Trim();

                if (EhEmailValido(email))
                {
                    return email;
                }

                Console.WriteLine("E-mail inválido. Digite novamente.");
            }
        }
        public static string LerEnderecoValido()
        {
            while (true)
            {
                Console.Write("Endereço: ");
                string endereco = (Console.ReadLine() ?? string.Empty).Trim();

                if (endereco.Length >= 8)
                {
                    return endereco;
                }

                Console.WriteLine("Endereço inválido. Digite um endereço mais completo.");
            }
        }
        public static string LerEnderecoComValorAtual(string valorAtual)
        {
            while (true)
            {
                Console.Write($"Endereço ({valorAtual}): ");
                string entrada = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(entrada))
                {
                    return valorAtual;
                }

                string endereco = entrada.Trim();

                if (endereco.Length >= 8)
                {
                    return endereco;
                }

                Console.WriteLine("Endereço inválido. Digite um endereço mais completo.");
            }
        }
        public static DateTime LerDataNascimentoValida()
        {
            while (true)
            {
                Console.Write("Data de nascimento (dd/mm/aaaa): ");
                string entrada = (Console.ReadLine() ?? string.Empty).Trim();

                bool dataValida = DateTime.TryParseExact(
                    entrada,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime dataNascimento);

                if (!dataValida)
                {
                    Console.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
                    continue;
                }

                if (dataNascimento > DateTime.Today)
                {
                    Console.WriteLine("A data de nascimento não pode ser no futuro.");
                    continue;
                }

                if (dataNascimento < DateTime.Today.AddYears(-120))
                {
                    Console.WriteLine("Data de nascimento inválida. Digite novamente.");
                    continue;
                }

                return dataNascimento;
            }
        }
        public static string LerGeneroValido()
        {
            while (true)
            {
                Console.WriteLine("Gênero:");
                Console.WriteLine("1 - Feminino");
                Console.WriteLine("2 - Masculino");
                Console.WriteLine("3 - Prefiro não responder");
                Console.WriteLine("4 - Descrever identidade");
                Console.Write("Escolha uma opção: ");

                string opcao = (Console.ReadLine() ?? string.Empty).Trim();

                switch (opcao)
                {
                    case "1":
                        return "Feminino";
                    case "2":
                        return "Masculino";
                    case "3":
                        return "Prefiro não responder";
                    case "4":
                        return LerTextoObrigatorio("Descreva a identidade de gênero: ");
                    default:
                        Console.WriteLine("Opção inválida. Digite um número de 1 a 4.");
                        break;
                }
            }
        }
        public static string LerSenhaValida()
        {
            while (true)
            {
                Console.WriteLine("\nSenha de Acesso: ");
                Console.WriteLine("- No mínimo 6 caracteres\n- Uma letra MAIÚSCULA\n- Uma letra minúscula\n- Um número");
                Console.Write("\nDigite a senha: ");
                string senha = Console.ReadLine() ?? "";

            // Validações individuais
            bool temMaiuscula = senha.Any(char.IsUpper);
            bool temMinuscula = senha.Any(char.IsLower);
            bool temNumero    = senha.Any(char.IsDigit);
            bool tamanhoOk    = senha.Length >= 6;

                if (tamanhoOk && temMaiuscula && temMinuscula && temNumero)
                {
                    Console.Write("Confirme a senha: ");
                
                if (Console.ReadLine() == senha) 
                {
                    Console.WriteLine("Senha válida!");
                    return senha;
                }
                    Console.WriteLine("As senhas não coincidem.");
                }
                else
                {
                    Console.WriteLine("Senha fraca! Verifique se você usou maiúsculas, minúsculas e números.");
                }

            }
        }
    }
}