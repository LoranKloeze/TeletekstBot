using Serilog;
using TeletekstBotHangfire.Services;
using TeletekstBotHangfire.Services.Interfaces;
using TeletekstBotHangfire.Services.Utils;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilog();
builder.Services.AddScoped<IHeadlinesService, HeadlinesService>();
builder.Services.AddScoped<HeadlinesScraper>();
builder.Services.AddScoped<ITeletekstPageService, TeletekstPageService>();
builder.Services.AddScoped<TeletekstPageScraper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/headlines", async (IHeadlinesService headlinesService, ITeletekstPageService pageService) =>
    {
        var headlines = await headlinesService.GetAllAsync();
        
        var firstOne = headlines.FirstOrDefault();
        if (firstOne == null)
        {
            return Results.NotFound();
        }

        var page = await pageService.GetPageAsync(firstOne.PageNr);

        return Results.Ok(page);
    })
    .WithName("GetHeadlines")
    .WithOpenApi();

app.Run();