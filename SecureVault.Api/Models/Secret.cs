namespace SecureVault.Api.Models
{
    /// <summary>
    /// Representa um segredo (credencial, token, API Key) armazenado no cofre da aplicação.
    /// </summary>
    public class Secret
    {
        /// <summary>
        /// identificador único do segredo no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de identificação do segredo (ex: "API_KEY", "DB_PASSWORD").
        /// Este valor é salvo em texto claro para permitir buscas.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// O valor real do segredo, criptografado em AES-256 e convertido para Base64.
        /// Nunca deve ser exposto ou trafegado em texto claro.
        /// </summary>
        public required string EncryptedValue { get; set; }
    }
}
