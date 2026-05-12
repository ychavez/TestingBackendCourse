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

## Gate de cobertura en CI

El workflow de GitHub Actions ejecuta un gate especifico para pruebas unitarias:

```bash
dotnet test tests/01-UnitTests/Course.UnitTests/Course.UnitTests.csproj \
  --configuration Release \
  --no-build \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:Threshold=80 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total
```

Si la cobertura total de lineas baja de 80%, el pipeline falla.

Las pruebas Playwright se ejecutan despues de instalar Chromium y no forman parte del gate unitario. El objetivo del gate es vigilar la cobertura de reglas de negocio, no forzar cobertura de la UI.
