# Performance con NBomber

Las pruebas de performance viven en `tests/06-PerformanceTests/Course.PerformanceTests`.

NBomber permite modelar carga:

- Usuarios o peticiones por segundo.
- Duracion de la prueba.
- Latencia y tasa de errores.
- Reportes para analisis.

En este proyecto el test esta marcado con `Skip` para no ejecutarse en cada `dotnet test`. En clase, quita el `Skip` y ajusta:

- `rate`
- `interval`
- `during`

Una prueba de performance util empieza pequena, mide una hipotesis concreta y se repite bajo condiciones comparables.
