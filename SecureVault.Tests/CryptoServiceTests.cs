using Moq;
using Microsoft.Extensions.Configuration;
using SecureVault.Api.Services;
using Xunit;

namespace SecureVault.Tests
{
    public class CryptoServiceTests
    {
        [Fact] // Indica que este é um método de teste
        public void Encrypt_And_Decrypt_ShouldReturnOriginalText()
        {
            // 1. Arrange (Organizar)
            var masterKey = "12345678901234567890123456789012"; // 32 caracteres exatos

            // Criamos um "Mock" da configuração para não precisar de um arquivo appsettings real
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["MasterKey"]).Returns(masterKey);

            var service = new CryptoService(mockConfig.Object);
            var originalText = "MinhaSenhaSecreta123";

            // 2. Act (Agir)
            var encrypted = service.Encrypt(originalText);
            var decrypted = service.Decrypt(encrypted);

            // 3. Assert (Afirmar)
            Assert.NotNull(encrypted);
            Assert.NotEqual(originalText, encrypted); // O texto criptografado DEVE ser diferente do original
            Assert.Equal(originalText, decrypted);    // O texto descriptografado DEVE ser igual ao original
        }
    }
}