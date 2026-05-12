# Frontend Blazor y Playwright

`Course.Web` es una app Blazor Server separada que consume `Course.Api` por HTTP local. La configuracion vive en `CourseApi:BaseUrl` y por defecto apunta a `http://localhost:5151`.

## Por que Blazor Server

- Mantiene el curso completo en .NET 10.
- Evita CORS porque las llamadas hacia la API ocurren servidor-a-servidor.
- Permite probar un flujo visible sin introducir un stack frontend adicional.

## Flujo implementado

- Cargar productos desde `GET /api/products`.
- Seleccionar cantidades y crear pedido con `POST /api/orders`.
- Mostrar estado, total y pago del pedido.
- Consultar pedido por id.
- Cancelar pedidos en estado `Created` o `PaymentRejected`.
- Mostrar errores de stock y reglas de negocio.

La UI expone `data-testid` estables para que las pruebas no dependan de clases CSS ni estructura visual.

## Pruebas Playwright

El proyecto `tests/08-FrontendTests/Course.PlaywrightTests` usa `Microsoft.Playwright.Xunit` y Chromium.

El fixture:

- Busca la raiz por `TestingBackendCourse.sln`.
- Arranca `Course.Api` en un puerto libre.
- Arranca `Course.Web` en otro puerto libre.
- Inyecta `CourseApi:BaseUrl` al proceso Blazor.
- Espera `/health` de API y Web.
- Apaga ambos procesos al finalizar.

Escenarios cubiertos:

- El catalogo carga y la UI muestra estado conectado.
- Un pedido de `Ergonomic Mouse` cantidad `2` termina pagado.
- `Developer Laptop` cantidad `99` muestra error de stock.
- `Developer Laptop` cantidad `1` queda rechazado por pago y se puede cancelar.

## Ejecucion local

```bash
dotnet build
pwsh tests/08-FrontendTests/Course.PlaywrightTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test tests/08-FrontendTests/Course.PlaywrightTests/Course.PlaywrightTests.csproj
```

## CI

GitHub Actions compila la solucion, valida el gate de cobertura unitaria, ejecuta pruebas backend con cobertura, instala Chromium y despues corre `Course.PlaywrightTests`.
