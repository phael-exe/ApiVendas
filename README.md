# ApiVendas - Teste Backend

API desenvolvida em .NET 8 utilizando DDD (Domain-Driven Design) para gerenciamento de pedidos.

## Tecnologias
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core (In-Memory Database)
- xUnit (Testes Unitários)
- Swagger (Documentação da API)

## Funcionalidades
- Gestão completa de pedidos (Criar, Adicionar/Remover Itens, Fechar).
- Regras de negócio blindadas no Domínio (Impossível fechar pedido vazio, etc).
- Paginação e Filtros na listagem.

## Como Rodar
1. Clone o repositório.
2. Abra a solução `ApiVendas.slnx` no Visual Studio.
3. Defina o projeto `ApiVendas` como Startup Project.
4. Execute (F5). O Swagger abrirá automaticamente.