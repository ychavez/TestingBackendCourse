# Buenas practicas

Principios del proyecto:

- Las reglas viven en Domain.
- Application orquesta casos de uso.
- Infrastructure implementa detalles reemplazables.
- API traduce HTTP a casos de uso.
- Las pruebas hablan el lenguaje del negocio.

Checklist para escribir mejores pruebas:

- Nombre del test con escenario y resultado esperado.
- Arrange, Act, Assert evidente.
- Datos pequenos y expresivos.
- Un motivo de fallo por prueba.
- Evitar sleeps, aleatoriedad innecesaria y dependencias externas.
- Preferir builders o fixtures cuando los datos crezcan.

La meta del curso es que el equipo aprenda a decidir que probar, en que nivel y con que costo de mantenimiento.
