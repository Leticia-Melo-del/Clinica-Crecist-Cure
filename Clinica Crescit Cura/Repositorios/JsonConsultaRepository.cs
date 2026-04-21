using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Clinica_Crescit.Cura
{
    // Implementação do repositório de consultas usando arquivos JSON
    public class JsonConsultaRepository : IConsultaRepository
    {
        // Configurações para formatação do JSON, garantindo que seja legível e consistente
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Construtor que define o caminho do arquivo JSON, usando um caminho padrão se nenhum for fornecido
        public JsonConsultaRepository(string? caminhoArquivo = null)
        {
            Destino = caminhoArquivo ?? Path.Combine(DescobrirDiretorioBase(), "dados", "consultas.json");
        }

        public string Destino { get; }

        // Método para salvar um novo registro de consulta, atribuindo um ID único e persistindo no arquivo JSON
        public void Salvar(Consulta consulta)
        {
            List<Consulta> consultas = CarregarConsultasExistentes();
            consulta.Id = ObterProximoId(consultas.Select(c => c.Id));
            consultas.Add(consulta);

            string json = JsonSerializer.Serialize(consultas, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Método para atualizar um registro de consulta existente, identificando-o pelo ID e garantindo que as alterações sejam persistidas no arquivo JSON
        public void Atualizar(Consulta consulta)
        {
            List<Consulta> consultas = CarregarConsultasExistentes();
            int indice = consultas.FindIndex(c => c.Id == consulta.Id);

            if (indice < 0)
            {
                throw new InvalidOperationException("Consulta não encontrada para atualização.");
            }

            consultas[indice] = consulta;

            string json = JsonSerializer.Serialize(consultas, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Método para obter todas as consultas associadas a um paciente específico, filtrando por pacienteId
        public List<Consulta> ObterPorPaciente(int pacienteId)
        {
            return CarregarConsultasExistentes().Where(c => c.PacienteId == pacienteId).ToList();
        }

        // Método para obter todas as consultas associadas a um médico específico, filtrando por medicoId
        public List<Consulta> ObterPorMedico(int medicoId)
        {
            return CarregarConsultasExistentes().Where(c => c.MedicoId == medicoId).ToList();
        }

        // Método para obter uma consulta específica pelo seu ID, retornando null se não for encontrada
        public Consulta? ObterPorId(int id)
        {
            return CarregarConsultasExistentes().FirstOrDefault(c => c.Id == id);
        }

        // Método para obter todas as consultas registradas, carregando-as do arquivo JSON
        public List<Consulta> ObterTodas()
        {
            return CarregarConsultasExistentes();
        }

        // Método auxiliar para carregar as consultas existentes do arquivo JSON, garantindo que o sistema possa ler os dados atuais antes de realizar operações de leitura ou escrita
        private List<Consulta> CarregarConsultasExistentes()
        {
            if (!File.Exists(Destino))
            {
                return new List<Consulta>();
            }

            string json = File.ReadAllText(Destino);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Consulta>();
            }

            return JsonSerializer.Deserialize<List<Consulta>>(json, _jsonOptions) ?? new List<Consulta>();
        }

        // Método auxiliar para obter o próximo ID disponível, garantindo que cada consulta tenha um identificador único e sequencial
        private static int ObterProximoId(IEnumerable<int> idsExistentes)
        {
            return idsExistentes.DefaultIfEmpty(0).Max() + 1;
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