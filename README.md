# Testing Profesional para Backend en .NET

Proyecto didactico para ensenar pruebas profesionales en un backend ASP.NET Core realista.

## Stack

- .NET 8
- ASP.NET Core Web API con controllers
- xUnit
- Moq
- FluentAssertions
- WebApplicationFactory
- NBomber
- Coverlet
- GitHub Actions

## Arquitectura

```text
src/
  Course.Domain          Entidades, estados y reglas de negocio
  Course.Application     Casos de uso, DTOs e interfaces
  Course.Infrastructure  Repositorios in-memory y gateway de pago simulado
  Course.Api             Controllers, DI y pipeline HTTP

tests/
  01-UnitTests           Pruebas puras de dominio
  02-Mocking             Pruebas con dobles usando Moq
  03-IntegrationTests    API en memoria con WebApplicationFactory
  04-SmokeTests          Verificaciones rapidas de disponibilidad
  05-EndToEndTests       Flujos completos de API
  06-PerformanceTests    Escenarios NBomber
  07-Coverage            Instrucciones de cobertura
```

## Casos de uso implementados

- Crear pedido: `POST /api/orders`
- Consultar pedido: `GET /api/orders/{id}`
- Cancelar pedido: `POST /api/orders/{id}/cancel`
- Calcular total: regla en `Order`
- Validar stock: regla en `Product` y `OrderItem`
- Simular pago: `FakePaymentGateway`

## Ejecutar

```bash
dotnet restore
dotnet build
dotnet run --project src/Course.Api/Course.Api.csproj
```

La API expone:

- `GET /health`
- `GET /api/products`
- `POST /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders/{id}/cancel`

## Ejecutar pruebas

```bash
dotnet test
```

Las pruebas de performance estan marcadas como omitidas por defecto para no ralentizar CI. Para usarlas como practica del curso, quita el `Skip` en `OrderPerformanceTests`.

## Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Consulta `tests/07-Coverage/coverage-instructions.md` para generar reportes HTML.
