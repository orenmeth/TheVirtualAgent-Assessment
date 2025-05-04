using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Infrastructure.Factories;
using TVA.Demo.App.Infrastructure.Providers;
using TVA.Demo.App.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnectionFactory, SqlConnectionFactory>();
builder.Services.AddSingleton<IDbConnectionProvider, DbConnectionProvider>();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost9000",
        policy =>
        {
            policy.WithOrigins("http://localhost:9000")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowLocalhost9000");

app.MapControllers();

await app.RunAsync();
