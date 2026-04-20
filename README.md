# Good Hamburger API

API REST para registro de pedidos da lanchonete Good Hamburger, implementada em C# com ASP.NET Core.

## Objetivo

Este projeto atende ao desafio tecnico de CRUD de pedidos com regras de desconto, validacoes de negocio e endpoint de cardapio.

## Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 9
- PostgreSQL 16
- xUnit + NSubstitute (testes unitarios)

## Arquitetura

O projeto esta organizado por camadas:

- `GoodHamburger.Api`: entrada HTTP (controllers e bootstrap da aplicacao)
- `GoodHamburger.Application`: casos de uso, validacoes e contratos de repositorio
- `GoodHamburger.Domain`: entidades e regras de negocio
- `GoodHamburger.Infra`: persistencia, EF Core, migrations e seed
- `GoodHamburger.Application.Tests`: testes unitarios de casos de uso
- `GoodHamburger.Domain.Tests`: testes unitarios de regra de desconto

Este README concentra as informacoes principais para execucao, arquitetura e uso da API.

## Como executar localmente

### 1) Preparar variaveis de ambiente

Copie `.env.example` para `.env` e ajuste se necessario.

### 2) Subir banco com Docker

```bash
docker compose up -d
```

### 3) Restaurar pacotes

```bash
dotnet restore
```

### 4) Aplicar migrations

Se `dotnet ef` estiver instalado globalmente:

```bash
dotnet ef database update --project GoodHamburger.Infra --startup-project GoodHamburger.Api
```

Se usar ferramenta local por manifest:

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project GoodHamburger.Infra --startup-project GoodHamburger.Api
```

### 5) Rodar API

```bash
dotnet run --project GoodHamburger.Api
```

A API sobe com Swagger/OpenAPI em ambiente de desenvolvimento.

## Como rodar testes

```bash
dotnet test
```

Cobertura atual:

- `GoodHamburger.Domain.Tests`: regras de desconto
- `GoodHamburger.Application.Tests`: casos de uso de pedidos e cardapio

## Regras de negocio implementadas

- Combos e categorias sao data-driven (vindos do banco)
- O pedido aplica somente o melhor combo elegivel
- Nao permite categorias repetidas no mesmo pedido
- Validacao de itens duplicados por ID e por categoria
- Validacao de nome do cliente obrigatorio

## Historico de pedidos (snapshot)

Para manter historico consistente, o pedido persiste:

- `unit_price` por item em `order_products`
- `subtotal`, `discount` e `total_price` em `orders`

Com isso, mudancas futuras de preco ou regra de desconto nao alteram pedidos ja registrados.

## Categorias e combos no banco

O desconto nao depende mais de enum fixo em codigo. A regra e baseada em dados:

- `product_categories`: categorias de produto
- `combos`: percentual de desconto e status ativo
- `combo_categories`: categorias exigidas por combo

No calculo, quando mais de um combo e elegivel, a API aplica somente o melhor combo (maior desconto).

## Endpoints principais

- `GET /api/menu`
- `POST /api/orders`
- `GET /api/orders`
- `GET /api/orders/{id}`
- `PUT /api/orders/{id}`
- `DELETE /api/orders/{id}`

Os contratos dos endpoints estao resumidos neste proprio README.

## Erros de negocio

Padrao de payload:

```json
{
  "code": "INVALID_ORDER",
  "message": "Pedido invalido: ..."
}
```

Codigos usados:

- `INVALID_ORDER`
- `DUPLICATE_ORDER_ITEMS`
- `RESOURCE_NOT_FOUND`

## Itens fora do escopo

- Frontend (Blazor)
- Testes de integracao HTTP e banco real
- Autenticacao/autorizacao
