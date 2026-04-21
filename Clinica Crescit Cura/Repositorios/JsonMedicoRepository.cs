using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Clinica_Crescit.Cura
{
    // Interface para gerenciamento de médicos do sistema, definindo os métodos necessários para operações CRUD e validação de dados
    public interface IMedicoRepository
    {
        string Destino { get; }

        IReadOnlyList<Medico> ObterTodos();
        Medico? ObterPorEmail(string email);
        void Salvar(Medico medico);
        bool EmailJaCadastrado(string email);
        bool CrmJaCadastrado(string crm);
        bool CpfJaCadastrado(string cpf);
        void GarantirCargaInicial();
    }

    // Implementação do repositório de médicos usando arquivos JSON para persistência dos dados
    public class JsonMedicoRepository : IMedicoRepository
    {
        // Configurações para formatação do JSON, garantindo que seja legível e consistente
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Construtor que define o caminho do arquivo JSON, usando um caminho padrão se nenhum for fornecido
        public JsonMedicoRepository(string? caminhoArquivo = null)
        {
            Destino = caminhoArquivo ?? Path.Combine(DescobrirDiretorioBase(), "dados", "medicos.json");
        }
        
        public string Destino { get; }

        // Método para obter todos os médicos cadastrados, carregando-os do arquivo JSON e garantindo que o sistema possa acessar a lista completa de médicos para operações de leitura ou validação
        public IReadOnlyList<Medico> ObterTodos()
        {
            return CarregarMedicosExistentes();
        }

        // Método para obter um médico específico pelo email, retornando null se não for encontrado, e garantindo que a busca seja feita de forma consistente, normalizando o email para evitar problemas de formatação
        public Medico? ObterPorEmail(string email)
        {
            string emailNormalizado = NormalizarEmail(email);

            return CarregarMedicosExistentes()
                .FirstOrDefault(medico => NormalizarEmail(medico.Email) == emailNormalizado);
        }

        // Método para salvar um novo médico, atribuindo um ID único e normalizando os dados antes de persistir no arquivo JSON, garantindo que as informações sejam armazenadas de forma consistente e que o sistema possa gerenciar os médicos de maneira eficaz
        public void Salvar(Medico medico)
        {
            string? diretorio = Path.GetDirectoryName(Destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            List<Medico> medicos = CarregarMedicosExistentes();
            medico.Id = ObterProximoId(medicos.Select(item => item.Id));
            medico.NormalizarCadastro(medico.Id);
            medicos.Add(medico);

            PersistirMedicos(medicos);
        }

        // Método para verificar se um email já está cadastrado no sistema, utilizando a normalização do email para garantir que a validação seja feita de forma consistente, evitando problemas com formatação ou diferenças de maiúsculas e minúsculas
        public bool EmailJaCadastrado(string email)
        {
            return ObterPorEmail(email) is not null;
        }

        // Método para verificar se um CRM já está cadastrado no sistema, utilizando a normalização do CRM para garantir que a validação seja feita de forma consistente, evitando problemas com formatação ou diferenças de maiúsculas e minúsculas
        public bool CrmJaCadastrado(string crm)
        {
            string crmNormalizado = NormalizarCrm(crm);

            return CarregarMedicosExistentes()
                .Any(medico => NormalizarCrm(medico.Crm) == crmNormalizado);
        }

        // Método para verificar se um CPF já está cadastrado no sistema, utilizando a limpeza dos dígitos para garantir que a validação seja feita de forma consistente, evitando problemas com formatação ou caracteres adicionais
        public bool CpfJaCadastrado(string cpf)
        {
            string cpfLimpo = LimparDigitos(cpf);

            return CarregarMedicosExistentes()
                .Any(medico => LimparDigitos(medico.CPF) == cpfLimpo);
        }

        // Método para garantir que haja pelo menos um médico cadastrado, criando um padrão se necessário, para assegurar que o sistema tenha dados iniciais para operações de leitura ou validação
        public void GarantirCargaInicial()
        {
            List<Medico> medicos = CarregarMedicosExistentes();

            if (medicos.Count > 0)
            {
                return;
            }

            PersistirMedicos(Medicos.CriarListaInicial());
        }

        // Método auxiliar para carregar os médicos existentes do arquivo JSON, garantindo que tenham IDs válidos e dados normalizados, para assegurar que o sistema possa acessar os dados atuais de forma consistente antes de realizar operações de leitura ou escrita
        private List<Medico> CarregarMedicosExistentes()
        {
            if (!File.Exists(Destino))
            {
                return new List<Medico>();
            }

            string json = File.ReadAllText(Destino);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Medico>();
            }

            List<Medico> medicos =
                JsonSerializer.Deserialize<List<Medico>>(json, _jsonOptions)
                ?? new List<Medico>();

            bool alterado = false;
            int proximoId = 1;

            foreach (Medico medico in medicos)
            {
                int idAnterior = medico.Id;
                string cpfNumericoAnterior = medico.CpfNumerico;
                string emailAnterior = medico.Email;
                string senhaHashAnterior = medico.SenhaHash;

                medico.NormalizarCadastro(proximoId);

                if (medico.Id != idAnterior
                    || medico.CpfNumerico != cpfNumericoAnterior
                    || medico.Email != emailAnterior
                    || medico.SenhaHash != senhaHashAnterior)
                {
                    alterado = true;
                }

                proximoId = Math.Max(proximoId, medico.Id + 1);
            }

            if (alterado)
            {
                PersistirMedicos(medicos);
            }

            return medicos;
        }

        // Método auxiliar para persistir a lista de médicos no arquivo JSON, garantindo que os dados sejam salvos de forma consistente e que o sistema possa manter as informações atualizadas para operações futuras
        private void PersistirMedicos(List<Medico> medicos)
        {
            string? diretorio = Path.GetDirectoryName(Destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            string json = JsonSerializer.Serialize(medicos, _jsonOptions);
            File.WriteAllText(Destino, json);
        }
        
        // Método auxiliar para obter o próximo ID disponível, garantindo que cada médico tenha um identificador único e sequencial, mesmo que haja registros com IDs ausentes ou duplicados, para assegurar a integridade dos dados ao adicionar novos médicos
        private static int ObterProximoId(IEnumerable<int> idsExistentes)
        {
            int maiorId = idsExistentes.DefaultIfEmpty(0).Max();
            return maiorId + 1;
        }

        // Método auxiliar para normalizar o email, removendo espaços em branco, pontos finais e convertendo para maiúsculas, para garantir que a validação de email seja feita de forma consistente, evitando problemas com formatação ou diferenças de maiúsculas e minúsculas
        private static string NormalizarEmail(string email)
        {
            return (email ?? string.Empty).Trim().TrimEnd('.').ToUpperInvariant();
        }

        // Método auxiliar para normalizar o CRM, removendo espaços em branco e convertendo para maiúsculas, para garantir que a validação de CRM seja feita de forma consistente, evitando problemas com formatação ou diferenças de maiúsculas e minúsculas
        private static string NormalizarCrm(string crm)
        {
            return (crm ?? string.Empty).Trim().ToUpperInvariant();
        }

        // Método auxiliar para limpar os dígitos de um valor, removendo todos os caracteres que não são dígitos, para garantir que a validação de CPF seja feita de forma consistente, evitando problemas com formatação ou caracteres adicionais
        private static string LimparDigitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }

        // Método para descobrir o diretório base do projeto, procurando por um arquivo .csproj, para garantir que os dados sejam armazenados em um local consistente
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
}
