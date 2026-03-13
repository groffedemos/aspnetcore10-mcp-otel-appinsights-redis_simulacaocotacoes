# aspnetcore10-mcp-otel-appinsights-redis_simulacaocotacoes
Implementação em ASP.NET Core + .NET 10 de MCP Server para consulta a cotações de moedas estrangeiras (simulação) armazenadas em uma instância do Redis. Inclui o uso de um script do Docker Compose (ambiente de testes), monitoramento/observabilidade com OpenTelemetry + Azure Application Insights e um Dockerfile (build de imagens deste MCP).

Aplicação para geração das cotações (dólar, euro, libra esterlina): https://github.com/renatogroffe/dotnet10-consoleapp-redis_simulacaocotacoes