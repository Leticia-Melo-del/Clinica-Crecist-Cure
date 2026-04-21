using System;
using System.Security.Cryptography;
using System.Text;

namespace Clinica_Crescit.Cura
{
    // Serviço responsável por gerar e verificar hashes de senhas, utilizando PBKDF2 para segurança aprimorada e mantendo compatibilidade com hashes legados em SHA-256
    public static class SenhaService
    {
        private const int Iteracoes = 100000;
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const string PrefixoPbkdf2 = "PBKDF2";

        // Método para gerar um hash de senha utilizando PBKDF2, incluindo um salt aleatório e o número de iterações, formatado para fácil armazenamento e verificação
        public static string GerarHash(string senha)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                senha,
                salt,
                Iteracoes,
                HashAlgorithmName.SHA256,
                TamanhoHash);

            return $"{PrefixoPbkdf2}${Iteracoes}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        // Método para verificar se a senha informada corresponde ao hash armazenado, detectando o formato do hash (PBKDF2 ou legado SHA-256) e aplicando a verificação adequada
        public static bool VerificarSenha(string senhaInformada, string senhaHashArmazenada)
        {
            if (string.IsNullOrWhiteSpace(senhaInformada) || string.IsNullOrWhiteSpace(senhaHashArmazenada))
            {
                return false;
            }

            if (senhaHashArmazenada.StartsWith($"{PrefixoPbkdf2}$", StringComparison.OrdinalIgnoreCase))
            {
                return VerificarSenhaPbkdf2(senhaInformada, senhaHashArmazenada);
            }

            return VerificarSenhaLegadaSha256(senhaInformada, senhaHashArmazenada);
        }

        // Método para verificar uma senha utilizando o formato PBKDF2, extraindo o número de iterações, salt e hash esperado do formato armazenado e comparando com o hash gerado a partir da senha informada
        private static bool VerificarSenhaPbkdf2(string senhaInformada, string senhaHashArmazenada)
        {
            string[] partes = senhaHashArmazenada.Split('$');

            if (partes.Length != 4 || !int.TryParse(partes[1], out int iteracoes))
            {
                return false;
            }

            byte[] salt = Convert.FromBase64String(partes[2]);
            byte[] hashEsperado = Convert.FromBase64String(partes[3]);
            byte[] hashInformado = Rfc2898DeriveBytes.Pbkdf2(
                senhaInformada,
                salt,
                iteracoes,
                HashAlgorithmName.SHA256,
                hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashInformado, hashEsperado);
        }

        // Método para verificar uma senha utilizando o formato legado SHA-256, gerando o hash da senha informada e comparando com o hash armazenado, ignorando diferenças de maiúsculas/minúsculas
        private static bool VerificarSenhaLegadaSha256(string senhaInformada, string senhaHashArmazenada)
        {
            byte[] senhaBytes = Encoding.UTF8.GetBytes(senhaInformada);
            byte[] hash = SHA256.HashData(senhaBytes);
            string hashHexadecimal = Convert.ToHexString(hash);

            return string.Equals(hashHexadecimal, senhaHashArmazenada, StringComparison.OrdinalIgnoreCase);
        }
    }
}
