using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Clinica_Crescit.Cura
{
    public interface IAdministradorRepository // Interface para gerenciamento de administradores do sistema
    {
        string Destino { get; }

        IReadOnlyList<AdministradorRegistro> ObterTodos();
        AdministradorRegistro? ObterPorEmail(string email);
        bool EmailJaCadastrado(string email);
        void GarantirCargaInicial();
    }

    public class JsonAdministradorRepository : IAdministradorRepository // Implementação do repositório de administradores usando arquivos JSON
    {
        // Configurações para formatação do JSON
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
        {
            WriteIndented = true
        };

        // Construtor que define o caminho do arquivo JSON, usando um caminho padrão se nenhum for fornecido
        public JsonAdministradorRepository(string? caminhoArquivo = null) 
        {
            Destino = caminhoArquivo ?? Path.Combine(DescobrirDiretorioBase(), "dados", "administradores.json");
        }

        // Caminho do arquivo JSON onde os administradores são armazenados
        public string Destino { get; } 

        // Retorna uma lista de todos os administradores cadastrados, carregando-os do arquivo JSON
        public IReadOnlyList<AdministradorRegistro> ObterTodos()
        {
            return CarregarAdministradoresExistentes();
        }

        // Retorna um administrador com base no email fornecido, ou null se não encontrado
        public AdministradorRegistro? ObterPorEmail(string email) 
        {
            string emailNormalizado = NormalizarEmail(email);

            return CarregarAdministradoresExistentes()
                .FirstOrDefault(administrador => NormalizarEmail(administrador.Email) == emailNormalizado);
        }

        // Verifica se um email já está cadastrado no sistema
        public bool EmailJaCadastrado(string email) 
        {
            return ObterPorEmail(email) is not null;
        }

        // Garante que haja pelo menos um administrador cadastrado, criando um padrão se necessário
        public void GarantirCargaInicial() 
        {
            List<AdministradorRegistro> administradores = CarregarAdministradoresExistentes();

            if (administradores.Count > 0)
            {
                return;
            }

            PersistirAdministradores(Administradores.CriarListaInicial());
        }

        // Carrega os administradores existentes do arquivo JSON, garantindo que tenham IDs válidos e senhas hash
        private List<AdministradorRegistro> CarregarAdministradoresExistentes() 
        {
            if (!File.Exists(Destino))
            {
                return new List<AdministradorRegistro>();
            }

            string json = File.ReadAllText(Destino);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<AdministradorRegistro>();
            }

            List<AdministradorRegistro> administradores =
                JsonSerializer.Deserialize<List<AdministradorRegistro>>(json, _jsonOptions)
                ?? new List<AdministradorRegistro>();

            bool alterado = false;
            int proximoId = 1;

            foreach (AdministradorRegistro administrador in administradores)
            {
                if (administrador.Id <= 0)
                {
                    administrador.Id = proximoId;
                    alterado = true;
                }

                string emailNormalizado = NormalizarEmailPersistido(administrador.Email);

                if (administrador.Email != emailNormalizado)
                {
                    administrador.Email = emailNormalizado;
                    alterado = true;
                }

                if (string.IsNullOrWhiteSpace(administrador.SenhaHash))
                {
                    administrador.SenhaHash = SenhaService.GerarHash(Administradores.SenhaPadraoInicial);
                    alterado = true;
                }

                proximoId = Math.Max(proximoId, administrador.Id + 1);
            }

            if (alterado)
            {
                PersistirAdministradores(administradores);
            }

            return administradores;
        }

        // Salva a lista de administradores no arquivo JSON, criando o diretório se necessário
        private void PersistirAdministradores(List<AdministradorRegistro> administradores) 
        {
            string? diretorio = Path.GetDirectoryName(Destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            string json = JsonSerializer.Serialize(administradores, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Normaliza o email para comparação, removendo espaços, pontos finais e convertendo para maiúsculas
        private static string NormalizarEmail(string email) 
        {
            return (email ?? string.Empty).Trim().TrimEnd('.').ToUpperInvariant();
        }

        // Normaliza o email para armazenamento, removendo espaços e pontos finais, mas mantendo a capitalização original
        private static string NormalizarEmailPersistido(string email) 
        {
            return (email ?? string.Empty).Trim().TrimEnd('.');
        }

        // Descobre o diretório base do projeto procurando por um arquivo .csproj, para garantir que os dados sejam armazenados em um local consistente
        private static string DescobrirDiretorioBase() 
        {
            DirectoryInfo? diretorioAtual = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (diretorioAtual is not null)
            {
                if (diretorioAtual.GetFiles("*.csproj").Length > 0)
                {
                    return diretorioAtual.FullName;
                }

                diretorioAtual = diretorioAtual.Parent;
            }

            return Directory.GetCurrentDirectory();
        }
    }

    // Classe auxiliar para criar uma lista inicial de administradores, garantindo que haja um administrador padrão para acesso inicial ao sistema
    public static class Administradores 
    {
        public const string EmailPadraoInicial = "admin@clinicacrescitcura.com";
        public const string SenhaPadraoInicial = "Admin123";

        public static List<AdministradorRegistro> CriarListaInicial()
        {
            return new List<AdministradorRegistro>
            {
                new AdministradorRegistro
                {
                    Id = 1,
                    NomeCompleto = "Administrador Principal",
                    Email = EmailPadraoInicial,
                    SenhaHash = SenhaService.GerarHash(SenhaPadraoInicial)
                }
            };
        }
    }
}
