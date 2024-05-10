using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

//var configuration = new ConfigurationBuilder()
//           .SetBasePath(Directory.GetCurrentDirectory())
//           .AddJsonFile("appsettings.json")
//           .AddJsonFile("appsettings.Development.json")
//           .Build();

//Creating the Logger with Minimum Settings
Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//Read from appsettings.json
builder.Services.AddSerilog(options =>
{
    //Override Few of the Configurations
    options.Enrich.WithProperty("Application", "ProductAPI")
       .Enrich.WithProperty("Environment", "Dev")
       .WriteTo.Console(new RenderedCompactJsonFormatter())
       .WriteTo.GrafanaLoki("http://loki:3100");
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();
