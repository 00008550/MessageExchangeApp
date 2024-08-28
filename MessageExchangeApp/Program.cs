using MessageExchangeApp.CustomLogger;
using MessageExchangeApp.Implementations;
using MessageExchangeApp.Interfaces;
using MessageExchangeApp.WebSocketSetup;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<WebSocketHandler>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();

// Configure PostgreSQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

// Add custom file logger
builder.Logging.ClearProviders(); 
builder.Logging.AddProvider(new FileLoggerProvider("Logs"));  // Logs directory

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Message Exchange API",
        Version = "v1",
        Description = "API for exchanging messages"
    });
});

var app = builder.Build();

app.UseWebSockets();

// Configure WebSocket endpoint
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        await webSocketHandler.Handle(context, webSocket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Enable Swagger middleware at /swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Exchange API v1");
    c.RoutePrefix = "swagger";  // Swagger UI is available at /swagger
});

// Map controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Messages}/{action=Index}/{id?}");

app.Run();
