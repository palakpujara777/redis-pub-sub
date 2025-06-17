using CallProcess.Application;
using CallProcess.Application.Features.CallPrefixes.Commands;
using CallProcess.Application.Features.CallPrefixes.Queries;
using CallProcess.Application.Interfaces;
using CallProcess.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICallPrefixRepository, InMemoryCallPrefixRepository>();
builder.Services.AddScoped<GetAllCallPrefixesHandler>();
builder.Services.AddScoped<GetCallPrefixByCodeHandler>();
builder.Services.AddScoped<AddOrUpdateCallPrefixHandler>();
builder.Services.AddScoped<DeleteCallPrefixHandler>();

// Cache support to store data in cache
var cacheConnectionString = builder.Configuration.GetSection("CacheSettings:ConnectionString").Value ?? string.Empty;
builder.Services.InitializeApplication(cacheConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
