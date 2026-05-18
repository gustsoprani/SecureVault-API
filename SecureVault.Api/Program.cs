using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecureVault.Api.Data;
using SecureVault.Api.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// SERVIÇOS DO SISTEMA E SEGURANÇA
builder.Services.AddControllers();

// Defesa contra ataques de força bruta e Enumeração (Rate Limiting)
builder.Services.AddRateLimiter(options =>
{
    // Política chamada "BloqueioPorIP" para bloquear apenas quem está causando o problema
    options.AddPolicy("BloqueioPorIP", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            // A "chave" da partição é o IP de quem está fazendo a requisição.
            // Se o IP for nulo, usamos "desconhecido" como fallback.
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",

            // Configura as regras que já conhecemos para essa partição
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3, // Quantas requisições?
                Window = TimeSpan.FromSeconds(10), // Em quanto tempo?
                QueueLimit = 0, // Passou de 3, falha na hora.
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
    options.RejectionStatusCode = 429;
});

// INFRAESTRUTURA E BANCO DE DADOS
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICryptoService, CryptoService>();

// DOCUMENTAÇÃO (SWAGGER)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// 4. PIPELINE DE REQUISIÇÕES
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//O limitador de taxa deve agir antes de chegar aos Controllers
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();