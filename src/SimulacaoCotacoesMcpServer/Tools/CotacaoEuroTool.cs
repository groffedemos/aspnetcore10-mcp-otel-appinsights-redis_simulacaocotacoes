using ModelContextProtocol.Server;
using SimulacaoCotacoesMcpServer.Models;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;

namespace SimulacaoCotacoesMcpServer.Tools;

/// <summary>
/// MCP Tool para obter a cotacao atual do euro em reais.
/// </summary>
internal class CotacaoEuroTool(ConnectionMultiplexer redisConnection)
{
    private ConnectionMultiplexer _redisConnection = redisConnection;

    [McpServerTool]
    [Description("Retorna a cotacao atual em reais do euro.")]
    public async Task<Result> ObterCotacaoEuro()
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
                result.Message = "Cotacao do euro obtida com sucesso!";
                result.Data = new Cotacao
                {
                    Moeda = "Euro (EUR)",
                    UltimaAtualizacao = JsonSerializer.Deserialize<DateTime>(dict["UltimaAtualizacao"]),
                    Valor = JsonSerializer.Deserialize<decimal>(dict["Euro"])
                };
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Cotacao do euro nao encontrada no Redis.";
            }
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao obter a cotacao do euro: {ex.Message}"
            };
        }
    }
}