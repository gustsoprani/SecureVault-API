using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SecureVault.Api.Services
{
    /// <summary>
    /// Classe que de fato efetua a criptografia e decriptografia de dados sensíveis.
    /// </summary>
    public class CryptoService : ICryptoService
    {
        private readonly string secretValue;
        /// <summary>
        /// Injeta as configurações da aplicação para resgatar a chave mestra.
        /// </summary>
        /// <param name="config">Parametro que pega o texto das variáveis do sistema</param>
        public CryptoService(IConfiguration config) 
        {
            secretValue = config["MasterKey"];
        }
        public string Encrypt(string toEncrypt)
        {
            // Converte a chave mestra em bytes. O AES-256 exige uma chave de exatos 32 bytes.
            byte[] key = Encoding.UTF8.GetBytes(secretValue);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                // Para este MVP, utilizamos um IV (Vetor de Inicialização) fixo de 16 bytes zerados.
                // Em um ambiente de alta segurança em produção, este IV deveria ser aleatório e salvo junto ao banco.
                aes.IV = new byte[16];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Cria a linha de montagem: Memória -> Criptografia -> Escrita
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(toEncrypt);
                        } // Ao fechar este bloco, o StreamWriter garante que todos os dados foram processados no fluxo.
                    }
                    // Retorna o resultado final de bytes transformando-o em uma string segura para transporte/banco (Base64)
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        public string Decrypt(string toDecrypt)
        {
            // Converte a chave mestra em bytes. O AES-256 exige uma chave de exatos 32 bytes.
            byte[] key = Encoding.UTF8.GetBytes(secretValue);
            // Converte o dado cifrado em bytes
            byte[] cipherBytes = Convert.FromBase64String(toDecrypt);
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                // Para este MVP, utilizamos um IV (Vetor de Inicialização) fixo de 16 bytes zerados.
                // Em um ambiente de alta segurança em produção, este IV deveria ser aleatório e salvo junto ao banco.
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Cria a linha de montagem: Memória -> Decriptografia -> Leitura
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string decrypted = srDecrypt.ReadToEnd();
                            // Retorna o resultado final do texto limpo e decriptografado
                            return decrypted;
                        }
                    }
                }
            }
        }
    }
}
