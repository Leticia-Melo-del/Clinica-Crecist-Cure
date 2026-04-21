using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Clinica_Crescit.Cura
{
    public class JsonCadastroRepository : ICadastroRepository
    {
        // Opções de serialização JSON para garantir que o JSON seja formatado de maneira legível
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Construtor que define o caminho do arquivo JSON, usando um caminho padrão se nenhum for fornecido
        public JsonCadastroRepository(string? caminhoArquivo = null)
        {
            Destino = caminhoArquivo ?? Path.Combine(DescobrirDiretorioBase(), "dados", "cadastros.json");
        }

        // Propriedade que retorna o caminho do arquivo JSON onde os cadastros são armazenados
        public string Destino { get; }

        // Método para salvar um novo cadastro
        public void Salvar(CadastroRegistro cadastro)
        {
            string? diretorio = Path.GetDirectoryName(Destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            List<CadastroRegistro> cadastros = CarregarCadastrosExistentes();
            AtribuirIds(cadastro, cadastros);
            cadastros.Add(cadastro);

            string json = JsonSerializer.Serialize(cadastros, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Método para atualizar um cadastro existente, identificando-o pelo ID e garantindo que os IDs dos pacientes sejam atribuídos corretamente
        public void Atualizar(CadastroRegistro cadastro)
        {
            string? diretorio = Path.GetDirectoryName(Destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            List<CadastroRegistro> cadastros = CarregarCadastrosExistentes();
            int indiceCadastro = cadastros.FindIndex(item => item.Id == cadastro.Id);

            if (indiceCadastro < 0)
            {
                throw new InvalidOperationException("Cadastro não encontrado para atualização.");
            }

            CadastroRegistro cadastroExistente = cadastros[indiceCadastro];
            cadastro.Responsavel.Id = cadastroExistente.Responsavel.Id;

            int proximoPacienteId = ObterProximoId(
                cadastros.SelectMany(item => item.Pacientes).Select(paciente => paciente.Id));

            foreach (PacienteRegistro paciente in cadastro.Pacientes)
            {
                if (paciente.Id <= 0)
                {
                    paciente.Id = proximoPacienteId;
                    proximoPacienteId++;
                }

                paciente.ResponsavelId = cadastro.Responsavel.Id;
            }

            cadastros[indiceCadastro] = cadastro;

            string json = JsonSerializer.Serialize(cadastros, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Método para obter um cadastro por email, normalizando o email para comparação
        public CadastroRegistro? ObterCadastroPorEmail(string email)
        {
            string emailNormalizado = NormalizarEmail(email);

            return CarregarCadastrosExistentes()
                .FirstOrDefault(cadastro => NormalizarEmail(cadastro.Responsavel.Email) == emailNormalizado);
        }

        // Método para verificar se um email já está cadastrado, usando o método de obtenção por email
        public bool EmailJaCadastrado(string email)
        {
            return ObterCadastroPorEmail(email) is not null;
        }

        // Método para verificar se um email já está cadastrado por outro cadastro, ignorando um cadastro específico pelo ID, útil para validação durante atualizações
        public bool EmailJaCadastradoPorOutroCadastro(string email, int cadastroIgnoradoId)
        {
            string emailNormalizado = NormalizarEmail(email);

            return CarregarCadastrosExistentes().Any(cadastro =>
                cadastro.Id != cadastroIgnoradoId
                && NormalizarEmail(cadastro.Responsavel.Email) == emailNormalizado);
        }

        // Método para obter todos os cadastros, carregando-os do arquivo JSON
        private List<CadastroRegistro> CarregarCadastrosExistentes()
        {
            if (!File.Exists(Destino))
            {
                return new List<CadastroRegistro>();
            }

            string jsonExistente = File.ReadAllText(Destino);

            if (string.IsNullOrWhiteSpace(jsonExistente))
            {
                return new List<CadastroRegistro>();
            }

            return JsonSerializer.Deserialize<List<CadastroRegistro>>(jsonExistente, _jsonOptions)
                ?? new List<CadastroRegistro>();
        }

        // Método auxiliar para atribuir IDs a um novo cadastro e seus pacientes, garantindo que os IDs sejam únicos e sequenciais
        private static void AtribuirIds(CadastroRegistro novoCadastro, List<CadastroRegistro> cadastrosExistentes)
        {
            novoCadastro.Id = ObterProximoId(cadastrosExistentes.Select(cadastro => cadastro.Id));
            novoCadastro.Responsavel.Id = ObterProximoId(
                cadastrosExistentes.Select(cadastro => cadastro.Responsavel.Id));

            int proximoPacienteId = ObterProximoId(
                cadastrosExistentes.SelectMany(cadastro => cadastro.Pacientes).Select(paciente => paciente.Id));

            foreach (PacienteRegistro paciente in novoCadastro.Pacientes)
            {
                paciente.Id = proximoPacienteId;
                paciente.ResponsavelId = novoCadastro.Responsavel.Id;
                proximoPacienteId++;
            }
        }

        // Método auxiliar para obter o próximo ID disponível, encontrando o maior ID existente e retornando o próximo valor
        private static int ObterProximoId(IEnumerable<int> idsExistentes)
        {
            int maiorId = idsExistentes.DefaultIfEmpty(0).Max();
            return maiorId + 1;
        }

        // Método para normalizar o email para comparação, removendo espaços e convertendo para maiúsculas, garantindo que a comparação de emails seja consistente
        private static string NormalizarEmail(string email)
        {
            return (email ?? string.Empty).Trim().ToUpperInvariant();
        }

        // Método para normalizar o email para armazenamento, removendo espaços e pontos finais, mas mantendo a capitalização original, garantindo que os emails sejam armazenados de maneira consistente
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
    
        // Método para obter todos os cadastros, retornando uma lista de cadastros carregados do arquivo JSON
        public List<CadastroRegistro> ObterTodos()
        {
            return CarregarCadastrosExistentes();
        }
        
        // Método para salvar todos os cadastros, sobrescrevendo o arquivo JSON com a lista fornecida, útil para operações de exclusão e atualização em massa
        public void SalvarTodos(List<CadastroRegistro> cadastros)
        {
            string json = JsonSerializer.Serialize(cadastros, _jsonOptions);
            File.WriteAllText(Destino, json);
        }

        // Método para excluir um cadastro por ID, removendo-o da lista de cadastros e salvando a lista atualizada no arquivo JSON
        public void Excluir(int id)
        {
            var cadastros = ObterTodos();
            var cadastroParaRemover = cadastros.FirstOrDefault(c => c.Id == id);
            
            if (cadastroParaRemover != null)
            {
                cadastros.Remove(cadastroParaRemover);
                SalvarTodos(cadastros);
            }
        }

    }
}
