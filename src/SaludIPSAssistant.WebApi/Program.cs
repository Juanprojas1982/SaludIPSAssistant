using Microsoft.EntityFrameworkCore;
using SaludIPSAssistant.Application.Interfaces;
using SaludIPSAssistant.Infrastructure.Persistence;
using SaludIPSAssistant.Infrastructure.Repositories;
using SaludIPSAssistant.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(SaludIPSAssistant.Application.Commands.ProcessIncomingMessageCommand).Assembly);
});

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories and unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add external services
builder.Services.AddHttpClient<INotificationService, WhatsAppNotificationService>();
builder.Services.AddHttpClient<IAgendaProvider, MockAgendaProvider>();
builder.Services.AddScoped<INlpService, SimpleNlpService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
