using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Flaky.Api.Models;

namespace Flaky.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EchoController : ControllerBase
{
    private readonly ILogger<EchoController> _logger;
    private readonly NpgsqlConnection _dbConnection;

    public EchoController(ILogger<EchoController> logger, NpgsqlConnection dbConnection)
    {
        _logger = logger;
        _dbConnection = dbConnection;
    }

    [HttpGet]
    public async Task<ActionResult<EchoResponse>> Get(string message)
    {
        await _dbConnection.OpenAsync();
        using var cmd = new NpgsqlCommand("SELECT 1", _dbConnection);
        await cmd.ExecuteNonQueryAsync();

        return Ok(new EchoResponse
        {
            Message = $"You said: {message}"
        });
    }
}
