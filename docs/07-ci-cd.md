# CI/CD

El workflow esta en `.github/workflows/ci.yml`.

Pipeline incluido:

- Checkout del repositorio.
- Instalacion de .NET 10.
- Restore.
- Build en Release.
- Gate de cobertura para pruebas unitarias con minimo 80% de lineas.
- Test backend con cobertura usando Coverlet.
- Instalacion de Chromium para Playwright.
- Pruebas de navegador contra `Course.Web`.
- Publicacion del archivo Cobertura como artifact.

Practicas recomendadas:

- Mantener CI rapido.
- Separar performance tests de la ejecucion comun.
- Mantener pocas pruebas de navegador, enfocadas en flujos criticos.
- Fallar el pipeline ante regresiones de pruebas.
- Revisar cobertura como senal, no como objetivo unico.

El trigger principal es `push` a `main`, por lo que el gate de cobertura y las pruebas Playwright se ejecutan antes de considerar estable esa rama.
