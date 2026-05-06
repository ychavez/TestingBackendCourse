# Pruebas de integracion

Las pruebas de integracion viven en `tests/03-IntegrationTests/Course.IntegrationTests`.

Usan `WebApplicationFactory<Program>` para levantar la API en memoria. Esto permite probar routing, model binding, controllers, DI y servicios reales sin abrir puertos.

Que se valida:

- `POST /api/orders` crea un pedido real.
- La API devuelve `201 Created`.
- Las reglas de stock llegan hasta HTTP como `400 Bad Request`.

Este nivel es ideal para detectar errores de configuracion que una prueba unitaria no ve.
