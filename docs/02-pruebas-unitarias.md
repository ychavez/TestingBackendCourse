# Pruebas unitarias

Las pruebas unitarias viven en `tests/01-UnitTests/Course.UnitTests`.

Objetivo:

- Probar reglas de negocio sin API, base de datos ni red.
- Mantener asserts claros con FluentAssertions.
- Cubrir casos felices y errores de dominio.

Ejemplos del proyecto:

- `Product.DecreaseStock` actualiza inventario.
- `Product.DecreaseStock` falla cuando no hay stock.
- `Order.CalculateTotal` suma items.
- `Order.Cancel` bloquea pedidos pagados.

Una buena prueba unitaria debe ser pequena, deterministica y facil de leer meses despues.
