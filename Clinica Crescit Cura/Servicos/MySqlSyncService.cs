using System.Globalization;
using System.Text;

namespace Clinica_Crescit.Cura
{
    /// <summary>
    /// Ponte única para exportar os dados atuais em JSON para um script compatível com MySQL.
    /// A ideia é facilitar a migração futura sem alterar a persistência atual do projeto.
    ///
    /// Exemplo de uso:
    /// var sync = new MySqlSyncService(cadastroRepo, medicoRepo, administradorRepo, consultaRepo);
    /// string caminho = sync.ExportarScript();
    /// </summary>
    public class MySqlSyncService
    {
        private readonly ICadastroRepository _cadastroRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IAdministradorRepository _administradorRepository;
        private readonly IConsultaRepository _consultaRepository;

        public MySqlSyncService(
            ICadastroRepository cadastroRepository,
            IMedicoRepository medicoRepository,
            IAdministradorRepository administradorRepository,
            IConsultaRepository consultaRepository)
        {
            _cadastroRepository = cadastroRepository;
            _medicoRepository = medicoRepository;
            _administradorRepository = administradorRepository;
            _consultaRepository = consultaRepository;
        }

        public string ExportarScript(string? caminhoArquivo = null)
        {
            string destino = caminhoArquivo
                ?? Path.Combine(DescobrirDiretorioBase(), "Dados", "mysql-sync.sql");

            string? diretorio = Path.GetDirectoryName(destino);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            File.WriteAllText(destino, GerarScriptCompleto(), new UTF8Encoding(false));
            return destino;
        }

        public string GerarScriptCompleto()
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("-- Script gerado automaticamente para migração/sincronização com MySQL");
            sql.AppendLine($"-- Gerado em: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sql.AppendLine("-- Observação: selecione o banco antes de executar este arquivo.");
            sql.AppendLine();
            sql.AppendLine("SET NAMES utf8mb4;");
            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 0;");
            sql.AppendLine();

            AppendSchema(sql);
            AppendAdministradores(sql);
            AppendMedicos(sql);
            AppendResponsaveis(sql);
            AppendPacientes(sql);
            AppendConsultas(sql);

            sql.AppendLine("SET FOREIGN_KEY_CHECKS = 1;");

            return sql.ToString();
        }

        private void AppendSchema(StringBuilder sql)
        {
            sql.AppendLine("CREATE TABLE IF NOT EXISTS administradores (");
            sql.AppendLine("    id INT NOT NULL PRIMARY KEY,");
            sql.AppendLine("    nome_completo VARCHAR(150) NOT NULL,");
            sql.AppendLine("    email VARCHAR(150) NOT NULL UNIQUE,");
            sql.AppendLine("    senha_hash VARCHAR(255) NOT NULL");
            sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            sql.AppendLine();

            sql.AppendLine("CREATE TABLE IF NOT EXISTS medicos (");
            sql.AppendLine("    id INT NOT NULL PRIMARY KEY,");
            sql.AppendLine("    nome_completo VARCHAR(150) NOT NULL,");
            sql.AppendLine("    cpf VARCHAR(20) NOT NULL,");
            sql.AppendLine("    cpf_numerico CHAR(11) NOT NULL UNIQUE,");
            sql.AppendLine("    crm VARCHAR(30) NOT NULL UNIQUE,");
            sql.AppendLine("    especialidade VARCHAR(120) NOT NULL,");
            sql.AppendLine("    email VARCHAR(150) NOT NULL UNIQUE,");
            sql.AppendLine("    senha_hash VARCHAR(255) NOT NULL");
            sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            sql.AppendLine();

            sql.AppendLine("CREATE TABLE IF NOT EXISTS responsaveis (");
            sql.AppendLine("    id INT NOT NULL PRIMARY KEY,");
            sql.AppendLine("    cadastro_id INT NOT NULL UNIQUE,");
            sql.AppendLine("    nome_completo VARCHAR(150) NOT NULL,");
            sql.AppendLine("    cpf VARCHAR(20) NOT NULL,");
            sql.AppendLine("    cpf_numerico CHAR(11) NOT NULL UNIQUE,");
            sql.AppendLine("    telefone VARCHAR(25) NOT NULL,");
            sql.AppendLine("    telefone_numerico VARCHAR(20) NOT NULL,");
            sql.AppendLine("    email VARCHAR(150) NOT NULL UNIQUE,");
            sql.AppendLine("    endereco VARCHAR(255) NOT NULL,");
            sql.AppendLine("    senha_hash VARCHAR(255) NOT NULL,");
            sql.AppendLine("    data_cadastro_utc DATETIME NOT NULL");
            sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            sql.AppendLine();

            sql.AppendLine("CREATE TABLE IF NOT EXISTS pacientes (");
            sql.AppendLine("    id INT NOT NULL PRIMARY KEY,");
            sql.AppendLine("    responsavel_id INT NOT NULL,");
            sql.AppendLine("    nome_completo VARCHAR(150) NOT NULL,");
            sql.AppendLine("    cpf VARCHAR(20) NOT NULL,");
            sql.AppendLine("    cpf_numerico CHAR(11) NOT NULL UNIQUE,");
            sql.AppendLine("    genero VARCHAR(60) NOT NULL,");
            sql.AppendLine("    data_nascimento DATETIME NOT NULL,");
            sql.AppendLine("    tipo_sanguineo VARCHAR(10) NULL,");
            sql.AppendLine("    alergias TEXT NULL,");
            sql.AppendLine("    doencas_preexistentes TEXT NULL,");
            sql.AppendLine("    peso DOUBLE NULL,");
            sql.AppendLine("    altura DOUBLE NULL,");
            sql.AppendLine("    CONSTRAINT fk_pacientes_responsavel");
            sql.AppendLine("        FOREIGN KEY (responsavel_id) REFERENCES responsaveis(id)");
            sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            sql.AppendLine();

            sql.AppendLine("CREATE TABLE IF NOT EXISTS consultas (");
            sql.AppendLine("    id INT NOT NULL PRIMARY KEY,");
            sql.AppendLine("    paciente_id INT NOT NULL,");
            sql.AppendLine("    medico_id INT NOT NULL,");
            sql.AppendLine("    data_hora DATETIME NOT NULL,");
            sql.AppendLine("    valor DECIMAL(10,2) NOT NULL,");
            sql.AppendLine("    status VARCHAR(40) NOT NULL,");
            sql.AppendLine("    observacoes TEXT NULL,");
            sql.AppendLine("    diagnostico TEXT NULL,");
            sql.AppendLine("    foi_pago TINYINT(1) NOT NULL,");
            sql.AppendLine("    CONSTRAINT fk_consultas_paciente");
            sql.AppendLine("        FOREIGN KEY (paciente_id) REFERENCES pacientes(id),");
            sql.AppendLine("    CONSTRAINT fk_consultas_medico");
            sql.AppendLine("        FOREIGN KEY (medico_id) REFERENCES medicos(id)");
            sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            sql.AppendLine();
        }

        private void AppendAdministradores(StringBuilder sql)
        {
            IReadOnlyList<AdministradorRegistro> administradores = _administradorRepository.ObterTodos();

            if (administradores.Count == 0)
            {
                return;
            }

            sql.AppendLine("-- Administradores");

            foreach (AdministradorRegistro administrador in administradores)
            {
                sql.AppendLine(
                    "INSERT INTO administradores (id, nome_completo, email, senha_hash) VALUES " +
                    $"({administrador.Id}, {SqlString(administrador.NomeCompleto)}, {SqlString(administrador.Email)}, {SqlString(administrador.SenhaHash)}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    "nome_completo = VALUES(nome_completo), " +
                    "email = VALUES(email), " +
                    "senha_hash = VALUES(senha_hash);");
            }

            sql.AppendLine();
        }

        private void AppendMedicos(StringBuilder sql)
        {
            IReadOnlyList<Medico> medicos = _medicoRepository.ObterTodos();

            if (medicos.Count == 0)
            {
                return;
            }

            sql.AppendLine("-- Médicos");

            foreach (Medico medico in medicos)
            {
                sql.AppendLine(
                    "INSERT INTO medicos (id, nome_completo, cpf, cpf_numerico, crm, especialidade, email, senha_hash) VALUES " +
                    $"({medico.Id}, {SqlString(medico.NomeCompleto)}, {SqlString(medico.CPF)}, {SqlString(medico.CpfNumerico)}, {SqlString(medico.Crm)}, {SqlString(medico.Especialidade)}, {SqlString(medico.Email)}, {SqlString(medico.SenhaHash)}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    "nome_completo = VALUES(nome_completo), " +
                    "cpf = VALUES(cpf), " +
                    "cpf_numerico = VALUES(cpf_numerico), " +
                    "crm = VALUES(crm), " +
                    "especialidade = VALUES(especialidade), " +
                    "email = VALUES(email), " +
                    "senha_hash = VALUES(senha_hash);");
            }

            sql.AppendLine();
        }

        private void AppendResponsaveis(StringBuilder sql)
        {
            List<CadastroRegistro> cadastros = _cadastroRepository.ObterTodos();

            if (cadastros.Count == 0)
            {
                return;
            }

            sql.AppendLine("-- Responsáveis");

            foreach (CadastroRegistro cadastro in cadastros)
            {
                ResponsavelRegistro responsavel = cadastro.Responsavel;

                sql.AppendLine(
                    "INSERT INTO responsaveis (id, cadastro_id, nome_completo, cpf, cpf_numerico, telefone, telefone_numerico, email, endereco, senha_hash, data_cadastro_utc) VALUES " +
                    $"({responsavel.Id}, {cadastro.Id}, {SqlString(responsavel.NomeCompleto)}, {SqlString(responsavel.Cpf)}, {SqlString(responsavel.CpfNumerico)}, {SqlString(responsavel.Telefone)}, {SqlString(responsavel.TelefoneNumerico)}, {SqlString(responsavel.Email)}, {SqlString(responsavel.Endereco)}, {SqlString(responsavel.SenhaHash)}, {SqlDateTime(cadastro.DataCadastroUtc)}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    "cadastro_id = VALUES(cadastro_id), " +
                    "nome_completo = VALUES(nome_completo), " +
                    "cpf = VALUES(cpf), " +
                    "cpf_numerico = VALUES(cpf_numerico), " +
                    "telefone = VALUES(telefone), " +
                    "telefone_numerico = VALUES(telefone_numerico), " +
                    "email = VALUES(email), " +
                    "endereco = VALUES(endereco), " +
                    "senha_hash = VALUES(senha_hash), " +
                    "data_cadastro_utc = VALUES(data_cadastro_utc);");
            }

            sql.AppendLine();
        }

        private void AppendPacientes(StringBuilder sql)
        {
            List<CadastroRegistro> cadastros = _cadastroRepository.ObterTodos();
            List<PacienteRegistro> pacientes = cadastros.SelectMany(cadastro => cadastro.Pacientes).ToList();

            if (pacientes.Count == 0)
            {
                return;
            }

            sql.AppendLine("-- Pacientes");

            foreach (PacienteRegistro paciente in pacientes)
            {
                sql.AppendLine(
                    "INSERT INTO pacientes (id, responsavel_id, nome_completo, cpf, cpf_numerico, genero, data_nascimento, tipo_sanguineo, alergias, doencas_preexistentes, peso, altura) VALUES " +
                    $"({paciente.Id}, {paciente.ResponsavelId}, {SqlString(paciente.NomeCompleto)}, {SqlString(paciente.Cpf)}, {SqlString(paciente.CpfNumerico)}, {SqlString(paciente.Genero)}, {SqlDateTime(paciente.DataNascimento)}, {SqlNullableString(paciente.TipoSanguineo)}, {SqlNullableString(paciente.Alergias)}, {SqlNullableString(paciente.DoencasPreExistentes)}, {SqlNullableDouble(paciente.Peso)}, {SqlNullableDouble(paciente.Altura)}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    "responsavel_id = VALUES(responsavel_id), " +
                    "nome_completo = VALUES(nome_completo), " +
                    "cpf = VALUES(cpf), " +
                    "cpf_numerico = VALUES(cpf_numerico), " +
                    "genero = VALUES(genero), " +
                    "data_nascimento = VALUES(data_nascimento), " +
                    "tipo_sanguineo = VALUES(tipo_sanguineo), " +
                    "alergias = VALUES(alergias), " +
                    "doencas_preexistentes = VALUES(doencas_preexistentes), " +
                    "peso = VALUES(peso), " +
                    "altura = VALUES(altura);");
            }

            sql.AppendLine();
        }

        private void AppendConsultas(StringBuilder sql)
        {
            List<Consulta> consultas = _consultaRepository.ObterTodas();

            if (consultas.Count == 0)
            {
                return;
            }

            sql.AppendLine("-- Consultas");

            foreach (Consulta consulta in consultas)
            {
                sql.AppendLine(
                    "INSERT INTO consultas (id, paciente_id, medico_id, data_hora, valor, status, observacoes, diagnostico, foi_pago) VALUES " +
                    $"({consulta.Id}, {consulta.PacienteId}, {consulta.MedicoId}, {SqlDateTime(consulta.DataHora)}, {SqlDecimal(consulta.Valor)}, {SqlString(consulta.Status)}, {SqlNullableString(consulta.Observacoes)}, {SqlNullableString(consulta.Diagnostico)}, {SqlBool(consulta.FoiPago)}) " +
                    "ON DUPLICATE KEY UPDATE " +
                    "paciente_id = VALUES(paciente_id), " +
                    "medico_id = VALUES(medico_id), " +
                    "data_hora = VALUES(data_hora), " +
                    "valor = VALUES(valor), " +
                    "status = VALUES(status), " +
                    "observacoes = VALUES(observacoes), " +
                    "diagnostico = VALUES(diagnostico), " +
                    "foi_pago = VALUES(foi_pago);");
            }

            sql.AppendLine();
        }

        private static string SqlString(string valor)
        {
            string texto = (valor ?? string.Empty).Replace("\\", "\\\\").Replace("'", "''");
            return $"'{texto}'";
        }

        private static string SqlNullableString(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "NULL" : SqlString(valor);
        }

        private static string SqlDateTime(DateTime data)
        {
            return $"'{data:yyyy-MM-dd HH:mm:ss}'";
        }

        private static string SqlDecimal(decimal valor)
        {
            return valor.ToString("0.00", CultureInfo.InvariantCulture);
        }

        private static string SqlNullableDouble(double valor)
        {
            return valor <= 0
                ? "NULL"
                : valor.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static string SqlBool(bool valor)
        {
            return valor ? "1" : "0";
        }

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
