using System.Collections.Generic;

namespace Clinica_Crescit.Cura
{
    public interface IConsultaRepository // Interface para gerenciamento de consultas médicas
    {
        string Destino { get; }

        void Salvar(Consulta consulta);
        void Atualizar(Consulta consulta);
        List<Consulta> ObterPorPaciente(int pacienteId);
        List<Consulta> ObterPorMedico(int medicoId);
        Consulta? ObterPorId(int id);
        List<Consulta> ObterTodas();
    }
}