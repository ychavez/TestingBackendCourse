# Mocking

Las pruebas con Moq viven en `tests/02-Mocking/Course.MockingTests`.

Se usan para probar `OrderService` sin depender de repositorios reales ni gateway de pago real.

Buenas practicas:

- Mockear interfaces propias, no detalles internos.
- Verificar interacciones importantes, no cada llamada accidental.
- Usar datos concretos del caso de negocio.
- Evitar que el test repita toda la implementacion.

En el curso se muestra como simular:

- Cliente existente.
- Producto existente o inexistente.
- Pago aprobado.
- Persistencia del pedido.
