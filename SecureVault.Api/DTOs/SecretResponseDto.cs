namespace SecureVault.Api.DTOs
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) utilizado para enviar respostas de criação de segredos.
    /// Impede que o usuário interaja diretamente com a Entidade de banco de dados.
    /// </summary>
    public class SecretResponseDto
    {
        /// <summary>
        /// Mensagem descritiva sobre o resultado da operação (Ex: "Segredo salvo com sucesso").
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Nome de identificação do segredo (Ex: Senha_Banco, API_KEY).
        /// </summary>
        public required string SecretName { get; set; }
    }
}
