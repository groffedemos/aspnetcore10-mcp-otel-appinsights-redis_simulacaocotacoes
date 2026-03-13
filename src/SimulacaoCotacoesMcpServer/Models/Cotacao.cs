namespace SimulacaoCotacoesMcpServer.Models;

public class Cotacao
{
    public string? Moeda { get; set; }
    public decimal? Valor { get; set; }
    public DateTime? UltimaAtualizacao { get; set; }
}