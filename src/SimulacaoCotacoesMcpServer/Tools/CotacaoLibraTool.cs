using ModelContextProtocol.Server;
using SimulacaoCotacoesMcpServer.Models;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;

namespace SimulacaoCotacoesMcpServer.Tools;

/// <summary>
/// MCP Tool para obter a cotacao atual da libra esterlina em reais.
/// </summary>
internal class CotacaoLibraTool(ConnectionMultiplexer redisConnection)
{
    private ConnectionMultiplexer _redisConnection = redisConnection;

    [McpServerTool]
    [Description("Retorna a cotacao atual em reais da libra esterlina.")]
    public async Task<Result> ObterCotacaoLibra()
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
                result.Message = "Cotacao da libra esterlina obtida com sucesso!";
                result.Data = new Cotacao
                {
                    Moeda = "Libra Esterlina (GBP)",
                    UltimaAtualizacao = JsonSerializer.Deserialize<DateTime>(dict["UltimaAtualizacao"]),
                    Valor = JsonSerializer.Deserialize<decimal>(dict["Libra"])
                };
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Cotacao da libra esterlina nao encontrada no Redis.";
            }
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao obter a cotacao da libra esterlina: {ex.Message}"
            };
        }
    }
}