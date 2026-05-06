# Cobertura con Coverlet

Los proyectos de prueba incluyen `coverlet.collector`, por lo que puedes ejecutar:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

El resultado se genera bajo `TestResults/` en formato Cobertura.

Para convertirlo a HTML instala ReportGenerator:

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

Lectura recomendada para clase:

- No perseguir 100% como metrica aislada.
- Priorizar reglas de negocio y flujos criticos.
- Revisar ramas no cubiertas, no solo lineas.
- Combinar cobertura con pruebas legibles y mantenibles.
