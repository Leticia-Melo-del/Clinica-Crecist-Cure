namespace Clinica_Crescit.Cura
{
    public interface ICadastroRepository // Interface para gerenciamento de cadastros de pacientes
    {
        string Destino { get; }

        void Salvar(CadastroRegistro cadastro);
        void Atualizar(CadastroRegistro cadastro);
        CadastroRegistro? ObterCadastroPorEmail(string email);
        bool EmailJaCadastrado(string email);
        bool EmailJaCadastradoPorOutroCadastro(string email, int cadastroIgnoradoId);
        List<CadastroRegistro> ObterTodos();
        void Excluir(int id);
    }

}
