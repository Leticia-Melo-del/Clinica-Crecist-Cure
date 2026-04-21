namespace Clinica_Crescit.Cura
{
   
   // Classe para representar o registro de um administrador
    public class AdministradorRegistro
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
    }
}
