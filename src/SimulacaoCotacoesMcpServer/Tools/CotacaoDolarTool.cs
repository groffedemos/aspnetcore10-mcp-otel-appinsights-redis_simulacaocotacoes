using ModelContextProtocol.Server;
using SimulacaoCotacoesMcpServer.Models;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;

namespace SimulacaoCotacoesMcpServer.Tools;

/// <summary>
/// MCP Tool para obter a cotacao atual do dolar em reais.
/// </summary>
internal class CotacaoDolarTool(ConnectionMultiplexer redisConnection)
{
    private ConnectionMultiplexer _redisConnection = redisConnection;

    [McpServerTool]
    [Description("Retorna a cotacao atual em reais do dolar.")]
    public async Task<Result> ObterCotacaoDolar()
    {
        try
        {
            var result = new Result();
            var dbRedis = _redisConnection.GetDatabase();
            if (dbRedis.KeyExists("SimulacaoCotacoes"))
            {
                var hashEntries = dbRedis.HashGetAll("SimulacaoCotacoes");
                var dict = hashEntries.ToDictionary(
                    he => he.Name.ToString(),
                    he => he.Value.ToString());
                result.IsSuccess = true;
                result.Message = "Cotacao do dolar obtida com sucesso!";
                result.Data = new Cotacao
                {
                    Moeda = "Dolar (USD)",
                    UltimaAtualizacao = JsonSerializer.Deserialize<DateTime>(dict["UltimaAtualizacao"]),
                    Valor = JsonSerializer.Deserialize<decimal>(dict["Dolar"])
                };
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Cotacao do dolar nao encontrada no Redis.";
            }
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao obter a cotacao do dolar: {ex.Message}"
            };
        }
    }
}