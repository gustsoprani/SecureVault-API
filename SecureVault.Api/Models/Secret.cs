namespace SecureVault.Api.Models
{
    public class Secret
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EncryptedValue { get; set; }
    }
}
