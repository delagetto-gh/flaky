using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Npgsql;
using Flaky.Api.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("http://+:5000");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<NpgsqlConnection>(sp =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>()
                             .GetConnectionString("db");

    var conn = new NpgsqlConnection(connectionString);

    return conn;
});
builder.Services.AddHealthChecks()
                .AddCheck<DbHealthCheck>("db");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = (currentHttpContext, healthReport) =>
    {
        currentHttpContext.Response.ContentType = "application/json; charset=utf-8";

        var serviceHealthSummary = new List<object>();

        foreach (var report in healthReport.Entries)
        {
            var dependencyHealthSummary = new
            {
                dependency = report.Key,
                deetz = new
                {
                    status = report.Value.Status.ToString(),
                    info = report.Value.Description ?? string.Empty
                }
            };

            serviceHealthSummary.Add(dependencyHealthSummary);
        }
        return currentHttpContext.Response.WriteAsJsonAsync(serviceHealthSummary);
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
