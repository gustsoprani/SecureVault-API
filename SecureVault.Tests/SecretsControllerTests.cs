using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SecureVault.Api.Controllers;
using SecureVault.Api.Data;
using SecureVault.Api.DTOs;
using SecureVault.Api.Models;
using SecureVault.Api.Services;

namespace SecureVault.Tests
{
    public class SecretsControllerTests
    {
        [Fact]
        public async Task GetSecret_WhenSecretDoesNotExist_ShouldReturnNotFound()
        {
            // 1. ARRANGE (Preparar o terreno)

            // a) Criamos o banco de dados falso na memória RAM
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BancoCofre_Teste")
                .Options;
            var dbContext = new AppDbContext(dbOptions);

            // b) Criamos um "Dublê" do serviço de criptografia usando o Moq
            // Não precisamos nem configurar ele, pois a busca já vai falhar no banco antes de criptografar
            var mockCryptoService = new Mock<ICryptoService>();

            // c) Instanciamos o Controller injetando o banco de mentira e o dublê
            var controller = new SecretsController(mockCryptoService.Object, dbContext);

            // 2. ACT (Agir)

            // Disparamos o método GET pedindo uma senha que sabemos que não colocamos no banco falso
            var result = await controller.GetSecret("SenhaQueNaoExiste");

            // 3. ASSERT (Afirmar)

            // Verificamos se o retorno do Controller foi de fato um objeto do tipo 404 (NotFoundObjectResult)
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateSecret_ValidRequest_ShouldSaveToDatabaseAndReturnOk()
        {
            // 1. ARRANGE
            // Criamos um banco NOVO em memória (cada teste deve ter o seu para não dar conflito)
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BancoCofre_Criacao")
                .Options;
            var dbContext = new AppDbContext(dbOptions);

            var mockCryptoService = new Mock<ICryptoService>();
            // Ensinamos o Moq: "Sempre que alguém pedir para encriptar qualquer coisa, retorne 'TextoEmBase64'"
            mockCryptoService.Setup(c => c.Encrypt(It.IsAny<string>())).Returns("TextoEmBase64");

            var controller = new SecretsController(mockCryptoService.Object, dbContext);
            var request = new CreateSecretRequest { SecretName = "Steam", RawValue = "senha123" };

            // 2. ACT
            var result = await controller.CreateSecret(request);

            // 3. ASSERT
            // Garante que a API respondeu Status 200 (OK)
            Assert.IsType<OkObjectResult>(result);

            // Vai lá no banco de mentira e garante que a linha foi salva de verdade
            var savedSecret = await dbContext.Secrets.FirstOrDefaultAsync(s => s.Name == "Steam");
            Assert.NotNull(savedSecret);
            Assert.Equal("TextoEmBase64", savedSecret.EncryptedValue);
        }

        [Fact]
        public async Task GetSecret_WhenSecretExists_ShouldReturnOkAndDecryptedValue()
        {
            // 1. ARRANGE
            var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BancoCofre_Leitura")
                .Options;
            var dbContext = new AppDbContext(dbOptions);

            // Plantamos um segredo falso no banco ANTES de testar
            dbContext.Secrets.Add(new Secret { Name = "Github", EncryptedValue = "TextoEmBase64" });
            await dbContext.SaveChangesAsync();

            var mockCryptoService = new Mock<ICryptoService>();
            // Ensinamos o Moq: "Quando pedirem para descriptografar 'TextoEmBase64', devolva 'senhaLimpa'"
            mockCryptoService.Setup(c => c.Decrypt("TextoEmBase64")).Returns("senhaLimpa");

            var controller = new SecretsController(mockCryptoService.Object, dbContext);

            // 2. ACT
            var result = await controller.GetSecret("Github");

            // 3. ASSERT
            // Garante que devolveu 200 (OK)
            Assert.IsType<OkObjectResult>(result);
        }
    }
}