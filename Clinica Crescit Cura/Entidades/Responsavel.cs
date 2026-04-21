using System;

namespace Clinica_Crescit.Cura
{
    // Dados do Responsável
    public class Responsavel : Pessoa
    {
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Senha {get; set;} = string.Empty;

        // Um responsável pode ter mais de um paciente sob seus cuidados (ex: dois filhos).
        // Criamos uma lista para armazenar isso.
        public List<Paciente> Pacientes { get; set; }

        // Construtor
        public Responsavel(string nomeCompleto, string cpf, string telefone, string email, string endereco) 
            : base(nomeCompleto, cpf) // Repassa nome e cpf para a classe base (Pessoa)
        {
            Telefone = telefone;
            Email = email;
            Endereco = endereco;
            Pacientes = new List<Paciente>();
        }

        // Adiciona a senha de forma encapsulada após o cadastro dos dados
        public void CadastrarSenha(string senha)
        {
            Senha = senha;
            // Futuramente, é aqui que você implementaria a criptografia (Hash) antes de salvar no banco!
        }

        // Polimorfismo: Sobrescrevendo o método da classe pai
        public override void ExibirResumo()
        {
            Console.WriteLine("\n--- Dados do Responsável ---");
            base.ExibirResumo(); // Chama o método da classe base
            Console.WriteLine($"Contato: {Telefone} | E-mail: {Email}");
        }
    
    }
}


