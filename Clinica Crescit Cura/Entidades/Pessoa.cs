using System;

namespace Clinica_Crescit.Cura
{
    // Dados Comuns a Paciente, Responsável, Médico e Administrador.
    public abstract class Pessoa
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        
        // Construtor
        public Pessoa(string nomeCompleto, string cpf)
        {
            NomeCompleto = nomeCompleto;
            CPF = cpf;
        }

        // Método virtual para demonstrar Polimorfismo. 
        // As classes filhas poderão alterar (override) o comportamento deste método.
        public virtual void ExibirResumo()
        {
            Console.WriteLine($"Nome: {NomeCompleto} | CPF: {CPF}");
        }
    }
}