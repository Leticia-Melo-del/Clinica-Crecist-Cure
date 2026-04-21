using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Clinica_Crescit.Cura
{
    public class Medico : Pessoa // Representa um médico cadastrado no sistema
    {
        public int Id { get; set; }
        public string CpfNumerico { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public string Especialidade { get; set; } = string.Empty;

        [JsonPropertyName("MedEmail")]
        public string Email { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public Medico() : base(string.Empty, string.Empty)
        {
        }
        
        // Construtor para criar um médico com os dados necessários, gerando o hash da senha
        public Medico(
            string nomeCompleto,
            string cpf,
            string crm,
            string especialidade,
            string email,
            string senhaEmTexto) : base(nomeCompleto, cpf)
        {
            CpfNumerico = LimparDigitos(cpf);
            Crm = crm;
            Especialidade = especialidade;
            Email = NormalizarEmailPersistido(email);
            SenhaHash = SenhaService.GerarHash(senhaEmTexto);
        }

        // Método para normalizar os dados do médico antes de salvar, garantindo que o ID seja definido e o CPF seja limpo
        public void NormalizarCadastro(int idSugerido) 
        {
            if (Id <= 0)
            {
                Id = idSugerido;
            }

            if (string.IsNullOrWhiteSpace(CpfNumerico))
            {
                CpfNumerico = LimparDigitos(CPF);
            }

            Email = NormalizarEmailPersistido(Email);

            if (string.IsNullOrWhiteSpace(SenhaHash))
            {
                SenhaHash = SenhaService.GerarHash(Medicos.SenhaPadraoCargaInicial);
            }
        }

        public override void ExibirResumo() // Exibe um resumo das informações do médico, incluindo especialidade e CRM
        {
            Console.WriteLine($"\nMédico: {NomeCompleto}");
            Console.WriteLine($"Especialidade: {Especialidade}");
            Console.WriteLine($"CRM: {Crm}");
            Console.WriteLine($"E-mail: {Email}");
        }

        private static string LimparDigitos(string valor) // Remove todos os caracteres não numéricos de uma string, útil para limpar o CPF antes de armazenar
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        private static string NormalizarEmailPersistido(string email) // Normaliza o e-mail para armazenamento, removendo espaços e pontos finais desnecessários
        {
            return (email ?? string.Empty).Trim().TrimEnd('.');
        }
    }

    public static class Medicos // Classe estática para fornecer uma lista inicial de médicos para carga inicial
    {
        public const string SenhaPadraoCargaInicial = "Medico123";

        public static List<Medico> CriarListaInicial()
        {
            return new List<Medico>
            {
                new Medico(
                    "Ana Silva",
                    "12345678143",
                    "CRM-SP 618605",
                    "Pediatria",
                    "ana.silva@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Marcos Oliveira",
                    "12345678224",
                    "CRM-SP 024566",
                    "Neurologia Pediatrica",
                    "marcos.oliveira@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Beatriz Costa",
                    "12345678305",
                    "CRM-SP 985477",
                    "Nutricionista Pediatrica",
                    "beatriz.costa@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Ricardo Alves",
                    "12345678496",
                    "CRM-SP 362110",
                    "Cardiologia Pediatrica",
                    "ricardo.alves@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Felipe Mendes",
                    "12345678577",
                    "CRM-SP 800289",
                    "Pneumologia Pediatrica",
                    "felipe.mendes@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Carla Dias",
                    "12345678658",
                    "CRM-SP 202465",
                    "Endocrinologia Pediatrica",
                    "carla.dias@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Ana Souza",
                    "12345678739",
                    "CRM-SP 502662",
                    "Puericultura Pediatrica",
                    "ana.souza@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial),
                new Medico(
                    "Luana Couto",
                    "12345678810",
                    "CRM-SP 125428",
                    "Dermatologia Pediatrica",
                    "luana.couto@clinicacrescitcura.com",
                    SenhaPadraoCargaInicial)
            };
        }
    }
}
