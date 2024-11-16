using Hangfire;
using Hangfire.PostgreSql;
using Mastonet;
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
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var runToFillDb = args.Length > 0 && args[0] == "filldb";

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
if (!runToFillDb)
{
    builder.Services.AddHangfireServer(options => { options.Queues = ["default"]; });
}

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();
builder.Services.AddTransient<IMastodonClient, MastodonClient>(_ =>
{
    var accessToken = builder.Configuration["Mastodon:AccessToken"] ?? 
                      throw new InvalidOperationException("Mastodon access token not found.");
    return new MastodonClient("mastodon.nl", accessToken);
});
builder.Services.AddScoped<ICurrentPagesService, CurrentPagesService>();
builder.Services.AddScoped<HeadlinesScraper>();
builder.Services.AddScoped<ITeletekstPageService, TeletekstPageService>();
builder.Services.AddScoped<TeletekstPageScraper>();
builder.Services.AddScoped<PostNewPagesJob>();
builder.Services.AddHttpClient<IBlueSkyPostsService, BlueSkyPostsService>();
builder.Services.AddScoped<IMastodonPostsService, MastodonPostsService>();
var app = builder.Build();
var isDevEnv = app.Environment.IsDevelopment();
// Configure the HTTP request pipeline.
if (isDevEnv)
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

// Setup Hangfire jobs
RecurringJob.AddOrUpdate<PostNewPagesJob>("postNewPages",
    "default", x => x.StartAsync(new PostNewPagesJobOptions
    {
        PostToSocialMedia = true
    }),
    Cron.Never());
if (runToFillDb)
{
    Console.WriteLine("Running PostNewPagesJob to fill the database without posting to social media");
    var scope = app.Services.CreateScope();
    var job = scope.ServiceProvider.GetRequiredService<PostNewPagesJob>();
    job.StartAsync(new PostNewPagesJobOptions
    {
        PostToSocialMedia = false
    }).Wait();
    Console.WriteLine("Done, bye!");
}
else
{
    app.Run();
}
