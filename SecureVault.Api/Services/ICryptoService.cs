namespace SecureVault.Api.Services
{
    /// <summary>
    /// Serviço responsável pela criptografia e descriptografia de dados sensíveis usando AES-256.
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>
        /// Criptografa uma string em texto claro para o formato Base64.
        /// </summary>
        /// <param name="toEncrypt">O texto original que será ocultado.</param>
        /// <returns>Uma string embaralhada em Base64.</returns>
        string Encrypt(string toEncrypt);

        /// <summary>
        /// Descriptografa uma string em Base64 de volta para o texto original.
        /// </summary>
        /// <param name="toDecrypt">A string cifrada em Base64.</param>
        /// <returns>O texto claro original.</returns>
        string Decrypt(string toDecrypt);
    }
}
