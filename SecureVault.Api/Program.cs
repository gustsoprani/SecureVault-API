using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using SecureVault.Api.Data;
using SecureVault.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// SERVIÇOS DO SISTEMA E SEGURANÇA
builder.Services.AddControllers();

// Defesa contra ataques de força bruta e Enumeração (Rate Limiting)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("BloqueioDeBruteForce", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 3;
        opt.QueueLimit = 0;
    });
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