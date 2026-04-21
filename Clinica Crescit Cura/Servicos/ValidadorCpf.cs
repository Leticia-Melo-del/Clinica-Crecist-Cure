using System.Linq;

namespace Clinica_Crescit.Cura
{

    // Validador de CPF
    public static class ValidadorCpf
    {
        public static bool EhValido(string cpf) // Método para validar o CPF
        {
            // Remove caracteres não numéricos do CPF
            string cpfLimpo = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpfLimpo.Length != 11)
            {
                return false;
            }

            if (cpfLimpo.All(digito => digito == cpfLimpo[0]))
            {
                return false;
            }

            // Calcula os dígitos verificadores
            int primeiroDigito = CalcularDigitoVerificador(cpfLimpo, 9); // Calcula o primeiro dígito verificador usando os primeiros 9 dígitos
            int segundoDigito = CalcularDigitoVerificador(cpfLimpo, 10); // Calcula o segundo dígito verificador usando os primeiros 10 dígitos

            return cpfLimpo[9] - '0' == primeiroDigito
                && cpfLimpo[10] - '0' == segundoDigito;
        }

        // Método para calcular o dígito verificador do CPF
        private static int CalcularDigitoVerificador(string cpf, int quantidadeDigitos)
        {
            int soma = 0;
            int peso = quantidadeDigitos + 1;

            for (int i = 0; i < quantidadeDigitos; i++)
            {
                soma += (cpf[i] - '0') * peso;
                peso--;
            }

            int resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }
    }
}
