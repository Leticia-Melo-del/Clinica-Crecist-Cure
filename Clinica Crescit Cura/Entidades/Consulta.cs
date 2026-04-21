using System;

namespace Clinica_Crescit.Cura
{
    public class Consulta // Representa uma consulta médica
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Valor { get; set; } = 250.00m; // Valor padrão da consulta
        public string Status { get; set; } = "Agendada"; // Agendada, Realizada, Cancelada
        public string Observacoes { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public bool FoiPago { get; set; }
    }
}