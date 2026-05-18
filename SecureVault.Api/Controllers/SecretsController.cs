using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecureVault.Api.Data;
using SecureVault.Api.DTOs;
using SecureVault.Api.Models;
using SecureVault.Api.Services;
using System;

namespace SecureVault.Api.Controllers
{
    /// <summary>
    /// Controlador principal responsável por orquestrar a entrada e saída de segredos.
    /// Atua como a porta de comunicação RESTful para a aplicação.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("BloqueioPorIP")]
    public class SecretsController : ControllerBase
    {
        // Crio as variáveis "trancadas" para guardar minhas ferramentas
        private readonly ICryptoService _cryptoService;
        private readonly AppDbContext _context;

        /// <summary>
        /// Construtor que injeta as dependências necessárias para o Controller funcionar.
        /// </summary>
        /// <param name="cryptoService">Serviço de criptografia AES-256.</param>
        /// <param name="context">Contexto de acesso ao banco de dados PostgreSQL.</param>
        public SecretsController(ICryptoService cryptoService, AppDbContext context)
        {
            _cryptoService = cryptoService;
            _context = context;
        }

        /// <summary>
        /// Recebe um texto claro, criptografa e armazena de forma segura no banco de dados.
        /// </summary>
        /// <param name="request">O DTO contendo o nome e o valor a ser protegido.</param>
        /// <returns>Retorna status 200 (OK) e o ID do segredo gerado no banco.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSecret([FromBody] CreateSecretRequest request)
        {
            string encryptedValue = _cryptoService.Encrypt(request.RawValue);
            Secret segredo = new Secret
            {
                Name = request.SecretName,
                EncryptedValue = encryptedValue
            };
            _context.Secrets.Add(segredo);
            await _context.SaveChangesAsync();
            return Ok(new SecretResponseDto { Message = "Segredo salvo com segurança", SecretName = request.SecretName });
        }

        /// <summary>
        /// Busca um segredo pelo nome e o devolve descriptografado em texto claro.
        /// </summary>
        /// <param name="name">O nome do segredo conforme salvo no banco de dados.</param>
        /// <returns>Retorna status 200 com o dado limpo, ou 404 se não for encontrado.</returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> GetSecret(string name)
        {
            // Isso vai no banco e procura. Se não achar, a variável 'segredo' fica com valor 'null'.
            var segredo = await _context.Secrets.FirstOrDefaultAsync(s => s.Name == name);
            if (segredo == null)
                return NotFound(new { Mensagem = "Segredo não encontrado." });
            string decryptedValue = _cryptoService.Decrypt(segredo.EncryptedValue);
            return Ok(new { Mensagem = $"Segredo descriptografado com sucesso!", Name = segredo.Name, value = decryptedValue });
        }
    }
}
