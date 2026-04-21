using System;
using System.Collections.Generic;
using System.Linq;

namespace Clinica_Crescit.Cura
{
    public class CadastroRegistro // Classe para representar o registro de um cadastro, incluindo o responsável e seus pacientes
    {
        public int Id { get; set; }
        public DateTime DataCadastroUtc { get; init; } = DateTime.UtcNow;
        public ResponsavelRegistro Responsavel { get; init; } = new ResponsavelRegistro();
        public List<PacienteRegistro> Pacientes { get; init; } = new List<PacienteRegistro>();

        public static CadastroRegistro CriarDe(Responsavel responsavel, string senhaEmTexto)
        {
            return new CadastroRegistro // Cria um novo registro de cadastro a partir dos dados do responsável e da senha em texto
            {
                Responsavel = new ResponsavelRegistro
                {
                    NomeCompleto = responsavel.NomeCompleto,
                    Cpf = responsavel.CPF,
                    CpfNumerico = LimparDigitos(responsavel.CPF),
                    Telefone = responsavel.Telefone,
                    TelefoneNumerico = LimparDigitos(responsavel.Telefone),
                    Email = responsavel.Email,
                    Endereco = responsavel.Endereco,
                    SenhaHash = SenhaService.GerarHash(senhaEmTexto)
                },
                Pacientes = responsavel.Pacientes
                    .Select(paciente => new PacienteRegistro
                    {
                        NomeCompleto = paciente.NomeCompleto,
                        Cpf = paciente.CPF,
                        CpfNumerico = LimparDigitos(paciente.CPF),
                        Genero = paciente.Genero,
                        DataNascimento = paciente.DataNascimento,
                        TipoSanguineo = paciente.TipoSanguineo,
                        Alergias = paciente.Alergias,
                        DoencasPreExistentes = paciente.DoencasPreExistentes,
                        Peso = paciente.Peso,
                        Altura = paciente.Altura
                    })
                    .ToList()
            };
        }

        private static string LimparDigitos(string valor) // Remove todos os caracteres que não são dígitos de uma string, útil para CPF e telefone
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }

    public class ResponsavelRegistro // Classe para representar o registro de um responsável, que é a pessoa que faz o cadastro e pode ter vários pacientes associados
    {
        public int Id { get; set; }
        public string NomeCompleto { get; init; } = string.Empty;
        public string Cpf { get; init; } = string.Empty;
        public string CpfNumerico { get; init; } = string.Empty;
        public string Telefone { get; init; } = string.Empty;
        public string TelefoneNumerico { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Endereco { get; init; } = string.Empty;
        public string SenhaHash { get; init; } = string.Empty;
    }

    public class PacienteRegistro // Classe para representar o registro de um paciente, que é associado a um responsável e contém informações pessoais e de saúde
    {
        public int Id { get; set; }
        public int ResponsavelId { get; set; }
        public string NomeCompleto { get; init; } = string.Empty;
        public string Cpf { get; init; } = string.Empty;
        public string CpfNumerico { get; init; } = string.Empty;
        public string Genero { get; init; } = string.Empty;
        public DateTime DataNascimento { get; init; }
        public string TipoSanguineo { get; init; } = string.Empty;
        public string Alergias { get; init; } = string.Empty;
        public string DoencasPreExistentes { get; init; } = string.Empty;
        public double Peso { get; init; }
        public double Altura { get; init; }

        public int Idade // Propriedade calculada para obter a idade do paciente com base na data de nascimento
        {
            get
            {
                int anos = DateTime.Today.Year - DataNascimento.Year;
                // Subtrai um ano se o paciente ainda não fez anos este ano
                if (DataNascimento.Date > DateTime.Today.AddYears(-anos)) 
                {
                    anos--;
                }
                return anos;
            }
        }
    }
}
