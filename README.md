# Testing Profesional para Backend en .NET

Proyecto didactico para ensenar pruebas profesionales en un backend ASP.NET Core realista.

## Stack

- .NET 10 para API, capas productivas y proyectos de prueba
- ASP.NET Core Web API con controllers
- Blazor Server como frontend local
- xUnit
- Moq
- FluentAssertions
- WebApplicationFactory
- Playwright para pruebas de navegador
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
  Course.Web             Frontend Blazor Server que consume Course.Api

tests/
  01-UnitTests           Pruebas puras de dominio
  02-Mocking             Pruebas con dobles usando Moq
  03-IntegrationTests    API en memoria con WebApplicationFactory
  04-SmokeTests          Verificaciones rapidas de disponibilidad
  05-EndToEndTests       Flujos completos de API
  06-PerformanceTests    Escenarios NBomber
  07-Coverage            Instrucciones de cobertura
  08-FrontendTests       Pruebas Playwright sobre Course.Web
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
dotnet run --project src/Course.Web/Course.Web.csproj
```

La API expone:

- `GET /health`
- `GET /api/products`
- `POST /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders/{id}/cancel`

Por defecto, `Course.Web` consume `Course.Api` en `http://localhost:5151` mediante la clave `CourseApi:BaseUrl`.

## Ejecutar pruebas

Los proyectos bajo `tests/` compilan contra `net10.0`.

```bash
dotnet test TestingBackendCourse.sln --filter "FullyQualifiedName!~Course.PlaywrightTests"
```

Las pruebas de performance estan marcadas como omitidas por defecto para no ralentizar CI. Para usarlas como practica del curso, quita el `Skip` en `OrderPerformanceTests`.

Para ejecutar las pruebas de navegador:

```bash
dotnet build
pwsh tests/08-FrontendTests/Course.PlaywrightTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test tests/08-FrontendTests/Course.PlaywrightTests/Course.PlaywrightTests.csproj
```

El fixture de Playwright levanta `Course.Api` y `Course.Web` en puertos locales libres.

## Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Consulta `tests/07-Coverage/coverage-instructions.md` para generar reportes HTML.

El CI incluye un gate de cobertura para `Course.UnitTests`: si la cobertura total de lineas baja de 80%, el pipeline falla.

El CI tambien instala Chromium y ejecuta las pruebas Playwright al hacer push a `main`.
