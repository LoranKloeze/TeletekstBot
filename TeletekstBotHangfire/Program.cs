using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using TeletekstBotHangfire.Data;
using TeletekstBotHangfire.Jobs;
using TeletekstBotHangfire.Services;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Services.Utils;
using TeletekstBotHangfire.Utils.Hangfire;


// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

// General setup
var builder = WebApplication.CreateBuilder(args);
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure EntityFrameworkCore

var dataSourceBuilder = new NpgsqlDataSourceBuilder(sqlConnectionString);
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

// Configure Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(sqlConnectionString)));
builder.Services.AddHangfireServer(options => { options.Queues = ["default"]; });

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();
builder.Services.AddScoped<ICurrentPagesService, CurrentPagesService>();
builder.Services.AddScoped<HeadlinesScraper>();
builder.Services.AddScoped<ITeletekstPageService, TeletekstPageService>();
builder.Services.AddScoped<TeletekstPageScraper>();
builder.Services.AddScoped<PostNewPagesJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire",
    new DashboardOptions
    {
        Authorization = [new HangfireAuthorizationFilter()]
    });

app.MapGet("/postnewpages", async (PostNewPagesJob postNewPagesJob) =>
    {
        await postNewPagesJob.RunAsync();

        return Results.Ok("blabla");
    })
    .WithName("GetHeadlines")
    .WithOpenApi();
app.Run();