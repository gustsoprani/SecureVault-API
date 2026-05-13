namespace SecureVault.Api.DTOs
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) utilizado para receber requisições de criação de segredos.
    /// Impede que o usuário interaja diretamente com a Entidade de banco de dados.
    /// </summary>
    public class CreateSecretRequest
    {
        /// <summary>
        /// Nome de identificação do segredo (Ex: Senha_Banco, API_KEY).
        /// </summary>
        public string SecretName { get; set; }

        /// <summary>
        /// O valor original e sensível em texto claro que será ocultado pela API.
        /// </summary>
        public string RawValue { get; set; }
    }
}
