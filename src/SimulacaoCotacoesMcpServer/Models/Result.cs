namespace SimulacaoCotacoesMcpServer.Models;

public class Result
{
    public bool? IsSuccess { get; set; }
    public string? Message { get; set; }
    public Cotacao? Data { get; set; }
}