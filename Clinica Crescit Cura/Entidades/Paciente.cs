using System;

namespace Clinica_Crescit.Cura
{
    // Dados da Criança/Adolescente
    public class Paciente : Pessoa  // Representa um paciente cadastrado no sistema, com informações pessoais e de saúde
    {
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set;} = string.Empty;

        // Dados Opcionas 
        public string TipoSanguineo { get; set; } = string.Empty;
        public string Alergias { get; set; } = string.Empty;
        public string DoencasPreExistentes { get; set; } = string.Empty;
        public double Peso { get; set; } 
        public double Altura { get; set; }

        public int Idade
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

        // O construtor exige apenas os dados obrigatórios
        public Paciente(string nomeCompleto, string cpf, string genero, DateTime dataNascimento) : base(nomeCompleto, cpf)
        {
            Genero = genero;
            DataNascimento = dataNascimento;
        }

        // O método ExibirResumo é sobrescrito para mostrar as informações específicas do paciente, incluindo idade, gênero e alergias
        public override void ExibirResumo()
        {
            Console.WriteLine("\n--- Dados do Paciente ---");
            base.ExibirResumo();
            Console.WriteLine($"Gênero: {Genero}");
            Console.WriteLine($"Alergias: {(string.IsNullOrEmpty(Alergias) ? "Nenhuma informada" : Alergias)}");
        }
    }
}

