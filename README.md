# eCommerce Backend en .NET

Version base del proyecto con API, capas productivas y frontend local.

## Stack

- .NET 10
- ASP.NET Core Web API con controllers
- Blazor Server como frontend local
- Arquitectura por capas
- Repositorios in-memory

## Estructura

```text
src/
  Course.Domain          Entidades, estados y reglas de negocio
  Course.Application     Casos de uso, DTOs e interfaces
  Course.Infrastructure  Repositorios in-memory y gateway de pago simulado
  Course.Api             Controllers, DI y pipeline HTTP
  Course.Web             Frontend Blazor Server que consume Course.Api
```

## Casos de uso

- Crear pedido: `POST /api/orders`
- Consultar pedido: `GET /api/orders/{id}`
- Cancelar pedido: `POST /api/orders/{id}/cancel`
- Calcular total
- Validar stock
- Simular pago

## Ejecutar

En una terminal:

```bash
dotnet run --project src/Course.Api/Course.Api.csproj --launch-profile http
```

En otra terminal:

```bash
dotnet run --project src/Course.Web/Course.Web.csproj --launch-profile http
```

La API queda en `http://localhost:5151` y el frontend en `http://localhost:5206`.

## Endpoints

- `GET /health`
- `GET /api/products`
- `POST /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders/{id}/cancel`

Por defecto, `Course.Web` consume `Course.Api` en `http://localhost:5151` mediante la clave `CourseApi:BaseUrl`.
